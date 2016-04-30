using PoolSharp;
using ScribeSharp.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace ScribeSharp
{
	/// <summary>
	/// Used to write log events. Default implementation of <see cref="ILogger"/> and the primary class used by client code. 
	/// </summary>
	public class Logger : ILogger, IDisposable
	{
		//TODO: Log operation stacks? recursive? WIP?
		//TODO: Call contexts? Async/thread/logical?
		//TODO: Property renderers? JsonRenderer
		//TODO: Message renderer separate to log event renderer/property renderer?
		//TODO: Json serialiser
		//TODO: Single filtering writer instead of base class?
		//TODO: Level switch

		#region Fields

		private readonly ILogEventPool _EntryPool;
		private readonly ILoggedJobPool _JobPool;
		private readonly ILogClock _LogClock;
		private readonly ILogEventFilter _Filter;
		private readonly ILogWriter _LogWriter;
		private readonly ILogEventContextProvider[] _ContextProviders;
		private readonly IDictionary<Type, IPropertyRenderer> _PropertyRenderers;
		private readonly ILoggingErrorHandler _ErrorHandler;
		private readonly string _Source;

		private bool _HasPropertyRenderers;
		private bool _IsEnabled;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="policy">A <see cref="LogPolicy"/> instance used to configure this logger.</param>
		/// <remarks>
		/// <para>The <paramref name="policy"/> should not be changed after being provided to the logger. Values from the policy are cached or copied before use and will not change even if the policy is updated after the logger is constructed.</para>
		/// </remarks>
		public Logger(LogPolicy policy)
		{
			if (policy == null) throw new ArgumentNullException(nameof(policy));
			if (policy.LogWriter == null) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeNull, "policy.LogWriter"), nameof(policy));

			_ErrorHandler = policy.ErrorHandler ?? SuppressingErrorHandler.DefaultInstance;
			_EntryPool = new LogEventPool(policy.LogEventPoolCapacity);
			_JobPool = new LoggedJobPool(policy.JobPoolCapacity);

			_LogWriter = policy.LogWriter;
			_LogClock = policy.Clock;
			_Filter = policy.Filter;
			_Source = policy.Source;

			if (policy.PropertyRenderers != null)
			{
				_PropertyRenderers = new Dictionary<Type, IPropertyRenderer>(policy.PropertyRenderers);
				_HasPropertyRenderers = Utils.Any(_PropertyRenderers);
			}
			else
				_HasPropertyRenderers = false;

			if (policy.ContextProviders != null)
				_ContextProviders = (policy.ContextProviders as ILogEventContextProvider[]) ?? policy.ContextProviders.ToArray();

			_IsEnabled = true;
		}

		#endregion

		#region ILogger

		#region WriteEvent Overloads


		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="eventType">A <see cref="LogEventType"/> to assign to the written log entry. The defaultvalue if <see cref="LogEventType.ApplicationEvent"/>.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		/// <param name="source">A string containing the source to assign to <see cref="LogEvent.Source"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerFilePathAttribute.</param>
		/// <param name="sourceMethod">A string containing the method name to assign to <see cref="LogEvent.SourceMethod"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerMemberNameAttribute.</param>
		/// <param name="sourceLineNumber">The line number of the source code at which this method was called.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "The previous call to Utils.Any technically does the validation, code analysis just can't figure that out.")]
		public void WriteEvent(string eventName, LogEventSeverity eventSeverity = LogEventSeverity.Information, LogEventType eventType = LogEventType.ApplicationEvent,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerFilePath] 
#endif
			string source = null,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerMemberName] 
#endif
			string sourceMethod = null,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerLineNumber] 
#endif
			int sourceLineNumber = 0,
			params KeyValuePair<string, object>[] properties)
		{
			try
			{
				if (!IsEnabled) return;

				using (var pooledLogEvent = _EntryPool.Take())
				{
					var logEvent = pooledLogEvent.Value;
					logEvent.DateTime = _LogClock?.Now ?? DateTimeOffset.Now;
					logEvent.EventName = eventName ?? String.Empty;
					logEvent.EventSeverity = eventSeverity;
					logEvent.EventType = eventType;

					if (Utils.Any<KeyValuePair<string, object>>(properties))
					{
						var eventProperties = (logEvent.Properties = logEvent.Properties ?? new Dictionary<string, object>());
						foreach (var property in properties)
						{
							if (_HasPropertyRenderers)
								eventProperties[property.Key] = RenderProperty(property.Value);
							else
								eventProperties[property.Key] = property.Value;
						}
					}

					UnsafeWriteEvent(logEvent, source, sourceMethod, sourceLineNumber);
				}
			}
			catch (StackOverflowException) { throw; }
			catch (LogException lex)
			{
				if (_ErrorHandler.ReportError(lex) == LoggingErrorPolicy.Rethrow)
					throw;
			}
			catch (Exception ex)
			{
				var wrappedException = new LogException(ex.Message, ex);
				if (_ErrorHandler.ReportError(wrappedException) == LoggingErrorPolicy.Rethrow)
					throw wrappedException;
			}
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance containing data to write.</param>
		/// <param name="source">A string containing the source to assign to <see cref="LogEvent.Source"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerFilePathAttribute.</param>
		/// <param name="sourceMethod">A string containing the method name to assign to <see cref="LogEvent.SourceMethod"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerMemberNameAttribute.</param>
		/// <param name="sourceLineNumber">The line number of the source code at which this method was called.</param>
		public void WriteEvent(LogEvent logEvent,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerFilePath] 
#endif
			string source = null,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerMemberName] 
#endif
			string sourceMethod = null,
#if SUPPORTS_CALLERATTRIBUTES
			[System.Runtime.CompilerServices.CallerLineNumber] 
#endif
			int sourceLineNumber = 0)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			try
			{
				if (!IsEnabled) return;

				UnsafeWriteEvent(logEvent, source, sourceMethod, sourceLineNumber);
			}
			catch (StackOverflowException) { throw; }
			catch (LogException lex)
			{
				if (_ErrorHandler.ReportError(lex) == LoggingErrorPolicy.Rethrow)
					throw;
			}
			catch (Exception ex)
			{
				var wrappedException = new LogException(ex.Message, ex);
				if (_ErrorHandler.ReportError(wrappedException) == LoggingErrorPolicy.Rethrow)
					throw wrappedException;
			}
		}

		#endregion

		#region CreateChildLogger Overloads

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, using the full type name of <typeparam name="T"/> as the source.
		/// </summary>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		public ILogger CreateChildLogger<T>()
		{
			return CreateChildLogger(typeof(T).FullName);
		}

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, using the full type name of <typeparam name="T"/> as the source.
		/// </summary>
		/// <param name="contextProviders">A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="ILogWriter"/>.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		public ILogger CreateChildLogger<T>(IEnumerable<ILogEventContextProvider> contextProviders)
		{
			return CreateChildLogger(typeof(T).FullName, contextProviders);
		}

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, but with an alternate source.
		/// </summary>
		/// <param name="source">The source to apply to all log event entries. If null, the inner source or system supplied source will be used.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		public ILogger CreateChildLogger(string source)
		{
			return CreateChildLogger(
				new LogPolicy()
				{
					Source = source
				}
			);
		}

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, but with an alternate source.
		/// </summary>
		/// <param name="source">The source to apply to all log event entries. If null, the inner source or system supplied source will be used.</param>
		/// <param name="contextProviders">A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="ILogWriter"/>.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		public ILogger CreateChildLogger(string source, IEnumerable<ILogEventContextProvider> contextProviders)
		{
			var policy = new LogPolicy()
			{
				ContextProviders = contextProviders,
				Source = source
			};

			return CreateChildLogger(policy);
		}

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, applying the supplied policy to log events before forwarding them to this logger for processing.
		/// </summary>
		/// <remarks>
		/// <para>The returned logger wraps this instance, so log entries are written to this logger but only if the filters on both loggers pass. Properties provided by context providers are applied by the wrapped instance first.</para>
		/// <para>If the <paramref name="policy"/> specifies a non-null log writer, then log events are written to both that writer and this logger.</para>
		/// </remarks>
		/// <param name="policy">The policy to apply to the wrapping logger.</param>
		/// <returns>An implementation of <see cref="ILogger"/> that wraps this one.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="policy"/> is null.</exception>
		public ILogger CreateChildLogger(LogPolicy policy)
		{
			if (policy == null) throw new ArgumentNullException(nameof(policy));

			policy.ErrorHandler = _ErrorHandler;
			policy.Clock = policy.Clock ?? MinTimeClock.DefaultInstance;
			if (policy.LogWriter == null)
				policy.LogWriter = new Writers.ForwardingLogWriter(this);
			else
				policy.LogWriter = new Writers.AggregateLogWriter(new ILogWriter[] { policy.LogWriter, new Writers.ForwardingLogWriter(this) });

			return new Logger(policy);
		}

		#endregion

		#region ExecuteLoggedJob Overloads

		/// <summary>
		/// Executes the specified action as a logged job, including start/completed events, associated errors and the total duration.
		/// </summary>
		/// <remarks>
		/// <para>Errors that occur within the job (action) are re-thrown, and need to be caught by the caller unless they want them to be unhandled (or caught further up the call stack). The exceptions will be logged and the job status updated, even though the exception is re-thrown.</para>
		/// </remarks>
		/// <param name="job">A <see cref="Action"/> to execute.</param>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="job"/> is null.</exception>
		public void ExecuteLoggedJob(Action job, string jobName, string jobId)
		{
			ExecuteLoggedJob(job, jobName, jobId, null);
		}

		/// <summary>
		/// Executes the specified action as a logged job, including start/completed events, associated errors and the total duration.
		/// </summary>
		/// <remarks>
		/// <para>Errors that occur within the job (action) are re-thrown, and need to be caught by the caller unless they want them to be unhandled (or caught further up the call stack). The exceptions will be logged and the job status updated, even though the exception is re-thrown.</para>
		/// </remarks>
		/// <param name="job">A <see cref="Action"/> to execute.</param>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <param name="properties">A set of additional properties to include on each event logged in relation to the job.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="job"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void ExecuteLoggedJob(Action job, string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties)
		{
			if (job == null) throw new ArgumentNullException(nameof(jobName));

			var token = BeginLoggedJob(jobName, jobId, properties);
			try
			{
				job();
			}
			catch (StackOverflowException)
			{
				throw;
			}
			catch (Exception ex)
			{
				token.SetFailure(ex);
				throw;
			}
			finally
			{
				token.Dispose();
			}
		}

		#endregion

		#region BeginLoggedJob Overloads

		/// <summary>
		/// Returns a <see cref="LoggedJob"/> that can be used to track a job, including start, completed and failure events, along with duration.
		/// </summary>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <returns>A <see cref="LoggedJob"/> instance for this specified job.</returns>
		public LoggedJob BeginLoggedJob(string jobName, string jobId)
		{
			return BeginLoggedJob(jobName, jobId, null);
		}

		/// <summary>
		/// Returns a <see cref="LoggedJob"/> that can be used to track a job, including start, completed and failure events, along with duration.
		/// </summary>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <param name="properties">A set of additional properties to include on each event logged in relation to the job.</param>
		/// <returns>A <see cref="LoggedJob"/> instance for this specified job.</returns>
		public LoggedJob BeginLoggedJob(string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties)
		{
			var retVal = _JobPool.Take();
			retVal.Initialize(this, jobName, jobId, properties, _JobPool);
			return retVal;
		}

		#endregion

		/// <summary>
		/// Sets or returns a boolean indicating if logging is currently enabled.
		/// </summary>
		/// <remarks>
		/// <para>Defaults to true.</para>
		/// <para>If false then no log events are written regardless of other settings and calling the <see cref="WriteEvent(string, LogEventSeverity, LogEventType, string, string, int, KeyValuePair{string, object}[])"/> overloads returns quickly without doing any work. If true, events are written based on the logger configuration.</para>
		/// </remarks>
		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}

			set
			{
				_IsEnabled = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Disposes this instance and all internal resources.
		/// </summary>
		/// <param name="isDisposing">True if the class is being explicitly disposed, false if it is being finalised.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_JobPool")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_EntryPool", Justification = "It is disposed, CA just can't tell.")]
		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				(_LogWriter as IDisposable)?.Dispose();
				_EntryPool?.Dispose();
				_JobPool?.Dispose();
			}
		}

		/// <summary>
		/// Called to apply additional properties to each log event written/created by this logger.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to modify.</param>
		protected virtual void FillProperties(LogEvent logEvent)
		{
			if (logEvent == null) return;
			if ((_ContextProviders?.Length ?? 0) == 0) return;

			if (logEvent.Properties == null) logEvent.Properties = new Dictionary<string, object>(_ContextProviders.Length);

			ILogEventContextProvider provider = null;
			var properties = logEvent.Properties;
			for (int cnt = 0; cnt < _ContextProviders.Length; cnt++)
			{
				provider = _ContextProviders[cnt];
				if (!properties.ContainsKey(provider.PropertyName) && (provider?.Filter?.ShouldProcess(logEvent) ?? true))
				{
					if (_HasPropertyRenderers)
						properties[provider.PropertyName] = RenderProperty(provider.GetValue());
					else
						properties[provider.PropertyName] = provider.GetValue();
				}
			}
		}

		/// <summary>
		/// Returns a boolean indicating if the specified <see cref="LogEvent"/> instance should be written.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to analyse.</param>
		/// <returns>True if the log event should be written, false to ignore it.</returns>
		protected virtual bool ShouldLogEvent(LogEvent logEvent)
		{
			return _Filter?.ShouldProcess(logEvent) ?? true;
		}

		#endregion

		#region Private Methods

		private void UnsafeWriteEvent(LogEvent logEvent, string source, string sourceMethod, int sourceLineNumber)
		{
			if (logEvent.DateTime == DateTimeOffset.MinValue)
				logEvent.DateTime = _LogClock?.Now ?? DateTimeOffset.Now;

			if (String.IsNullOrEmpty(logEvent.Source))
				logEvent.Source = _Source ?? source;

			if (String.IsNullOrWhiteSpace(logEvent.SourceMethod))
				logEvent.SourceMethod = sourceMethod;
			if (logEvent.SourceLineNumber == 0)
				logEvent.SourceLineNumber = sourceLineNumber;

			FillProperties(logEvent);

			if (ShouldLogEvent(logEvent))
			{
				if (_LogWriter.RequiresSynchronization)
				{
					lock (this)
					{
						UnsynchronisedWrite(logEvent);
					}
				}
				else
					UnsynchronisedWrite(logEvent);
			}
		}

		private object RenderProperty(object value)
		{
			if (value == null) return null;

			var type = value.GetType();
			if (_PropertyRenderers.ContainsKey(value.GetType())) return _PropertyRenderers[type].RenderValue(value);

			return value;
		}

		private void UnsynchronisedWrite(LogEvent LogEvent)
		{
			_LogWriter.Write(LogEvent);
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Dispsoses this object and all internal resources.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		#endregion

	}
}
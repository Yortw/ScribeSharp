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
	public class Logger : ILogger, IFlushable, IDisposable
	{
		//TODO: Logger extension methods?
		//TODO: Logger.WriteFormat? Message renderer separate to log event renderer/property renderer?

		//TODO: Writers
			//TODO: Sql writer
			//TODO: Msmq writer
			//TODO: Azure event hub writer

		#region Fields

		private readonly ILogEventPool _EntryPool;
		private readonly ILoggedJobPool _JobPool;
		private readonly ILogClock _LogClock;
		private readonly ILogEventFilter _Filter;
		private readonly IFirstChanceLogFilter _FirstChanceFilter;
		private readonly ILogWriter _LogWriter;
		private readonly ILogEventContextProvider[] _ContextProviders;
		private readonly ILoggingErrorHandler _ErrorHandler;
		private readonly ITypeRendererMap _RendererMap;
		private readonly string _Source;
		private readonly IPropertyRenderer _DefaultExceptionRenderer;

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
			_LogClock = policy.Clock ?? new CachingClock(new LocalSystemClock(), TimeSpan.FromTicks(16));
			_Filter = policy.Filter;
			_FirstChanceFilter = policy.FirstChanceFilter;
			_RendererMap = policy.TypeRendererMap; 
			_Source = policy.Source;
			_DefaultExceptionRenderer = policy.DefaultExceptionRenderer;

			if (policy.ContextProviders != null)
				_ContextProviders = (policy.ContextProviders as ILogEventContextProvider[]) ?? policy.ContextProviders.ToArray();

			_IsEnabled = true;
		}

		#endregion

		#region ILogger

		#region WriteEvent Overloads

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter, using the exception provided to generate a message and severity.
		/// </summary>
		/// <param name="exception">The exception associated with this log event, if any.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
		public void WriteEvent(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			WriteEvent(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ErrorOccurred, exception.GetType().FullName + ":" + exception.Message), 
				LogEventSeverity.Error,
				LogEventType.ApplicationEvent,
				exception, 
				null
			);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		public void WriteEvent(string eventName)
		{
			WriteEvent(eventName, LogEventSeverity.Information);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		public void WriteEvent(string eventName, params KeyValuePair<string, object>[] properties)
		{
			WriteEvent(eventName, null, properties);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="exception">The exception associated with this log event, if any.</param>
		public void WriteEvent(string eventName, Exception exception)
		{
			WriteEvent(eventName, exception, null);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="exception">The exception associated with this log event, if any.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		public void WriteEvent(string eventName, Exception exception, params KeyValuePair<string, object>[] properties)
		{
			WriteEvent(eventName, exception == null ? LogEventSeverity.Information : LogEventSeverity.Error, LogEventType.ApplicationEvent, exception, properties);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		public void WriteEvent(string eventName, LogEventSeverity eventSeverity)
		{
			WriteEvent(eventName, eventSeverity, null);
		}


		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		public void WriteEvent(string eventName, LogEventSeverity eventSeverity, params KeyValuePair<string, object>[] properties)
		{
			WriteEvent(eventName, eventSeverity, LogEventType.ApplicationEvent, null, properties);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="eventType">A <see cref="LogEventType"/> to assign to the written log entry. The defaultvalue if <see cref="LogEventType.ApplicationEvent"/>.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "The previous call to Utils.Any technically does the validation, code analysis just can't figure that out.")]
		public void WriteEvent(string eventName, LogEventSeverity eventSeverity, LogEventType eventType, params KeyValuePair<string, object>[] properties)
		{
			WriteEvent(eventName, eventSeverity, eventType, null, properties);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="eventType">A <see cref="LogEventType"/> to assign to the written log entry. The defaultvalue if <see cref="LogEventType.ApplicationEvent"/>.</param>
		/// <param name="exception">The exception associated with this log event, if any.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "The previous call to Utils.Any technically does the validation, code analysis just can't figure that out.")]
		public void WriteEvent(string eventName, LogEventSeverity eventSeverity, LogEventType eventType, Exception exception, params KeyValuePair<string, object>[] properties)
		{
			WriteEventWithExplicitSource(eventName, eventSeverity, eventType, null, null, -1, properties, exception);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance containing data to write.</param>
		public void WriteEvent(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			try
			{
				if (!IsEnabled || !(_FirstChanceFilter?.ShouldLog(logEvent.EventName, logEvent.EventSeverity, logEvent.EventType, logEvent.Source, logEvent.SourceMethod) ?? true)) return;

				UnsafeWriteEvent(logEvent, null, null, -1);
			}
			catch (LogException lex)
			{
				if (_ErrorHandler.ReportError(lex) == LoggingErrorPolicy.Rethrow)
					throw;
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				var wrappedException = new LogException(ex.Message, ex);
				if (_ErrorHandler.ReportError(wrappedException) == LoggingErrorPolicy.Rethrow)
					throw wrappedException;
			}
		}

		#endregion

		#region WriteEventWithSource Overloads

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="eventType">A <see cref="LogEventType"/> to assign to the written log entry. The defaultvalue if <see cref="LogEventType.ApplicationEvent"/>.</param>
		/// <param name="exception">The exception associated with this log event, if any.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		/// <param name="source">A string containing the source to assign to <see cref="LogEvent.Source"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerFilePathAttribute.</param>
		/// <param name="sourceMethod">A string containing the method name to assign to <see cref="LogEvent.SourceMethod"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerMemberNameAttribute.</param>
		/// <param name="sourceLineNumber">The line number of the source code at which this method was called.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "The previous call to Utils.Any technically does the validation, code analysis just can't figure that out.")]
		public void WriteEventWithSource(string eventName, LogEventSeverity eventSeverity = LogEventSeverity.Information, LogEventType eventType = LogEventType.ApplicationEvent,
			Exception exception = null,
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
			int sourceLineNumber = -1,
			params KeyValuePair<string, object>[] properties)
		{
			WriteEventWithExplicitSource(eventName, eventSeverity, eventType, source, sourceMethod, sourceLineNumber, properties, exception);
		}

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance containing data to write.</param>
		/// <param name="source">A string containing the source to assign to <see cref="LogEvent.Source"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerFilePathAttribute.</param>
		/// <param name="sourceMethod">A string containing the method name to assign to <see cref="LogEvent.SourceMethod"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerMemberNameAttribute.</param>
		/// <param name="sourceLineNumber">The line number of the source code at which this method was called.</param>
		/// <param name="cloneEvent">A boolean indicating if a cloned copy of the <paramref name="logEvent"/> should be used. If the <see cref="LogEvent"/> instance provided is or might be used elsewhere (such as in the case of the <see cref="Writers.ForwardingLogWriter"/> then using a clone prevents concurrency issues.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public void WriteEventWithSource(LogEvent logEvent,
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
			int sourceLineNumber = -1,
			bool cloneEvent = false)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			try
			{
				if (!IsEnabled || !(_FirstChanceFilter?.ShouldLog(logEvent.EventName, logEvent.EventSeverity, logEvent.EventType, logEvent.Source, logEvent.SourceMethod) ?? true)) return;

				LogEvent eventToWrite = logEvent;
				PooledObject<LogEvent> pooledLogEvent = null;
				try
				{
					if (cloneEvent)
					{
						pooledLogEvent = _EntryPool.Take();
						logEvent.Clone(pooledLogEvent.Value);
						eventToWrite = pooledLogEvent.Value;
					}

					if (eventToWrite.DateTime == DateTimeOffset.MinValue)
						eventToWrite.DateTime = _LogClock.Now;

					UnsafeWriteEvent(eventToWrite, source, sourceMethod, sourceLineNumber);
				}
				finally
				{
					if (cloneEvent && pooledLogEvent != null)
						pooledLogEvent.Dispose();
				}
			}
			catch (LogException lex)
			{
				if (_ErrorHandler.ReportError(lex) == LoggingErrorPolicy.Rethrow)
					throw;
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
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
		public void ExecuteLoggedJob(Action job, string jobName, string jobId, params KeyValuePair<string, object>[] properties)
		{
			if (job == null) throw new ArgumentNullException(nameof(jobName));

			var token = BeginLoggedJob(jobName, jobId, TimeSpan.Zero, properties);
			try
			{
				job();
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
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
			return BeginLoggedJob(jobName, jobId, TimeSpan.Zero);
		}

		/// <summary>
		/// Returns a <see cref="LoggedJob"/> that can be used to track a job, including start, completed and failure events, along with duration.
		/// </summary>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <param name="properties">A set of additional properties to include on each event logged in relation to the job.</param>
		/// <param name="maxExpectedDuration">The maximum expected duration of the work. If the work takes longer than this, the job complete event will be marked with a warning severity.</param>
		/// <returns>A <see cref="LoggedJob"/> instance for this specified job.</returns>
		public LoggedJob BeginLoggedJob(string jobName, string jobId, TimeSpan maxExpectedDuration, params KeyValuePair<string, object>[] properties)
		{
			var retVal = _JobPool.Take();
			retVal.Initialize(this, jobName, jobId, properties, _JobPool, maxExpectedDuration);
			return retVal;
		}

		#endregion

		/// <summary>
		/// Sets or returns a boolean indicating if logging is currently enabled.
		/// </summary>
		/// <remarks>
		/// <para>Defaults to true.</para>
		/// <para>If false then no log events are written regardless of other settings and calling the <see cref="WriteEventWithSource(string, LogEventSeverity, LogEventType, Exception, string, string, int, KeyValuePair{string, object}[])"/> overloads returns quickly without doing any work. If true, events are written based on the logger configuration.</para>
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

		/// <summary>
		/// Flushes the inner log writer, causing an immediate write of any pending log events, if possible.
		/// </summary>
		public void Flush()
		{
			(_LogWriter as IFlushable)?.Flush();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Disposes this instance and all internal resources.
		/// </summary>
		/// <param name="isDisposing">True if the class is being explicitly disposed, false if it is being finalised.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_JobPool", Justification = "It is disposed, CA just can't tell.")]
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
		protected virtual void ApplyContext(LogEvent logEvent)
		{
			if (logEvent == null) return;
			if ((_ContextProviders?.Length ?? 0) == 0) return;

			if (logEvent.Properties == null) logEvent.Properties = new Dictionary<string, object>(_ContextProviders.Length);

			for (int cnt = 0; cnt < _ContextProviders.Length; cnt++)
			{
				_ContextProviders[cnt].AddProperties(logEvent, _RendererMap);
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

		private void WriteEventWithExplicitSource(string eventName, LogEventSeverity eventSeverity, LogEventType eventType, string source, string sourceMethod, int sourceLineNumber, KeyValuePair<string, object>[] properties, Exception exception)
		{
			try
			{
				if (!IsEnabled || !(_FirstChanceFilter?.ShouldLog(eventName, eventSeverity, eventType, source, sourceMethod) ?? true)) return;

				using (var pooledLogEvent = _EntryPool.Take())
				{
					var logEvent = pooledLogEvent.Value;
					InitialiseLogEvent(eventName, eventSeverity, eventType, properties, exception, logEvent);

					UnsafeWriteEvent(logEvent, source, sourceMethod, sourceLineNumber);
				}
			}
			catch (LogException lex)
			{
				if (_ErrorHandler.ReportError(lex) == LoggingErrorPolicy.Rethrow)
					throw;
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				var wrappedException = new LogException(ex.Message, ex);
				if (_ErrorHandler.ReportError(wrappedException) == LoggingErrorPolicy.Rethrow)
					throw wrappedException;
			}
		}

		private void InitialiseLogEvent(string eventName, LogEventSeverity eventSeverity, LogEventType eventType, KeyValuePair<string, object>[] properties, Exception exception, LogEvent logEvent)
		{
			logEvent.DateTime = _LogClock.Now;
			logEvent.EventName = eventName ?? String.Empty;
			logEvent.EventSeverity = eventSeverity;
			logEvent.EventType = eventType;
			logEvent.Exception = exception;

			RenderProperties(properties, logEvent);
		}

		private void RenderProperties(KeyValuePair<string, object>[] properties, LogEvent logEvent)
		{
			if (Utils.Any<KeyValuePair<string, object>>(properties))
			{
				IPropertyRenderer renderer = null;
				bool hasRenderers = _RendererMap != null;
				var eventProperties = (logEvent.Properties = logEvent.Properties ?? new Dictionary<string, object>((properties?.Length ?? 1) + (_ContextProviders?.Length ?? 0)));
				foreach (var property in properties)
				{
					renderer = null;
					if (property.Value != null)
					{
						if (hasRenderers)
							renderer = _RendererMap.GetRenderer(property.Value.GetType());
						else if (_DefaultExceptionRenderer != null && property.Value is Exception)
							renderer = _DefaultExceptionRenderer;
					}

					if (renderer != null)
						eventProperties[property.Key] = renderer.RenderValue(property.Value);
					else
						eventProperties[property.Key] = property.Value;
				}
			}
		}

		private void UnsafeWriteEvent(LogEvent logEvent, string source, string sourceMethod, int sourceLineNumber)
		{
			if (String.IsNullOrEmpty(logEvent.Source))
				logEvent.Source = String.IsNullOrEmpty(source) ? _Source : source; 

			if (String.IsNullOrEmpty(logEvent.SourceMethod))
				logEvent.SourceMethod = sourceMethod;

			if (logEvent.SourceLineNumber < 0)
				logEvent.SourceLineNumber = sourceLineNumber;

			ApplyContext(logEvent);

			if (ShouldLogEvent(logEvent))
			{
				if (_LogWriter.RequiresSynchronization)
				{
					lock (_LogWriter)
					{
						_LogWriter.Write(logEvent);
					}
				}
				else
					_LogWriter.Write(logEvent);
			}
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
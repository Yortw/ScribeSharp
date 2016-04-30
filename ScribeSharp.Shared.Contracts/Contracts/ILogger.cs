using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can be used to write log entries. This is the primary interface used by client code.
	/// </summary>
	public interface ILogger
	{
		#region WriteEvent Overloads

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="eventName">The event name or message to write to the log.</param>
		/// <param name="eventSeverity">A <see cref="LogEventSeverity"/> to assign to the written log entry. The default value is <see cref="LogEventSeverity.Information"/>.</param>
		/// <param name="eventType">A <see cref="LogEventType"/> to assign to the written log entry. The defaultvalue if <see cref="LogEventType.ApplicationEvent"/>.</param>
		/// <param name="properties">An enumerable set of <see cref="KeyValuePair{TKey, TValue}"/> instance that contain additional property information to write with the log entry. The key must be a string, the value will be used for the property value.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		void WriteEvent(string eventName,
			LogEventSeverity eventSeverity = LogEventSeverity.Information,
			LogEventType eventType = LogEventType.ApplicationEvent,
			params KeyValuePair<string, object>[] properties);

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance containing data to write.</param>
		void WriteEvent(LogEvent logEvent);

		#endregion

		#region WriteEventWithSource Overloads

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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		void WriteEventWithSource(string eventName, 
			LogEventSeverity eventSeverity = LogEventSeverity.Information, 
			LogEventType eventType = LogEventType.ApplicationEvent,
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
			params KeyValuePair<string, object>[] properties);

		/// <summary>
		/// Writes a <see cref="LogEvent"/> to the appropriate output locations if it meets the configured filter.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance containing data to write.</param>
		/// <param name="source">A string containing the source to assign to <see cref="LogEvent.Source"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerFilePathAttribute.</param>
		/// <param name="sourceMethod">A string containing the method name to assign to <see cref="LogEvent.SourceMethod"/> if it is not already set. If not supplied this parameter will be set by the compiler on systems that support System.Runtime.CompilerServices.CallerMemberNameAttribute.</param>
		/// <param name="sourceLineNumber">The line number of the source code at which this method was called.</param>
		void WriteEventWithSource(LogEvent logEvent,
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
			int sourceLineNumber = -1);

		#endregion

		#region CreateChildLogger Overloads

		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, using the full type name of <typeparam name="T"/> as the source.
		/// </summary>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		ILogger CreateChildLogger<T>();
		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, using the full type name of <typeparam name="T"/> as the source.
		/// </summary>
		/// <param name="contextProviders">A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="ILogWriter"/>.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		ILogger CreateChildLogger<T>(IEnumerable<ILogEventContextProvider> contextProviders);
		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, but with an alternate source.
		/// </summary>
		/// <param name="source">The source to apply to all log event entries. If null, the inner source or system supplied source will be used.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		ILogger CreateChildLogger(string source);
		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, but with an alternate source.
		/// </summary>
		/// <param name="source">The source to apply to all log event entries. If null, the inner source or system supplied source will be used.</param>
		/// <param name="contextProviders">A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="ILogWriter"/>.</param>
		/// <returns>An implementation of <see cref="ILogger"/> with the specified source applied.</returns>
		ILogger CreateChildLogger(string source, IEnumerable<ILogEventContextProvider> contextProviders);
		/// <summary>
		/// Creates a new <see cref="ILogger"/> implementation which wraps this one, applying the supplied policy to log events before forwarding them to this logger for processing.
		/// </summary>
		/// <param name="policy">The policy to apply to the wrapping logger.</param>
		/// <returns>An implementation of <see cref="ILogger"/> that wraps this one.</returns>
		ILogger CreateChildLogger(LogPolicy policy);

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
		void ExecuteLoggedJob(Action job, string jobName, string jobId);

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
		void ExecuteLoggedJob(Action job, string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties);

		#endregion

		#region BeginLoggedJob Overloads

		/// <summary>
		/// Returns a <see cref="LoggedJob"/> that can be used to track a job, including start, completed and failure events, along with duration.
		/// </summary>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <returns>A <see cref="LoggedJob"/> instance for this specified job.</returns>
		LoggedJob BeginLoggedJob(string jobName, string jobId);

		/// <summary>
		/// Returns a <see cref="LoggedJob"/> that can be used to track a job, including start, completed and failure events, along with duration.
		/// </summary>
		/// <param name="jobName">A name/type/descriptio of the job, i.e "Process message".</param>
		/// <param name="jobId">The unique id of the job, used to distinguish it from other jobs of the same type. Optional.</param>
		/// <param name="properties">A set of additional properties to include on each event logged in relation to the job.</param>
		/// <returns>A <see cref="LoggedJob"/> instance for this specified job.</returns>
		LoggedJob BeginLoggedJob(string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties);

		#endregion

		/// <summary>
		/// Sets or returns a boolean indicating if logging is currently enabled.
		/// </summary>
		/// <remarks>
		/// <para>Defaults to true.</para>
		/// <para>If false then no log events are written regardless of other settings and calling the <see cref="WriteEventWithSource(string, LogEventSeverity, LogEventType, string, string, int, KeyValuePair{string, object}[])"/> overloads returns quickly without doing any work. If true, events are written based on the logger configuration.</para>
		/// </remarks>
		bool IsEnabled { get; set; }

	}
}
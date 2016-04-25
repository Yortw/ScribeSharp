using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Specifies the severity or importnce of an event entry.
	/// </summary>
	public enum LogEventSeverity
	{
		/// <summary>
		/// The minimum severity, used to log the most detailed level of event information.
		/// </summary>
		Diagnostic = 0,
		/// <summary>
		/// Specifies detailed event logging.
		/// </summary>
		Verbose,
		/// <summary>
		/// Specifies extra debgging over the normal informative event logs.
		/// </summary>
		Debug,
		/// <summary>
		/// The default level of logging. Indicates an informative event possibly useful for tracing or diagnosis, but one that does not indicate any sort of failure as is not written so often as to be considered <see cref="Verbose"/>.
		/// </summary>
		Information,
		/// <summary>
		/// Indicates something unusual that should be taken note of, often a condition that may or may not be a failure or a transient error state.
		/// </summary>
		Warning,
		/// <summary>
		/// Indicates an error, but not neccesarily an unexpected or fatal error.
		/// </summary>
		Error,
		/// <summary>
		/// Indicates a significant error that should be taken note of and dealt with as soon as possible.
		/// </summary>
		CriticalError,
		/// <summary>
		/// Indicates an error severe enough that the system stopped or restarted.
		/// </summary>
		FatalError
	}
}
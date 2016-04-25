using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filter's an event based on it's <see cref="LogEvent.EventSeverity"/> property.
	/// </summary>
	public sealed class LogSeverityFilter : ILogEventFilter
	{

		private LogEventSeverity _RequiredSeverity;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="requiredSeverity">A value from the <see cref="LogEventSeverity"/> enum to be matched.</param>
		public LogSeverityFilter(LogEventSeverity requiredSeverity)
		{
			_RequiredSeverity = requiredSeverity;
		}

		/// <summary>
		/// Returns true of the <see cref="LogEvent.EventSeverity"/> matches the specified severity.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			return logEvent.EventSeverity == _RequiredSeverity;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filter's an event based on it's <see cref="LogEvent.EventSeverity"/> property.
	/// </summary>
	public sealed class MinimumLogSeverityFilter : ILogEventFilter
	{
		private LogEventSeverity _MinimumSeverity;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="minimumSeverity">A value from the <see cref="LogEventSeverity"/> enum that specifies the lowest severity of event to log.</param>
		public MinimumLogSeverityFilter(LogEventSeverity minimumSeverity)
		{
			_MinimumSeverity = minimumSeverity;
		}

		/// <summary>
		/// Returns true of the <see cref="LogEvent.EventSeverity"/> is greater than or equal to the severity specified.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			return logEvent.EventSeverity >= _MinimumSeverity;
		}
	}
}
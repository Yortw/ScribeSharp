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
		private ILogEventSeveritySwitch _SeveritySwitch;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="severitySwitch">An <see cref="ILogEventSeveritySwitch"/> implementation used to provide and check the minimum allowed event severity.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="severitySwitch"/> is null.</exception>
		public MinimumLogSeverityFilter(ILogEventSeveritySwitch severitySwitch)
		{
			_SeveritySwitch = severitySwitch;
		}

		/// <summary>
		/// Sets or returns the minimum severity allowed to pass the filter.
		/// </summary>
		public LogEventSeverity MinimumSeverity
		{
			get
			{
				return _SeveritySwitch.MinimumSeverity;
			}

			set
			{
				_SeveritySwitch.MinimumSeverity = value;
			}
		}

		/// <summary>
		/// Returns true of the <see cref="LogEvent.EventSeverity"/> is greater than or equal to the severity specified.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			return _SeveritySwitch.IsAllowed(logEvent.EventSeverity);
		}
	}
}
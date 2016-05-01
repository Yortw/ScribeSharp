using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// A simple <see cref="IFirstChanceLogFilter"/> implementation that can filter based on minimum event severity and an optional list of event types.
	/// </summary>
	public class StandardFirstChanceFilter : IFirstChanceLogFilter
	{

		#region Fields

		private LogEventSeverity _MinimumSeverity;
		private LogEventType[] _AllowedEventTypes;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="minimumSeverity">The minimum severity of events to log.</param>
		/// <param name="allowedEventTypes">Null to avoid filtering on event type, or else an enumerable set of event types that will pass the filter.</param>
		public StandardFirstChanceFilter(LogEventSeverity minimumSeverity, IEnumerable<LogEventType> allowedEventTypes)
		{
			_MinimumSeverity = minimumSeverity;
			_AllowedEventTypes = allowedEventTypes?.ToArray();
		}

		/// <summary>
		/// <para>Returns a boolean indicating if an event should be logged based on some basic properties, known early in the logging process.</para>
		/// </summary>
		/// <param name="message">The message text for the event.</param>
		/// <param name="eventSeverity">The <see cref="LogEventSeverity"/> of the event.</param>
		/// <param name="eventType">The <see cref="LogEventType"/> for the event.</param>
		/// <param name="source">The string representation of the source of the event.</param>
		/// <param name="sourceMethod">The string representation of the source method requesting the event be logged.</param>
		/// <returns>A boolean, true if the event should be logged, else false.</returns>
		public bool ShouldLog(string message, LogEventSeverity eventSeverity, LogEventType eventType, string source, string sourceMethod)
		{
			return eventSeverity >= _MinimumSeverity
				&& IsEventTypeAllowed(eventType);
		}

		#endregion
		
		#region Public Properties

		/// <summary>
		/// Sets or returns the minimum severity of event to log.
		/// </summary>
		public LogEventSeverity MinimumSeverity
		{
			get
			{
				return _MinimumSeverity;
			}

			set
			{
				_MinimumSeverity = value;
			}
		}

		#endregion

		#region Private Methods

		private bool IsEventTypeAllowed(LogEventType eventType)
		{
			if (_AllowedEventTypes == null) return true;

			for (int cnt = 0; cnt < _AllowedEventTypes.Length; cnt++)
			{
				if (_AllowedEventTypes[cnt] == eventType) return true;
			}

			return false;
		}

		#endregion

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filters an event based on it's log event type.
	/// </summary>
	public sealed class LogEventTypeFilter : ILogEventFilter
	{
		private LogEventType _EventType;
		private FilterConditionStyle _FilterCondition;

		/// <summary>
		/// Partial constructor. Takes a <see cref="LogEventType"/> value that will be included by this filter.
		/// </summary>
		/// <param name="eventType">A value from the <see cref="LogEventType"/> enum that will pass this filter.</param>
		public LogEventTypeFilter(LogEventType eventType) : this(eventType, FilterConditionStyle.Include)
		{
		}

		/// <summary>
		/// Full constructor. Takes a <see cref="LogEventType"/> value that will be included or excluded by this filter.
		/// </summary>
		/// <param name="eventType">A value from the <see cref="LogEventType"/> checked by this filter.</param>
		/// <param name="filterCondition">A <see cref="FilterConditionStyle"/> value that indicates whether <paramref name="eventType"/> is included or excluded.</param>
		public LogEventTypeFilter(LogEventType eventType, FilterConditionStyle filterCondition) 
		{
			_EventType = eventType;
			_FilterCondition = filterCondition;
		}

		/// <summary>
		/// Returns a boolean based on whether the <seealso cref="LogEvent"/> passes the filter.
		/// </summary>
		/// <param name="logEvent">The <seealso cref="LogEvent"/> instance to examine.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			var retVal = _EventType == logEvent.EventType;
			return _FilterCondition == FilterConditionStyle.Include ? retVal : !retVal;
		}
	}
}
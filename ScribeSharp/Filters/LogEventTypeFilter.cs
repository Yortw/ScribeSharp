using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class LogEventTypeFilter : ILogEventFilter
	{
		private LogEventType[] _AllowedEventTypes;

		public LogEventTypeFilter(IEnumerable<LogEventType> allowedEventTypes)
		{
			if (allowedEventTypes == null) throw new ArgumentNullException(nameof(allowedEventTypes));

			_AllowedEventTypes = allowedEventTypes.ToArray();
		}

		public bool ShouldProcess(LogEvent logEvent)
		{
			for (int cnt = 0; cnt < _AllowedEventTypes.Length; cnt++)
			{
				if (_AllowedEventTypes[cnt] == logEvent.EventType) return true;
			}
			return false;
		}
	}
}
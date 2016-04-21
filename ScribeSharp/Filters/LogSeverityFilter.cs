using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class LogSeverityFilter : ILogEventFilter
	{

		private LogEventSeverity _RequiredSeverity;

		public LogSeverityFilter(LogEventSeverity requiredSeverity)
		{
			_RequiredSeverity = requiredSeverity;
		}

		public bool ShouldProcess(LogEvent logEvent)
		{
			return logEvent.EventSeverity == _RequiredSeverity;
		}
	}
}
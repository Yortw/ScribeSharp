using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class MinimumLogSeverityFilter : ILogEventFilter
	{
		private LogEventSeverity _MinimumSeverity;

		public MinimumLogSeverityFilter(LogEventSeverity minimumSeverity)
		{
			_MinimumSeverity = minimumSeverity;
		}

		public bool ShouldProcess(LogEvent logEvent)
		{
			return logEvent.EventSeverity >= _MinimumSeverity;
		}
	}
}
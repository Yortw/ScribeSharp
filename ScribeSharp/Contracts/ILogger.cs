using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public interface ILogger
	{
		void WriteEvent(string eventName, LogEventSeverity eventSeverity = LogEventSeverity.Information, LogEventType eventType = LogEventType.ApplicationEvent, IEnumerable<KeyValuePair<string, object>> properties = null, string source = "", string sourceMethod = "");

		void WriteEvent(LogEvent logEvent, string source = "", string sourceMethod = "");

		bool IsEnabled { get; set; }
	}
}
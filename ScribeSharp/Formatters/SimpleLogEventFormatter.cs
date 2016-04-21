using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	public sealed class SimpleLogEventFormatter : ILogEventFormatter
	{
		private static SimpleLogEventFormatter s_DefaultInstance;

		public string Format(LogEvent logEvent)
		{
			return "[" + logEvent.DateTime.ToString("G") + "] [" + logEvent.EventSeverity.ToString() + "] [" + logEvent.EventType.ToString() + "] [" + logEvent.Source + "] [" + logEvent.SourceMethod + "] " + logEvent.EventName;
		}

		public static SimpleLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new SimpleLogEventFormatter()); }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.AutoLoggers
{
	public sealed class DateTimeChangedAutoLogger : AutoLoggerBase
	{
		public DateTimeChangedAutoLogger(ILogger logger) : base(logger)
		{
			Microsoft.Win32.SystemEvents.TimeChanged += SystemEvents_TimeChanged;
		}

		private void SystemEvents_TimeChanged(object sender, EventArgs e)
		{
			Logger.WriteEvent($"System date/time changed. New time is {DateTimeOffset.Now.ToString("g")}", LogEventSeverity.Warning, LogEventType.SystemEvent);
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				Microsoft.Win32.SystemEvents.TimeChanged -= this.SystemEvents_TimeChanged;
			}
		}
	}
}
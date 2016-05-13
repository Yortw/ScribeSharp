#if SUPPORTS_SYSTEMEVENTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.AutoLoggers
{
	/// <summary>
	/// Writes an event log entry if the system clock changes.
	/// </summary>
	public sealed class DateTimeChangedAutoLogger : AutoLoggerBase
	{
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance to write to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
		public DateTimeChangedAutoLogger(ILogger logger) : base(logger)
		{
			Microsoft.Win32.SystemEvents.TimeChanged += SystemEvents_TimeChanged;
		}

		/// <summary>
		/// Disconnects this instance from the time changed event.
		/// </summary>
		/// <param name="isDisposing">A boolean indicating if this instance is being explicitly disposed (true) or finalised (false).</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose")]
		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				Microsoft.Win32.SystemEvents.TimeChanged -= this.SystemEvents_TimeChanged;
			}
		}

		private void SystemEvents_TimeChanged(object sender, EventArgs e)
		{
			Logger.WriteEventWithSource(String.Format(System.Globalization.CultureInfo.InvariantCulture, Properties.Resources.DateTimeChangedLogEventText, DateTimeOffset.Now.ToString("G", System.Globalization.CultureInfo.InvariantCulture)), LogEventSeverity.Warning, LogEventType.SystemEvent);
		}
	}
}

#endif
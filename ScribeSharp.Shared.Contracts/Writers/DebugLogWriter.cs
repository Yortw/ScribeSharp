using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes event log entries using <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	public sealed class DebugLogWriter : LogWriterBase
	{
		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		public DebugLogWriter(ILogEventFormatter eventFormatter) 
		{
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns false.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Writes the event to the console.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			System.Diagnostics.Debugger.Log(Convert.ToInt32(logEvent.EventSeverity, System.Globalization.CultureInfo.InvariantCulture), logEvent.EventType.ToString(), _LogEventFormatter.FormatToString(logEvent));
			//System.Diagnostics.Debug.WriteLine(_LogEventFormatter.FormatToString(logEvent), logEvent.EventType.ToString());
		}

		#endregion
	}
}
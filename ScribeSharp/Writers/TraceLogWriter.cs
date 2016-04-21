using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class TraceLogWriter : LogWriterBase
	{

		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		public TraceLogWriter(ILogEventFormatter eventFormatter) : this(eventFormatter, null)
		{
		}

		public TraceLogWriter(ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			switch (logEvent.EventSeverity)
			{
				case LogEventSeverity.Information:
					System.Diagnostics.Trace.TraceInformation(_LogEventFormatter.Format(logEvent));
					break;

				case LogEventSeverity.Error:
					System.Diagnostics.Trace.TraceError(_LogEventFormatter.Format(logEvent));
					break;

				case LogEventSeverity.Warning:
					System.Diagnostics.Trace.TraceWarning(_LogEventFormatter.Format(logEvent));
					break;

				default:
					System.Diagnostics.Trace.TraceWarning(_LogEventFormatter.Format(logEvent), logEvent.EventSeverity.ToString());
					break;
			}
		}

		public override bool RequiresSynchronisation
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
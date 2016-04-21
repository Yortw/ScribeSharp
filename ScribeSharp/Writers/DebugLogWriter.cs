using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class DebugLogWriter : LogWriterBase
	{
		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		public DebugLogWriter(ILogEventFormatter eventFormatter) : this(eventFormatter, null)
		{
		}

		public DebugLogWriter(ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		public override bool RequiresSynchronisation
		{
			get
			{
				return false;
			}
		}

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			System.Diagnostics.Debug.WriteLine(_LogEventFormatter.Format(logEvent), logEvent.EventType.ToString());
		}

		#endregion
	}
}
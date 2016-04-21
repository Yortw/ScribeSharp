using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class TextLogWriter : LogWriterBase
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		public TextLogWriter(System.IO.TextWriter writer, ILogEventFormatter eventFormatter) : this(writer, eventFormatter, null)
		{
		}

		public TextLogWriter(System.IO.TextWriter writer, ILogEventFormatter eventFormatter, ILogEventFilter filter) :base(filter)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			_Writer = writer;
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		public override bool RequiresSynchronisation
		{
			get
			{
				return true;
			}
		}

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			_Writer.Write(_LogEventFormatter.Format(logEvent));
		}

		#endregion

	}
}
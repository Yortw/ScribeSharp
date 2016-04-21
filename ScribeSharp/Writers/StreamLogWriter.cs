using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class StreamLogWriter : LogWriterBase
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		public StreamLogWriter(System.IO.Stream stream, System.Text.Encoding encoding, ILogEventFormatter eventFormatter) : this(stream, encoding, eventFormatter, null)
		{
		}

		public StreamLogWriter(System.IO.Stream stream, System.Text.Encoding encoding, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			_Writer = new System.IO.StreamWriter(stream, encoding ?? System.Text.UTF8Encoding.UTF8);
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
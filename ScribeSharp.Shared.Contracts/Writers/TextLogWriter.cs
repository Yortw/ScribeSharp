using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer used to output log events to a <see cref="System.IO.TextWriter"/>.
	/// </summary>
	public sealed class TextLogWriter : LogWriterBase, IBatchLogWriter
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="writer">A <see cref="System.IO.TextWriter"/> derived class to output log events to.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
		public TextLogWriter(System.IO.TextWriter writer, ILogEventFormatter eventFormatter)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			_Writer = writer;
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns true.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Uses the configured formatter to write the supplied <see cref="LogEvent"/> instance to the stream.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			_LogEventFormatter.FormatToTextWriter(logEvent, _Writer);
			_Writer.Flush();
		}

		#endregion

		#region IBatchLogWriter Members

		/// <summary>
		/// Writes multiple events to the stream and calls flush once when the batch is finished.
		/// </summary>
		/// <param name="logEvents">A enumerable set of <see cref="LogEvent"/> instances to write.</param>
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			if (logEvents == null) return;

			foreach (var logEvent in logEvents)
			{
				_LogEventFormatter.FormatToTextWriter(logEvent, _Writer);
			}
			_Writer.Flush();
		}

		/// <summary>
		/// Writes multiple events to the stream and calls flush once when the batch is finished.
		/// </summary>
		/// <param name="logEvents">An array of <see cref="LogEvent"/> instances to be written.</param>
		/// <param name="length">The number of items (starting from index 0) in the array to write.</param>
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null) return;

			for (int cnt = 0; cnt < length; cnt++)
			{
				_LogEventFormatter.FormatToTextWriter(logEvents[cnt], _Writer);
			}
			_Writer.Flush();
		}

		#endregion


	}
}
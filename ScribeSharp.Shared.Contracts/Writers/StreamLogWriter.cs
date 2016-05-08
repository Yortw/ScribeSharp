using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer used to output log events to a <see cref="System.IO.Stream"/>.
	/// </summary>
	public sealed class StreamLogWriter : LogWriterBase, IDisposable, IBatchLogWriter
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="stream">A <see cref="System.IO.Stream"/> derived class to output log events to.</param>
		/// <param name="encoding">A <see cref="System.Text.Encoding"/> instance used to output the log event data to the stream.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
		public StreamLogWriter(System.IO.Stream stream, System.Text.Encoding encoding, ILogEventFormatter eventFormatter)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			_Writer = new System.IO.StreamWriter(stream, encoding ?? System.Text.UTF8Encoding.UTF8);
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

		#region IDisposable

		/// <summary>
		/// Disposes the stream written to by this object.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Writer", Justification = "It is disposed, CA just doesn't realise it.")]
		public void Dispose()
		{
			_Writer?.Dispose();
		}

		#endregion
	}
}
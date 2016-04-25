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
	public sealed class StreamLogWriter : LogWriterBase, IDisposable
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="stream">A <see cref="System.IO.Stream"/> derived class to output log events to.</param>
		/// <param name="encoding">A <see cref="System.Text.Encoding"/> instance used to output the log event data to the stream.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
		public StreamLogWriter(System.IO.Stream stream, System.Text.Encoding encoding, ILogEventFormatter eventFormatter) : this(stream, encoding, eventFormatter, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="stream">A <see cref="System.IO.Stream"/> derived class to output log events to.</param>
		/// <param name="encoding">A <see cref="System.Text.Encoding"/> instance used to output the log event data to the stream.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
		public StreamLogWriter(System.IO.Stream stream, System.Text.Encoding encoding, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
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
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			_Writer.Write(_LogEventFormatter.Format(logEvent));
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Disposes the stream written to by this object.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Writer", Justification="It is disposed, CA just doesn't realise it.")]
		public void Dispose()
		{
			_Writer?.Dispose();
		}

		#endregion
	}
}
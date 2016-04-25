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
	public sealed class TextLogWriter : LogWriterBase
	{

		#region Fields

		private System.IO.TextWriter _Writer;
		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="writer">A <see cref="System.IO.TextWriter"/> derived class to output log events to.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
		public TextLogWriter(System.IO.TextWriter writer, ILogEventFormatter eventFormatter) : this(writer, eventFormatter, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="writer">A <see cref="System.IO.TextWriter"/> derived class to output log events to.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> instance used to format the event before writing to the stream. If null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
		public TextLogWriter(System.IO.TextWriter writer, ILogEventFormatter eventFormatter, ILogEventFilter filter) :base(filter)
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
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			_Writer.Write(_LogEventFormatter.Format(logEvent));
		}

		#endregion

	}
}
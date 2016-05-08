using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> to a simple human readable string containing the severity, type and message.
	/// </summary>
	public sealed class MinimalLogEventFormatter : LogEventFormatterBase
	{
		private static MinimalLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MinimalLogEventFormatter() : base()
		{
		}

		/// <summary>
		/// Writes the important parts of the log event as a single line containing only the severity, event type and message.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format and output.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			writer.Write("[");
			writer.Write(logEvent.EventSeverity.ToString());
			writer.Write("] ");

			writer.Write("[");
			writer.Write(logEvent.EventType.ToString());
			writer.Write("] ");

			writer.Write(logEvent.EventName);

			writer.Flush();
		}

		/// <summary>
		/// Returns a common instance of the formatter which can be resused to reduce allocations.
		/// </summary>
		public static MinimalLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new MinimalLogEventFormatter()); }
		}
	}
}
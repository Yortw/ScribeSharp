using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> to a simple human readable string containing the date, severity, event, source, source method and message.
	/// </summary>
	public sealed class SimpleLogEventFormatter : LogEventFormatterBase
	{
		private static SimpleLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Writes the important parts of the log event as a single line, with multiple lines if the message contains line breaks or there is an exception.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format and output.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			writer.Write("[");
			writer.Write(logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			writer.Write("] ");

			writer.Write("[");
			writer.Write(logEvent.EventSeverity.ToString());
			writer.Write("] ");

			writer.Write("[");
			writer.Write(logEvent.EventType.ToString());
			writer.Write("] ");

			writer.Write("[");
			writer.Write(logEvent.Source);
			writer.Write("] ");

			writer.Write("[");
			writer.Write(logEvent.SourceMethod);
			writer.Write("] ");

			writer.WriteLine(logEvent.EventName);

			if (logEvent.Exception != null)
				writer.WriteLine(logEvent.Exception.ToString());
		}

		/// <summary>
		/// Returns a common instance of the formatter which can be resused to reduce allocations.
		/// </summary>
		public static SimpleLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new SimpleLogEventFormatter()); }
		}
	}
}
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
	public sealed class FullTextLogEventFormatter : LogEventFormatterBase
	{
		private static FullTextLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Formats and outputs the log event to the specified writer as a series of human readable lines of text, with a blank line at the end.
		/// </summary>
		/// <param name="logEvent">The log event to format and output.</param>
		/// <param name="writer">A <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			WriteLine(writer, "Date: ", logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			WriteLine(writer, "Event Name: ", logEvent.EventName);
			WriteLine(writer, "Severity: ",logEvent.EventSeverity.ToString());
			WriteLine(writer, "Event Type: ", logEvent.EventType.ToString());
			WriteLine(writer, "Source: ", logEvent.Source);
			WriteLine(writer, "Source Method: ", logEvent.SourceMethod);
			if (logEvent.SourceLineNumber >= 0)
				WriteLine(writer, "Source Line: ", logEvent.SourceLineNumber.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (logEvent.Exception != null)
			{
				writer.WriteLine("Exception:");
				writer.WriteLine(logEvent.Exception.ToString());
			}

			var properties = logEvent.Properties;
			if (properties != null)
			{
				writer.WriteLine("Properties:");
				foreach (var property in properties)
				{
					writer.Write(property.Key);
					writer.Write(": ");
					writer.Write(property.Value);
				}
			}
		}

		private void WriteLine(TextWriter writer, string fieldCaption, string value)
		{
			writer.WriteLine(fieldCaption, value);
		}

		/// <summary>
		/// Returns a common instance of the formatter which can be resused to reduce allocations.
		/// </summary>
		public static FullTextLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new FullTextLogEventFormatter()); }
		}
	}
}
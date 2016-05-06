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

			writer.WriteLine("Date: " + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteLine("Event Name: " + logEvent.EventName);
			writer.WriteLine("Severity: " + logEvent.EventSeverity.ToString());
			writer.WriteLine("Event Type: " + logEvent.EventType.ToString());
			writer.WriteLine("Source: " + logEvent.Source);
			writer.WriteLine("SourceMethod: " + logEvent.SourceMethod);

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
					writer.WriteLine(property.Key + ": " + property.Value);
				}
			}
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
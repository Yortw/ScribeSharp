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
		/// Default constructor.
		/// </summary>
		public SimpleLogEventFormatter() : base()
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="exceptionRenderers">An <see cref="ITypeRendererMap"/> implementation used to locate renders used to format exceptions written to the log.</param>
		public SimpleLogEventFormatter(ITypeRendererMap exceptionRenderers) : base(exceptionRenderers)
		{
		}

		/// <summary>
		/// Writes the important parts of the log event as a single line, with multiple lines if the message contains line breaks or there is an exception.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format and output.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			WriteBracketedValue(writer, logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			WriteBracketedValue(writer, logEvent.EventSeverity.ToString());
			WriteBracketedValue(writer, logEvent.EventType.ToString());
			WriteBracketedValue(writer, logEvent.Source);
			WriteBracketedValue(writer, logEvent.SourceMethod);

			writer.WriteLine(logEvent.EventName);

			if (logEvent.Exception != null)
				writer.WriteLine(RenderException(logEvent.Exception));

			writer.Flush();
		}

		/// <summary>
		/// Returns a common instance of the formatter which can be resused to reduce allocations.
		/// </summary>
		public static SimpleLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new SimpleLogEventFormatter()); }
		}

		private static void WriteBracketedValue(TextWriter writer, string value)
		{
			writer.Write("[");
			writer.Write(value);
			writer.Write("] ");
		}
	}
}
using ScribeSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> instance to a json object.
	/// </summary>
	public sealed class JsonLogEventFormatter : LogEventFormatterBase
	{
		private static JsonLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public JsonLogEventFormatter() : base()
		{
		}

		/// <summary>
		/// Writes the full log event as a Json object.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format and output.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			if (writer == null) return;

			using (var jsonWriter = new JsonWriter(writer, false))
			{
				jsonWriter.WriteJsonObject<LogEvent>(logEvent);
				jsonWriter.Flush();
			}
		}

		/// <summary>
		/// Returns a common instance of the formatter which can be resused to reduce allocations.
		/// </summary>
		public static JsonLogEventFormatter DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new JsonLogEventFormatter()); }
		}

	}
}
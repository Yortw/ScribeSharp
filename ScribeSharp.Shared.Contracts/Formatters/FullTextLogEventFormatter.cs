using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> to a simple human readable string containing the date, severity, event, source, source method and message.
	/// </summary>
	public sealed class FullTextLogEventFormatter : ILogEventFormatter
	{
		private static FullTextLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Formats the <paramref name="logEvent"/> instance to a simple human readable string.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing a human readable string of the primary log event properties.</returns>
		public string Format(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			var sb = new StringBuilder();
			sb.AppendLine("Date: " + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			sb.AppendLine("Event Name: " + logEvent.EventName);
			sb.AppendLine("Severity: " + logEvent.EventSeverity.ToString());
			sb.AppendLine("Event Type: " + logEvent.EventType.ToString());
			sb.AppendLine("Source: " + logEvent.Source);
			sb.AppendLine("SourceMethod: " + logEvent.SourceMethod);

			if (logEvent.Exception != null)
			{
				sb.AppendLine("Exception:");
				sb.AppendLine(logEvent.Exception.ToString());
			}

			var properties = logEvent.Properties;
			if (properties != null)
			{
				sb.AppendLine("Properties:");
				foreach (var property in properties)
				{
					sb.AppendLine(property.Key + ": " + property.Value);
				}
			}

			return sb.ToString();
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
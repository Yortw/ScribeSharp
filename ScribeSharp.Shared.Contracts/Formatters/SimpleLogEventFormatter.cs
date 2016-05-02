using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> to a simple human readable string containing the date, severity, event, source, source method and message.
	/// </summary>
	public sealed class SimpleLogEventFormatter : ILogEventFormatter
	{
		private static SimpleLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Formats the <paramref name="logEvent"/> instance to a simple human readable string.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing a human readable string of the primary log event properties.</returns>
		public string Format(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			var retVal = "[" + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "] [" + logEvent.EventSeverity.ToString() + "] [" + logEvent.EventType.ToString() + "] [" + logEvent.Source + "] [" + logEvent.SourceMethod + "] " + logEvent.EventName;
			if (logEvent.Exception != null)
				retVal += Environment.NewLine + logEvent.Exception.ToString();

			return retVal;
			//var sb = new System.Text.StringBuilder();
			//sb.Append("[");
			//sb.Append(logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
			//sb.Append("] ");

			//sb.Append("[");
			//sb.Append(logEvent.EventSeverity.ToString());
			//sb.Append("] ");

			//sb.Append("[");
			//sb.Append(logEvent.EventType.ToString());
			//sb.Append("] ");

			//if (!String.IsNullOrEmpty(logEvent.Source))
			//{
			//	sb.Append("[");
			//	sb.Append(logEvent.Source);
			//	sb.Append("] ");
			//}

			//if (!String.IsNullOrEmpty(logEvent.SourceMethod))
			//{
			//	sb.Append("[");
			//	sb.Append(logEvent.SourceMethod);
			//	sb.Append("] ");
			//}

			//sb.AppendLine(logEvent.EventName);

			//if (logEvent.Exception != null)
			//	sb.AppendLine(logEvent.Exception.ToString());

			//return sb.ToString();
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
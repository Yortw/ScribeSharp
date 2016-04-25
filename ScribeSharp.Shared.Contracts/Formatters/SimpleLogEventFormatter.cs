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

			return "[" + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "] [" + logEvent.EventSeverity.ToString() + "] [" + logEvent.EventType.ToString() + "] [" + logEvent.Source + "] [" + logEvent.SourceMethod + "] " + logEvent.EventName;
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
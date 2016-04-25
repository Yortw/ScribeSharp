using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> to a simple human readable string containing the severity, type and message.
	/// </summary>
	public sealed class MinimalLogEventFormatter : ILogEventFormatter
	{
		private static MinimalLogEventFormatter s_DefaultInstance;

		/// <summary>
		/// Formats the <paramref name="logEvent"/> instance to a simple human readable string.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing a human readable string of the primary log event properties.</returns>
		public string Format(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			return "[" + logEvent.EventSeverity.ToString() + "] [" + logEvent.EventType.ToString() + "] " + logEvent.EventName;
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
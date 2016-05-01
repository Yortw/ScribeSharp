using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// <para>A special log filter used as soon as possible in the logging pipeline, often even before a <see cref="LogEvent"/> instance is allocated.</para>
	/// <para>Because the filter is applied so early it can only operate on a limited set of properties, but filtering log events early can improve performance in some situatiions.</para>
	/// </summary>
	public interface IFirstChanceLogFilter
	{
		/// <summary>
		/// <para>Returns a boolean indicating if an event should be logged based on some basic properties, known early in the logging process.</para>
		/// </summary>
		/// <param name="message">The message text for the event.</param>
		/// <param name="eventSeverity">The <see cref="LogEventSeverity"/> of the event.</param>
		/// <param name="eventType">The <see cref="LogEventType"/> for the event.</param>
		/// <param name="source">The string representation of the source of the event.</param>
		/// <param name="sourceMethod">The string representation of the source method requesting the event be logged.</param>
		/// <returns>A boolean, true if the event should be logged, else false.</returns>
		bool ShouldLog(string message, LogEventSeverity eventSeverity, LogEventType eventType, string source, string sourceMethod);
	}
}
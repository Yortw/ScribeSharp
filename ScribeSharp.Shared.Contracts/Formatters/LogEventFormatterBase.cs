using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Base class for <see cref="ILogEventFormatter"/> implementations, providing common logic.
	/// </summary>
	public abstract class LogEventFormatterBase : ILogEventFormatter
	{
		/// <summary>
		/// Returns the formatted version of the log event as a string.
		/// </summary>
		/// <remarks>
		/// <para>Prefer the <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> method where possible as it is usually more efficient.</para>
		/// <para>The base implementation of this method using a pooled, recycling text writer and the <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> to format the event, so only overriding <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> is required and the method is still reasonable efficient.</para>
		/// </remarks>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing the formatted event text.</returns>
		public virtual string FormatToString(LogEvent logEvent)
		{
			using (var pooledWriter = Globals.TextWriterPool.Take())
			{
				FormatToTextWriter(logEvent, pooledWriter.Value);
				return pooledWriter.Value.GetText();
			}
		}

		/// <summary>
		/// Overridden by derived classes to actually format the log event and write the formatted output to the <paramref name="writer"/> provided.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public abstract void FormatToTextWriter(LogEvent logEvent, TextWriter writer);
	}
}
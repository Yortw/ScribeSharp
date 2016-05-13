#if SUPPORTS_CONSOLE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes log events to the <see cref="System.Console.Out"/> stream.
	/// </summary>
	public sealed class ConsoleLogWriter : LogWriterBase
	{

		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="eventFormatter"/> is null then a simple human readable version of the string is output.</para>
		/// </remarks>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		public ConsoleLogWriter(ILogEventFormatter eventFormatter) 
		{
			_LogEventFormatter = eventFormatter;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns true to ensure the <see cref="System.Console.Out"/> stream doesn't get corrupted by interleaved event information.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Writes the event to the console.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			var writer = logEvent.EventSeverity >= LogEventSeverity.Error ? Console.Error : Console.Out;
			if (_LogEventFormatter != null)
			{
				Console.ForegroundColor = GetConsoleColor(logEvent.EventSeverity);
				_LogEventFormatter.FormatToTextWriter(logEvent, writer);
				Console.ResetColor();
			}
			else
			{
				writer.Write("[" + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + "] ");
				Console.ForegroundColor = GetConsoleColor(logEvent.EventSeverity);
				writer.Write("[" + logEvent.EventSeverity.ToString() + "] ");
				Console.ResetColor();
				writer.Write("[" + logEvent.EventType.ToString() + "] ");
				writer.Write("[" + logEvent.Source + "] ");
				writer.Write("[" + logEvent.SourceMethod + "] ");
				writer.WriteLine(logEvent.EventName);
				if (logEvent.Exception != null)
				{
					writer.Write(logEvent.Exception.ToString());
				}
			}
		}

		#endregion

		#region Private Methods

		private static ConsoleColor GetConsoleColor(LogEventSeverity eventSeverity)
		{
			switch (eventSeverity)
			{
				case LogEventSeverity.CriticalError:
				case LogEventSeverity.FatalError:
					return ConsoleColor.DarkRed;

				case LogEventSeverity.Error:
					return ConsoleColor.Red;

				case LogEventSeverity.Debug:
				case LogEventSeverity.Diagnostic:
					return ConsoleColor.Magenta;

				case LogEventSeverity.Verbose:
					return ConsoleColor.DarkCyan;

				case LogEventSeverity.Warning:
					return ConsoleColor.Yellow;

				default:
					return ConsoleColor.Green;
			}
		}

		#endregion

	}
}

#endif
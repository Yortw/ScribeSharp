using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class ConsoleLogWriter : LogWriterBase
	{

		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		public ConsoleLogWriter(ILogEventFormatter eventFormatter) : this(eventFormatter, null) 
		{
		}

		public ConsoleLogWriter(ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			_LogEventFormatter = eventFormatter;
		}

		#endregion

		#region Overrides

		public override bool RequiresSynchronisation
		{
			get
			{
				return true;
			}
		}

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			var writer = logEvent.EventSeverity >= LogEventSeverity.Error ? Console.Error : Console.Out;
			if (_LogEventFormatter != null)
			{
				Console.ForegroundColor = GetConsoleColor(logEvent.EventSeverity);
				writer.Write(_LogEventFormatter.Format(logEvent) + Environment.NewLine);
				Console.ResetColor();
			}
			else
			{
				writer.Write("[" + logEvent.DateTime.ToString("G") + "] ");
				Console.ForegroundColor = GetConsoleColor(logEvent.EventSeverity);
				writer.Write("[" + logEvent.EventSeverity.ToString() + "] ");
				Console.ResetColor();
				writer.Write("[" + logEvent.EventType.ToString() + "] ");
				writer.Write("[" + logEvent.Source + "] ");
				writer.Write("[" + logEvent.SourceMethod + "] ");
				writer.Write(logEvent.EventName);
				writer.Write(Environment.NewLine);
			}
		}

		#endregion

		#region Private Methods

		private ConsoleColor GetConsoleColor(LogEventSeverity eventSeverity)
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
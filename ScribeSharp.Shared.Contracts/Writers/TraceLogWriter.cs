#if SUPPORTS_TRACE

using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes event log entries using <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	public sealed class TraceLogWriter : LogWriterBase
	{

#region Fields

		private ILogEventFormatter _LogEventFormatter;

#endregion

#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		public TraceLogWriter(ILogEventFormatter eventFormatter) 
		{
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Writes the supplied <see cref="LogEvent"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <remarks>
		/// <para>The <see cref="LogEvent.EventSeverity"/> is used to decide which <see cref="System.Diagnostics.Trace"/> method is called to log the event.</para>
		/// </remarks>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			var formattedMessage = _LogEventFormatter.FormatToString(logEvent) ?? String.Empty;
			if (formattedMessage.Contains("{") || formattedMessage.Contains("}"))
				formattedMessage = formattedMessage.Replace("{", "{{").Replace("}", "}}");

			switch (logEvent.EventSeverity)
			{
				case LogEventSeverity.Information:
					System.Diagnostics.Trace.TraceInformation(formattedMessage);
					break;

				case LogEventSeverity.Error:
					System.Diagnostics.Trace.TraceError(formattedMessage);
					break;

				case LogEventSeverity.Warning:
					System.Diagnostics.Trace.TraceWarning(formattedMessage);
					break;

				default:
					System.Diagnostics.Trace.Write(formattedMessage, logEvent.EventSeverity.ToString());
					break;
			}
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

#endregion
	}
}

#endif
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
		/// Partial constructor.
		/// </summary>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		public TraceLogWriter(ILogEventFormatter eventFormatter) : this(eventFormatter, null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="eventFormatter"/> is null then <see cref="SimpleLogEventFormatter.DefaultInstance"/> is used.</para>
		/// </remarks>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		public TraceLogWriter(ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
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
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			switch (logEvent.EventSeverity)
			{
				case LogEventSeverity.Information:
					System.Diagnostics.Trace.TraceInformation(_LogEventFormatter.Format(logEvent));
					break;

				case LogEventSeverity.Error:
					System.Diagnostics.Trace.TraceError(_LogEventFormatter.Format(logEvent));
					break;

				case LogEventSeverity.Warning:
					System.Diagnostics.Trace.TraceWarning(_LogEventFormatter.Format(logEvent));
					break;

				default:
					System.Diagnostics.Trace.TraceWarning(_LogEventFormatter.Format(logEvent), logEvent.EventSeverity.ToString());
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
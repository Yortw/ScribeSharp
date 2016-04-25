using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes event log entries using <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	public sealed class DebugLogWriter : LogWriterBase
	{
		#region Fields

		private ILogEventFormatter _LogEventFormatter;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		public DebugLogWriter(ILogEventFormatter eventFormatter) : this(eventFormatter, null)
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
		public DebugLogWriter(ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			_LogEventFormatter = eventFormatter ?? SimpleLogEventFormatter.DefaultInstance;
		}

		#endregion

		#region Overrides

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

		/// <summary>
		/// Writes the event to the console.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			System.Diagnostics.Debug.WriteLine(_LogEventFormatter.Format(logEvent), logEvent.EventType.ToString());
		}

		#endregion
	}
}
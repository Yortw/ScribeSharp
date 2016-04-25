using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes <see cref="LogEvent"/> instances to the Windows Event log.
	/// </summary>
	public sealed class WindowsEventLogWriter : LogWriterBase, IDisposable
	{

		#region Fields

		private ILogEventFormatter _EventFormatter;

		private System.Diagnostics.EventLog _Log;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new event source and log if required and configures the writer to output events there.
		/// </summary>
		/// <remarks>
		/// <para>Warning: It's often a better idea to pre-create the event log and use the <see cref="WindowsEventLogWriter(System.Diagnostics.EventLog, ILogEventFormatter, ILogEventFilter)"/> constructor to avoid issues.
		/// Creating new event sources and logs requires administrative priviledges on Windows.
		/// Newly created event logs are not always available immediately, and output to them can either fail or be redirected to the application event log (often until a reboot).
		/// </para>
		/// </remarks>
		/// <param name="eventLogName">The name of the Window Event log to write to.</param>
		/// <param name="eventSourceName">The name of the Windows Event Source associated with events output from this writer.</param>
		/// <param name="eventLogMachineName">The name of the machine hosting the event log to output to. If blank or null the local machine is used.</param>
		/// <param name="createEventLog">If true the event log will be created if it doesn't exist, otherwise an exception will be thrown.</param>
		/// <param name="overflowAction">Specifies the overflow policy for the Windows Event Log policy if the event log is to be created.</param>
		/// <param name="retentionDays">Specifies the number of days to keep events for before overwriting them, used if the event log is to be created.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		public WindowsEventLogWriter(string eventLogName, string eventSourceName, string eventLogMachineName, bool createEventLog, System.Diagnostics.OverflowAction overflowAction, int retentionDays, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			eventLogName = eventLogName ?? "Application";
			if (String.IsNullOrWhiteSpace(eventLogName)) throw new ArgumentException(nameof(eventLogName) + " cannot be empty or whitespace.", nameof(eventLogName));
			if (eventSourceName == null) throw new ArgumentNullException(nameof(eventSourceName));
			if (String.IsNullOrWhiteSpace(eventSourceName)) throw new ArgumentException(nameof(eventSourceName) + " cannot be empty or whitespace.", nameof(eventSourceName));

			_EventFormatter = eventFormatter;

			if (createEventLog)
			{
				if (String.IsNullOrWhiteSpace(eventLogMachineName)) eventLogMachineName = ".";
				if (!System.Diagnostics.EventLog.SourceExists(eventSourceName, eventLogMachineName))
				{
					var creationData = new System.Diagnostics.EventSourceCreationData(eventSourceName, eventLogName);
					creationData.MachineName = eventLogMachineName;
					System.Diagnostics.EventLog.CreateEventSource(creationData);

					var log = new System.Diagnostics.EventLog(eventLogName, eventLogMachineName);
					log.ModifyOverflowPolicy(overflowAction, retentionDays);
				}
			}

			_Log = new System.Diagnostics.EventLog(eventLogName, eventLogMachineName, eventSourceName);
		}

		/// <summary>
		/// Sets the windows event log to write to using a <see cref="System.Diagnostics.EventLog"/> instance.
		/// </summary>
		/// <param name="eventLog">The Windows event log to write to.</param>
		/// <param name="eventFormatter">A <see cref="ILogEventFormatter"/> implementation used to format the event log entries to text for outputting.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		public WindowsEventLogWriter(System.Diagnostics.EventLog eventLog, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			if (eventLog == null) throw new ArgumentNullException(nameof(eventLog));

			_EventFormatter = eventFormatter;
			_Log = eventLog;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Uses the configured formatter to write the supplied <see cref="LogEvent"/> instance to the stream.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.EventLog.WriteEntry(System.String,System.Diagnostics.EventLogEntryType,System.Int32,System.Int16,System.Byte[])", Justification="The interface is inherited from another interface we don't control, the interface member does map to a properly implemented dispose pattern anyway.")]
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			string message = null;
			if (_EventFormatter != null)
				message = _EventFormatter.Format(logEvent);
			else
				message = logEvent.EventName;

			if (message != null && message.Length > 32000)
			{
				int startIndex = 0;
				int partCnt = 1;
				while (startIndex < message.Length)
				{ 
					int partSize = Math.Min(32000, message.Length - startIndex);
					_Log.WriteEntry(String.Format(System.Globalization.CultureInfo.InvariantCulture, Properties.Resources.SplitEventEntryPrefix, partCnt.ToString(System.Globalization.CultureInfo.InvariantCulture)) + Environment.NewLine +  message.Substring(startIndex, partSize), LogEventToWindowsEventType(logEvent), logEvent.GetPropertyOrDefault("EventId", 0), logEvent.GetPropertyOrDefault<short>("CategoryId", LogEventToWindowsEventCategory(logEvent)), logEvent.GetPropertyOrDefault<byte[]>("RawData", null));
					startIndex += partSize;
					partCnt++;
				}
			}
			else
				_Log.WriteEntry(message, LogEventToWindowsEventType(logEvent), logEvent.GetPropertyOrDefault("EventId", 0), logEvent.GetPropertyOrDefault<short>("CategoryId", LogEventToWindowsEventCategory(logEvent)), logEvent.GetPropertyOrDefault<byte[]>("RawData", null));
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

		#region Private Methods

		private static short LogEventToWindowsEventCategory(LogEvent logEvent)
		{
			return (short)logEvent.EventType;
		}

		private static System.Diagnostics.EventLogEntryType LogEventToWindowsEventType(LogEvent logEvent)
		{
			switch (logEvent.EventSeverity)
			{
				case LogEventSeverity.CriticalError:
				case LogEventSeverity.FatalError:
				case LogEventSeverity.Error:
					return System.Diagnostics.EventLogEntryType.Error;

				case LogEventSeverity.Warning:
					return System.Diagnostics.EventLogEntryType.Warning;

				default:
					return System.Diagnostics.EventLogEntryType.Information;
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Disposes the internal <see cref="System.Diagnostics.EventLog"/> instance used by this class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Log", Justification = "It is disposed, CA just doesn't realise it.")]
		public void Dispose()
		{
			_Log?.Dispose();
		}

		#endregion

	}
}
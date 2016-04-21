using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class WindowsEventLogWriter : LogWriterBase
	{

		#region Fields

		private string _EventLogName;
		private string _EventSourceName;

		private ILogEventFormatter _EventFormatter;

		private System.Diagnostics.EventLog _Log;

		#endregion

		#region Constructors

		public WindowsEventLogWriter(string eventLogName, string eventSourceName, string eventLogMachineName, bool createEventLog, System.Diagnostics.OverflowAction overflowAction, int retentionDays, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			eventLogName = eventLogName ?? "Application";
			if (String.IsNullOrWhiteSpace(eventLogName)) throw new ArgumentException(nameof(eventLogName) + " cannot be empty or whitespace.", nameof(eventLogName));
			if (eventSourceName == null) throw new ArgumentNullException(nameof(eventSourceName));
			if (String.IsNullOrWhiteSpace(eventSourceName)) throw new ArgumentException(nameof(eventSourceName) + " cannot be empty or whitespace.", nameof(eventSourceName));

			_EventFormatter = eventFormatter;
			_EventLogName = eventLogName;
			_EventSourceName = eventSourceName;

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

		public WindowsEventLogWriter(System.Diagnostics.EventLog eventLog, ILogEventFormatter eventFormatter, ILogEventFilter filter) : base(filter)
		{
			if (eventLog == null) throw new ArgumentNullException(nameof(eventLog));

			_EventFormatter = eventFormatter;
			_Log = eventLog;
		}

		#endregion

		#region Overrides

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
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
					_Log.WriteEntry($"Split Event, Part {partCnt.ToString()}" + Environment.NewLine +  message.Substring(startIndex, partSize), LogEventToWindowsEventType(logEvent), logEvent.GetPropertyOrDefault("EventId", 0), logEvent.GetPropertyOrDefault<short>("CategoryId", LogEventToWindowsEventCategory(logEvent)), logEvent.GetPropertyOrDefault<byte[]>("RawData", null));
					startIndex += partSize;
					partCnt++;
				}
			}
			else
				_Log.WriteEntry(message, LogEventToWindowsEventType(logEvent), logEvent.GetPropertyOrDefault("EventId", 0), logEvent.GetPropertyOrDefault<short>("CategoryId", LogEventToWindowsEventCategory(logEvent)), logEvent.GetPropertyOrDefault<byte[]>("RawData", null));
		}

		public override bool RequiresSynchronisation
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Private Methods

		private short LogEventToWindowsEventCategory(LogEvent logEvent)
		{
			return (short)logEvent.EventType;
		}

		private System.Diagnostics.EventLogEntryType LogEventToWindowsEventType(LogEvent logEvent)
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

	}
}
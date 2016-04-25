using ScribeSharp.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public class Logger : ILogger
	{
		//TODO: Logger decorator for adding extra context?
		//TODO: Log timed operations?
		//TODO: Log operation stacks? recursive?
		//TODO: Call contexts? Async/thread/logical?
		//TODO: Property renderers?
		//TODO: Internal error handling
		//TODO: Json serialiser
		//TODO: Pool background thread
		//TODO: Buffered logger background thread

		#region Fields

		private readonly ILogEventPool _EntryPool;
		private readonly ILogClock _LogClock;
		private readonly ILogEventFilter _Filter;
		private readonly ILogWriter _LogWriter;
		private readonly ILogEventContextProvider[] _ContextProviders;

		private static readonly IDictionary<string, object> EmptyProperties = new Dictionary<string, object>(0);

		private bool _IsEnabled;

		#endregion

		#region Constructors

		// ILogWriter logWriter, IEnumerable<ILogEventContextProvider> contextProviders, ILogClock logClock
		public Logger(LogPolicy policy)
		{
			if (policy == null) throw new ArgumentNullException(nameof(policy));
			if (policy.LogWriter == null) throw new ArgumentException("policy.LogWriter cannot be null", nameof(policy));

			_EntryPool = new LogEventPool();
			_LogWriter = policy.LogWriter;
			_LogClock = policy.Clock;
			_Filter = policy.Filter;

			if (policy.ContextProviders != null)
				_ContextProviders = (policy.ContextProviders as ILogEventContextProvider[]) ?? policy.ContextProviders.ToArray();

			_IsEnabled = true;
		}

		#endregion

		#region ILogger

		public void WriteEvent(string eventName, LogEventSeverity eventSeverity = LogEventSeverity.Information, LogEventType eventType = LogEventType.ApplicationEvent, IEnumerable<KeyValuePair<string, object>> properties = null, string source = "", string sourceMethod = "")
		{
			if (!IsEnabled) return;

			using (var pooledLogEvent = _EntryPool.Take())
			{
				var logEvent = pooledLogEvent.Value;
				logEvent.DateTime = _LogClock?.Now ?? DateTimeOffset.Now;
				logEvent.EventName = eventName ?? String.Empty;
				logEvent.EventSeverity = eventSeverity;
				logEvent.EventType = eventType;

				if (Utils.Any<KeyValuePair<string, object>>(properties))
				{
					logEvent.Properties = new Dictionary<string, object>();
					foreach (var property in properties)
					{
						logEvent.Properties[property.Key] = RenderProperty(property.Value);
					}
				}

				WriteEvent(logEvent, source, sourceMethod);
			}
		}

		public void WriteEvent(LogEvent logEvent, string source = "", string sourceMethod = "")
		{
			if (!IsEnabled) return;

			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			if (logEvent.DateTime == DateTimeOffset.MinValue) 
				logEvent.DateTime = _LogClock?.Now ?? DateTimeOffset.Now;

			if (String.IsNullOrWhiteSpace(logEvent.Source))
				logEvent.Source = source;
			if (String.IsNullOrWhiteSpace(logEvent.SourceMethod))
				logEvent.SourceMethod = sourceMethod;

			FillProperties(logEvent);

			if (logEvent.Properties == null)
				logEvent.Properties = EmptyProperties;

			if (_Filter?.ShouldProcess(logEvent) ?? true)
			{
				if (_LogWriter.RequiresSynchronisation)
				{
					lock (this)
					{
						UnsynchronisedWrite(logEvent);
					}
				}
				else
					UnsynchronisedWrite(logEvent);
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}

			set
			{
				_IsEnabled = value;
			}
		}

		#endregion

		#region Private Methods

		private void FillProperties(LogEvent entry)
		{
			if ((_ContextProviders?.Length ?? 0) == 0) return;

			if (entry.Properties == null) entry.Properties = new Dictionary<string, object>(_ContextProviders.Length);

			var properties = entry.Properties;
			for (int cnt = 0; cnt < _ContextProviders.Length; cnt++)
			{
				var provider = _ContextProviders[cnt];
				if (!properties.ContainsKey(provider.PropertyName) && (provider?.Filter?.ShouldProcess(entry) ?? true))
					properties[provider.PropertyName] = RenderProperty(provider.GetValue());
			}
		}

		private object RenderProperty(object value)
		{
			return value;
		}

		private void UnsynchronisedWrite(LogEvent LogEvent)
		{
			_LogWriter.Write(LogEvent);
		}

		#endregion

	}
}
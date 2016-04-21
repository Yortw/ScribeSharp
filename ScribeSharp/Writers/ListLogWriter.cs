using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class ListLogWriter : LogWriterBase
	{

		#region Fields

		private IList<LogEvent> _Events;
		private int _MaximumCapacity;

		private IPool<LogEvent> _Pool;

		#endregion

		#region Constructors

		public ListLogWriter(IList<LogEvent> events, int maximumCapacity) : this(events, maximumCapacity, null)
		{
		}

		public ListLogWriter(IList<LogEvent> events, int maximumCapacity, ILogEventFilter filter) : base(filter)
		{
			if (maximumCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(maximumCapacity), "maximumCapacity must be greater than zero.");

			_MaximumCapacity = maximumCapacity;
			_Events = events ?? new List<LogEvent>(maximumCapacity);
			_Pool = new Pool<LogEvent>(new PoolPolicy<LogEvent>()
			{
				Factory = (pool) => new LogEvent(),
				InitializationPolicy = PooledItemInitialization.Return,
				MaximumPoolSize = (uint)maximumCapacity,
				ReinitializeObject = (entry) =>
				{
					entry.DateTime = DateTime.Now;
					entry.EventName = null;
					entry.EventSeverity = LogEventSeverity.Information;
					entry.EventType = LogEventType.ApplicationEvent;
					entry.Properties.Clear();
					entry.Source = null;
					entry.SourceMethod = null;
				}
			});
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
			if (_Events.Count >= _MaximumCapacity)
			{
				var item = _Events[0];
				_Events.RemoveAt(0);
				_Pool.Return(item);
			}

			var newEntry = _Pool.Take();
			logEvent.Clone(newEntry);
			_Events.Add(newEntry);
		}

		#endregion

	}
}
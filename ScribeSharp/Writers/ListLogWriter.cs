using PoolSharp;
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
			if (events == null) throw new ArgumentNullException(nameof(events));
			if (maximumCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(maximumCapacity), "maximumCapacity must be greater than zero.");

			_MaximumCapacity = maximumCapacity;
			_Events = events ?? new List<LogEvent>(maximumCapacity);
			_Pool = new Pool<LogEvent>(new PoolPolicy<LogEvent>()
			{
				Factory = (pool) => new LogEvent(),
				InitializationPolicy = PooledItemInitialization.Return,
				MaximumPoolSize = maximumCapacity,
				ReinitializeObject = (entry) =>
				{
					entry.Clear();
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
				_Pool.Add(item);
			}

			var newEntry = _Pool.Take();
			logEvent.Clone(newEntry);
			_Events.Add(newEntry);
		}

		#endregion

	}
}
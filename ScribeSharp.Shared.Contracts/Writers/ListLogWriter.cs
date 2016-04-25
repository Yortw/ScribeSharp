using PoolSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Adds <see cref="LogEvent"/> instances to an in memory list. The list has a maximum capacity. When the capacity is reached, older events are removed.
	/// </summary>
	public sealed class ListLogWriter : LogWriterBase, IDisposable
	{

		#region Fields

		private IList<LogEvent> _Events;
		private int _MaximumCapacity;

		private IPool<LogEvent> _Pool;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="events">An <see cref="IList{LogEvent}"/> instance that log events will be added to.</param>
		/// <param name="maximumCapacity">A the maximum number of log events to keep in the list at any one time. When the list capacity is reached, the first event in the list is removed before the next event is added.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="events"/> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="maximumCapacity"/> is less than or equal to 0.</exception>
		public ListLogWriter(IList<LogEvent> events, int maximumCapacity) : this(events, maximumCapacity, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="events">An <see cref="IList{LogEvent}"/> instance that log events will be added to.</param>
		/// <param name="maximumCapacity">A the maximum number of log events to keep in the list at any one time. When the list capacity is reached, the first event in the list is removed before the next event is added.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="events"/> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="maximumCapacity"/> is less than or equal to 0.</exception>
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

		/// <summary>
		/// Returns true.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Clones the supplied <see cref="LogEvent"/> instance and adds the clone to the list.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to clone and add to the queue.</param>
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (logEvent == null) return;

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

		#region IDisposable

		/// <summary>
		/// Disposes this instance an all internal resources.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Pool", Justification = "It is disposed, CA just can't tell.")]
		public void Dispose()
		{
			_Pool?.Dispose();
		}

		#endregion

	}
}
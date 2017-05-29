using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Provides a clock that caches the current time for a specified interval, which can reduce allocations when calling <see cref="DateTimeOffset.Now"/>, <see cref="DateTimeOffset.UtcNow"/>, <see cref="DateTime.Now"/> or <see cref="DateTimeOffset.UtcNow"/>.
	/// </summary>
	public class CachingClock : ILogClock
	{
		private DateTimeOffset _CachedTime;
		private int _LastCheckedTimeTicks;
		private int _CacheIntervalTicks;
		private ILogClock _InnerClock;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="innerClock">Another <see cref="ILogClock"/> implementation to actually get the current time from when required.</param>
		/// <param name="cacheInterval">The interval to cache the time for once it has been requested. Subsequent requests after that time will be cached.</param>
		public CachingClock(ILogClock innerClock, TimeSpan cacheInterval)
		{
			if (innerClock == null) throw new ArgumentNullException(nameof(innerClock));
			if (cacheInterval.Ticks > Int32.MaxValue) throw new ArgumentOutOfRangeException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ArgumentOutOfRangeMessage, "cacheInterval.Ticks", 1, Int32.MaxValue));

			_InnerClock = innerClock;
			_CacheIntervalTicks = Convert.ToInt32(cacheInterval.Ticks);
			_CachedTime = innerClock.Now;
			_LastCheckedTimeTicks = Environment.TickCount;
		}

		/// <summary>
		/// Returns the approximate (within cache interval) current date and time as a <see cref="DateTimeOffset"/>.
		/// </summary>
		public DateTimeOffset Now
		{
			get
			{
				if (Environment.TickCount - _LastCheckedTimeTicks >= _CacheIntervalTicks)
				{
					_CachedTime = _InnerClock.Now;
					_LastCheckedTimeTicks = Environment.TickCount;
				}

				return _CachedTime;
			}
		}

	}
}
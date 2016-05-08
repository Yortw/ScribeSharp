using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An <seealso cref="ILogClock"/> instance that always returns <seealso cref="DateTimeOffset.MinValue"/>.
	/// </summary>
	/// <remarks>
	/// <para>Useful for setting the date and time of wrapping loggers, where you want the inner logger to set the date and time.</para>
	/// </remarks>
	public class MinTimeClock : ILogClock
	{
		/// <summary>
		/// Returns <seealso cref="DateTimeOffset.MinValue"/>.
		/// </summary>
		public DateTimeOffset Now
		{
			get
			{
				return DateTimeOffset.MinValue;
			}
		}

		private static MinTimeClock s_DefaultInstance;

		/// <summary>
		/// Returns a common instance of the clock. Reuse of the same instance reduces allocations.
		/// </summary>
		public static MinTimeClock DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new MinTimeClock()); }
		}

	}
}
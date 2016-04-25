using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An <seealso cref="ILogClock"/> instance that returns <seealso cref="DateTimeOffset.UtcNow"/>, so the system time converted to UTC.
	/// </summary>
	public class UtcSystemClock : ILogClock
	{
		/// <summary>
		/// Returns <seealso cref="DateTimeOffset.UtcNow"/>.
		/// </summary>
		public DateTimeOffset Now
		{
			get
			{
				return DateTimeOffset.UtcNow;
			}
		}

		private static UtcSystemClock s_DefaultInstance;
		
		/// <summary>
		/// Returns a common instance of the clock. Reuse of the same instance reduces allocations.
		/// </summary>
		public static UtcSystemClock DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new UtcSystemClock()); }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An <seealso cref="ILogClock"/> instance that returns <seealso cref="DateTimeOffset.Now"/>, so the local time of the device.
	/// </summary>
	public class LocalSystemClock : ILogClock
	{
		/// <summary>
		/// Returns <seealso cref="DateTimeOffset.Now"/>.
		/// </summary>
		public DateTimeOffset Now
		{
			get
			{
				return DateTime.Now;
			}
		}

		private static LocalSystemClock s_DefaultInstance;

		/// <summary>
		/// Returns a common instance of the clock. Reuse of the same instance reduces allocations.
		/// </summary>
		public static LocalSystemClock DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new LocalSystemClock()); }
		}

	}
}
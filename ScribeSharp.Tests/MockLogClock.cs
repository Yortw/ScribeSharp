using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	public sealed class MockLogClock : ILogClock
	{
		private DateTimeOffset _FixedTime;

		public MockLogClock(DateTimeOffset fixedTime)
		{
			_FixedTime = fixedTime;
		}

		public DateTimeOffset Now
		{
			get
			{
				return _FixedTime;
			}
		}
	}
}
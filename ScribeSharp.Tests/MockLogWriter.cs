using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	public class MockLogWriter : Writers.LogWriterBase
	{
		private LogEvent _LastEvent;
		private bool _RequiresSynchronisation;

		public MockLogWriter() : base()
		{
		}

		protected override void WriteEventInternal(LogEvent logEvent)
		{
			_LastEvent = logEvent;
		}

		public LogEvent LastEvent
		{
			get { return _LastEvent; }
		}

		public override bool RequiresSynchronization
		{
			get
			{
				return _RequiresSynchronisation;
			}
		}

		public void SetRequiresSynchronisation(bool value)
		{
			_RequiresSynchronisation = value;
		}

	}

	public class MockLogWriterWithError : Writers.LogWriterBase
	{
		public override bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

		protected override void WriteEventInternal(LogEvent logEvent)
		{
			throw new InvalidOperationException();
		}
	}
}
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

		public MockLogWriter(ILogEventFilter filter) : base(filter) { }

		protected override void WriteFilteredEvent(LogEvent logEvent)
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
}
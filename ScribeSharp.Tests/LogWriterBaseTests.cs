using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class LogWriterBaseTests
	{

		[TestMethod]
		public void LogWriterBase_WriteEvent_UsesFilter()
		{
			var logWriter = new MockLogWriter(new Filters.LogSeverityFilter(LogEventSeverity.Error));
			var logEvent = new LogEvent() { EventName = "Test" };
			logWriter.Write(logEvent);
			Assert.AreEqual(null, logWriter.LastEvent);

			logEvent.EventSeverity = LogEventSeverity.Error;
			logWriter.Write(logEvent);
			Assert.AreEqual(logEvent, logWriter.LastEvent);
		}

		[TestMethod]
		public void LogWriterBase_WriteEvent_WorksWithNullFilter()
		{
			var logWriter = new MockLogWriter(null);
			var logEvent = new LogEvent() { EventName = "Test" };
			logWriter.Write(logEvent);
			Assert.AreEqual(logEvent, logWriter.LastEvent);
		}

	}
}
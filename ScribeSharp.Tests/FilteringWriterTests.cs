using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class FilteringWriterTests
	{

		#region Constructor Tests


		[TestMethod]
		[TestCategory("FilteringLogWriter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FilteredWriter_Constructor_ThrowsOnNullFilter()
		{
			var mockWriter = new MockLogWriter();
			var logWriter = new Writers.FilteringLogWriter(null, mockWriter);
		}

		[TestMethod]
		[TestCategory("FilteringLogWriter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FilteredWriter_Constructor_ThrowsOnNullLogWriter()
		{
			var mockWriter = new MockLogWriter();
			var logWriter = new Writers.FilteringLogWriter(new Filters.LogSeverityFilter(LogEventSeverity.Error), null);
		}

		#endregion

		#region WriteEvent Tests

		[TestMethod]
		[TestCategory("FilteringLogWriter")]
		[TestCategory("Writers")]
		public void FilteredWriter_WriteEvent_DoesNotWriteEventWhenFilterFails()
		{
			var mockWriter = new MockLogWriter();
			var logWriter = new Writers.FilteringLogWriter(new Filters.LogSeverityFilter(LogEventSeverity.Error), mockWriter);
			var logEvent = new LogEvent() { EventName = "Test" };
			logWriter.Write(logEvent);
			Assert.AreEqual(null, mockWriter.LastEvent);
		}

		[TestMethod]
		[TestCategory("FilteringLogWriter")]
		[TestCategory("Writers")]
		public void FilteredWriter_WriteEvent_WritesEventWhenFilterPasses()
		{
			var mockWriter = new MockLogWriter();
			var logWriter = new Writers.FilteringLogWriter(new Filters.LogSeverityFilter(LogEventSeverity.Error), mockWriter);

			var logEvent = new LogEvent() { EventName = "Test" };
			logEvent.EventSeverity = LogEventSeverity.Error;
			logWriter.Write(logEvent);
			Assert.AreEqual(logEvent, mockWriter.LastEvent);
		}

		#endregion

	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class AsyncQueueLogWriterTests
	{
		
		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AsyncQueueLogWriterTests_Constructor_ThrowsOnNullChildren()
		{
			var logWriter = new AsyncQueueLogWriter(null, 1, TimeSpan.Zero, null);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void AsyncQueueLogWriterTests_Constructor_ThrowsOnInvalidBatchSize()
		{
			var logWriter = new AsyncQueueLogWriter(new MockLogWriter(), 0, TimeSpan.Zero, null);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("Writers")]
		public void AsyncQueueLogWriterTests_WriteEvent_WritesToChild()
		{
			var events = new List<LogEvent>(10);
			var childWriter = new ListLogWriter(events, 10);
			var logWriter = new AsyncQueueLogWriter(childWriter, 1, TimeSpan.Zero, null);
			var logEvent = new LogEvent() { EventName = "Test" };
			logWriter.Write(logEvent);
			System.Threading.Thread.Sleep(250);
			Assert.AreEqual(1, events.Count);
			Assert.AreEqual(logEvent.EventName, events[0].EventName);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("Writers")]
		public void AggregateLogWriter_WriteEvent_WritesToChildOnBatchSizeReached()
		{
			var events = new List<LogEvent>(10);
			var child = new ListLogWriter(events, 10);
			var logWriter = new AsyncQueueLogWriter(child, 10, TimeSpan.Zero, null);

			for (int cnt = 0; cnt < 5; cnt++)
			{
				var logEvent = new LogEvent();
				logEvent.EventName = "Test " + Guid.NewGuid().ToString();
				logWriter.Write(logEvent);
			}
			System.Threading.Thread.Sleep(250);
			Assert.AreEqual(0, events.Count);

			for (int cnt = 0; cnt < 5; cnt++)
			{
				var logEvent = new LogEvent();
				logEvent.EventName = "Test " + Guid.NewGuid().ToString();
				logWriter.Write(logEvent);
			}
			System.Threading.Thread.Sleep(250);
			Assert.AreEqual(10, events.Count);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("Writers")]
		public void AggregateLogWriter_WriteEvent_WritesAfterTimeoutIfBatchSizeNotReached()
		{
			var events = new List<LogEvent>(10);
			var child = new ListLogWriter(events, 10);
			var logWriter = new AsyncQueueLogWriter(child, 10, TimeSpan.FromSeconds(1), null);

			for (int cnt = 0; cnt < 5; cnt++)
			{
				var logEvent = new LogEvent();
				logEvent.EventName = "Test " + Guid.NewGuid().ToString();
				logWriter.Write(logEvent);
			}
			Assert.AreEqual(0, events.Count);
			System.Threading.Thread.Sleep(250);
			Assert.AreEqual(0, events.Count);

			System.Threading.Thread.Sleep(1000);

			Assert.AreEqual(5, events.Count);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("Writers")]
		public void AggregateLogWriter_WriteEvent_FlushesOnDispose()
		{
			var events = new List<LogEvent>(10);
			var child = new ListLogWriter(events, 10);
			var logWriter = new AsyncQueueLogWriter(child, 10, TimeSpan.Zero, null);

			for (int cnt = 0; cnt < 5; cnt++)
			{
				var logEvent = new LogEvent();
				logEvent.EventName = "Test " + Guid.NewGuid().ToString();
				logWriter.Write(logEvent);
			}
			System.Threading.Thread.Sleep(250);
			Assert.AreEqual(0, events.Count);

			logWriter.Dispose();
			Assert.AreEqual(5, events.Count);
		}

		[TestMethod]
		[TestCategory("AsyncQueueLogWriter")]
		[TestCategory("Writers")]
		public void AggregateLogWriter_RequiresSynchronisation_IsFalse()
		{
			var child = new MockLogWriter();
			var logWriter = new AsyncQueueLogWriter(child, 10, TimeSpan.Zero, null);

			Assert.AreEqual(false, logWriter.RequiresSynchronization);
		}

	}
}
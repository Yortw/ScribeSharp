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
	public class FallbackWriterTests
	{

		#region Constructor Tests

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_Constructor_ThrowsOnNullPrimaryWriter()
		{
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(null, secondaryWriter);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_Constructor_ThrowsOnNullSecondaryWriter()
		{
			var primaryList = new List<LogEvent>();
			var primaryWriter = new ListLogWriter(primaryList, 10);

			var writer = new FallbackWriter(primaryWriter, null);
		}

		[TestMethod]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_Constructor_ConstructsOkWithValidWriters()
		{
			var primaryList = new List<LogEvent>();
			var primaryWriter = new ListLogWriter(primaryList, 10);
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);
		}

		#endregion

		#region Write Tests

		[TestMethod]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_Write_WritesToPrimaryWriter()
		{
			var primaryList = new List<LogEvent>();
			var primaryWriter = new ListLogWriter(primaryList, 10);
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			var logEvent = new LogEvent()
			{
				EventName = "Test log message"
			};
			writer.Write(logEvent);

			Assert.AreEqual(1, primaryList.Count);
			Assert.AreEqual(0, secondaryList.Count);
			Assert.AreEqual("Test log message", primaryList[0].EventName);
		}

		[TestMethod]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_Write_FallsbackToSecondaryWriter()
		{
			var primaryWriter = new MockLogWriterWithError();
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			var logEvent = new LogEvent()
			{
				EventName = "Test log message"
			};
			writer.Write(logEvent);

			Assert.AreEqual(1, secondaryList.Count);
			Assert.AreEqual("Test log message", secondaryList[0].EventName);
		}

		[TestMethod]
		[TestCategory("FallbackWriter")]
		[ExpectedException(typeof(LogWriterException))]
		public void FallbackWriter_Write_RethrowsOnSecondaryError()
		{
			var primaryWriter = new MockLogWriterWithError();
			var secondaryWriter = new MockLogWriterWithError();

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			var logEvent = new LogEvent()
			{
				EventName = "Test log message"
			};
			writer.Write(logEvent);
		}

		#endregion

		#region Write Batch Tests

		[TestMethod]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_WriteBatch_WritesToPrimaryWriter()
		{
			var primaryList = new List<LogEvent>();
			var primaryWriter = new ListLogWriter(primaryList, 10);
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			LogEvent[] logEvents = new LogEvent[2];
			logEvents[0] =  new LogEvent()
			{
				EventName = "Test log message"
			};
			logEvents[1] = new LogEvent()
			{
				EventName = "Test log message 2"
			};

			writer.WriteBatch(logEvents);

			Assert.AreEqual(2, primaryList.Count);
			Assert.AreEqual(0, secondaryList.Count);
			Assert.AreEqual("Test log message", primaryList[0].EventName);
			Assert.AreEqual("Test log message 2", primaryList[1].EventName);
		}

		[TestMethod]
		[TestCategory("FallbackWriter")]
		public void FallbackWriter_WriteBatch_FallsbackToSecondaryWriter()
		{
			var primaryWriter = new MockLogWriterWithError();
			var secondaryList = new List<LogEvent>();
			var secondaryWriter = new ListLogWriter(secondaryList, 10);

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			LogEvent[] logEvents = new LogEvent[2];
			logEvents[0] = new LogEvent()
			{
				EventName = "Test log message"
			};
			logEvents[1] = new LogEvent()
			{
				EventName = "Test log message 2"
			};

			writer.WriteBatch(logEvents);

			Assert.AreEqual(2, secondaryList.Count);
			Assert.AreEqual("Test log message", secondaryList[0].EventName);
			Assert.AreEqual("Test log message 2", secondaryList[1].EventName);
		}

		[TestMethod]
		[TestCategory("FallbackWriter")]
		[ExpectedException(typeof(LogWriterException))]
		public void FallbackWriter_WriteBatch_RethrowsOnSecondaryError()
		{
			var primaryWriter = new MockLogWriterWithError();
			var secondaryWriter = new MockLogWriterWithError();

			var writer = new FallbackWriter(primaryWriter, secondaryWriter);

			LogEvent[] logEvents = new LogEvent[2];
			logEvents[0] = new LogEvent()
			{
				EventName = "Test log message"
			};
			logEvents[1] = new LogEvent()
			{
				EventName = "Test log message 2"
			};

			writer.WriteBatch(logEvents);
		}

		#endregion

	}
}
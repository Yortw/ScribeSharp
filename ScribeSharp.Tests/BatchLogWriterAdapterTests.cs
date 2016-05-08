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
	public class BatchLogWriterAdapterTests
	{

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestCategory("BatchLogWriterAdapter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		public void BatchLogWriterAdapter_Constructor_ThrowsOnNullInnerWriter()
		{
			var writer = new BatchLogWriterAdapter(null);
		}

		[TestMethod]
		[TestCategory("BatchLogWriterAdapter")]
		[TestCategory("Writers")]
		public void BatchLogWriterAdapter_Constructor_ConstructsOk()
		{
			var list = new List<LogEvent>(10);
			var listWriter = new ListLogWriter(list, 10);
			var writer = new BatchLogWriterAdapter(listWriter);
		}

		[TestMethod]
		[TestCategory("BatchLogWriterAdapter")]
		[TestCategory("Writers")]
		public void BatchLogWriterAdapter_WriteBatch_WritesAllEventsFromArray()
		{
			var list = new List<LogEvent>(10);
			var listWriter = new ListLogWriter(list, 10);
			var writer = new BatchLogWriterAdapter(listWriter);

			var logEvents = new LogEvent[2];
			logEvents[0] = new LogEvent()
			{
				EventName = "Test"
			};
			logEvents[1] = new LogEvent()
			{
				EventName = "Test 2"
			};

			writer.WriteBatch(logEvents, 2);

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("Test", list[0].EventName);
			Assert.AreEqual("Test 2", list[1].EventName);
		}

		[TestMethod]
		[TestCategory("BatchLogWriterAdapter")]
		[TestCategory("Writers")]
		public void BatchLogWriterAdapter_WriteBatch_WritesSomeEventsFromArray()
		{
			var list = new List<LogEvent>(10);
			var listWriter = new ListLogWriter(list, 10);
			var writer = new BatchLogWriterAdapter(listWriter);

			var logEvents = new LogEvent[2];
			logEvents[0] = new LogEvent()
			{
				EventName = "Test"
			};
			logEvents[1] = new LogEvent()
			{
				EventName = "Test 2"
			};

			writer.WriteBatch(logEvents, 1);

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("Test", list[0].EventName);
		}

		[TestMethod]
		[TestCategory("BatchLogWriterAdapter")]
		[TestCategory("Writers")]
		public void BatchLogWriterAdapter_WriteBatch_WritesAllEventsFromEnumerable()
		{
			var list = new List<LogEvent>(10);
			var listWriter = new ListLogWriter(list, 10);
			var writer = new BatchLogWriterAdapter(listWriter);

			var logEvents = new List<LogEvent>(2);
			logEvents.Add(new LogEvent()
			{
				EventName = "Test"
			});
			logEvents.Add(new LogEvent()
			{
				EventName = "Test 2"
			});

			writer.WriteBatch(logEvents);

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("Test", list[0].EventName);
			Assert.AreEqual("Test 2", list[1].EventName);
		}

	}
}
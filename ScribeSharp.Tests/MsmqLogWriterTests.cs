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
	public class MsmqLogWriterTests
	{

		private const string _TestQueuePath = ".\\PRIVATE$\\ScribeSharpTests";
	
		#region Constructor Tests

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Constructor_ConstructsOkWithValidQueuePath()
		{
			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			var writer = new MsmqLogWriter(_TestQueuePath, false);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Constructor_ConstructsOkAndCreatesNewQueue()
		{
			if (System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Delete(_TestQueuePath);

			var writer = new MsmqLogWriter(_TestQueuePath, true);

			Assert.IsTrue(System.Messaging.MessageQueue.Exists(_TestQueuePath));
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("MsmqLogWriter")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MsmqLogWriter_Constructor_ThrowsOnNullQueue()
		{
			var writer = new MsmqLogWriter((System.Messaging.MessageQueue)null);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Constructor_ConstructsOkWithNullFormatter()
		{
			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			var writer = new MsmqLogWriter(new System.Messaging.MessageQueue(_TestQueuePath), null);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Constructor_ConstructsOkWithFormatter()
		{
			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			var writer = new MsmqLogWriter(new System.Messaging.MessageQueue(_TestQueuePath), Formatters.JsonLogEventFormatter.DefaultInstance);
		}

		#endregion

		#region Write Tests

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Write_WritesToQueue()
		{
			if (System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Delete(_TestQueuePath);

			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			using (var writeQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.Send))
			{
				writeQueue.Formatter = new MsmqXmlLogEventMessageFormatter();

				var writer = new MsmqLogWriter(writeQueue);

				using (var readQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.SendAndReceive))
				{
					readQueue.Formatter = new MsmqXmlLogEventMessageFormatter();

					var logEvent = new LogEvent()
					{
						EventName = "Test Event",
						Source = "Test Source"
					};
					writer.Write(logEvent);

					var result = readQueue.Receive(TimeSpan.FromSeconds(5));
					Assert.IsNotNull(result);
					var xdoc = System.Xml.Linq.XDocument.Parse(result.Body.ToString());
					Assert.IsNotNull((from n in xdoc.Descendants("LogEvent") select n).FirstOrDefault());
				}
			}
		}

		#endregion

		#region WriteBatch Tests

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Write_WritesFullBatchArrayToQueue()
		{
			if (System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Delete(_TestQueuePath);

			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			using (var writeQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.Send))
			{
				writeQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

				var writer = new MsmqLogWriter(writeQueue);

				using (var readQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.SendAndReceive))
				{
					readQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

					LogEvent[] logEvents = new LogEvent[2];
					logEvents[0] = new LogEvent()
					{
						EventName = "Test Event",
						Source = "Test Source"
					};
					logEvents[1] = new LogEvent()
					{
						EventName = "Test Event 2",
						Source = "Test Source"
					};
					writer.WriteBatch(logEvents, 2);

					var result = readQueue.Receive(TimeSpan.FromSeconds(5));
					Assert.IsNotNull(result);

					var results = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent[]>(result.Body.ToString());
					Assert.AreEqual(2, results.Length);
					Assert.AreEqual("Test Event", results[0].EventName);
					Assert.AreEqual("Test Event 2", results[1].EventName);
				}
			}
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Write_WritesPartialBatchArrayToQueue()
		{
			if (System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Delete(_TestQueuePath);

			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			using (var writeQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.Send))
			{
				writeQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

				var writer = new MsmqLogWriter(writeQueue);

				using (var readQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.SendAndReceive))
				{
					readQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

					LogEvent[] logEvents = new LogEvent[2];
					logEvents[0] = new LogEvent()
					{
						EventName = "Test Event",
						Source = "Test Source"
					};
					logEvents[1] = new LogEvent()
					{
						EventName = "Test Event 2",
						Source = "Test Source"
					};
					writer.WriteBatch(logEvents, 1);

					var result = readQueue.Receive(TimeSpan.FromSeconds(5));
					Assert.IsNotNull(result);

					var results = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent[]>(result.Body.ToString());
					Assert.AreEqual(1, results.Length);
					Assert.AreEqual("Test Event", results[0].EventName);
				}
			}
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("MsmqLogWriter")]
		public void MsmqLogWriter_Write_WritesEnumerableBatchToQueue()
		{
			if (System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Delete(_TestQueuePath);

			if (!System.Messaging.MessageQueue.Exists(_TestQueuePath))
				System.Messaging.MessageQueue.Create(_TestQueuePath, false);

			using (var writeQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.Send))
			{
				writeQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

				var writer = new MsmqLogWriter(writeQueue);

				using (var readQueue = new System.Messaging.MessageQueue(_TestQueuePath, false, true, System.Messaging.QueueAccessMode.SendAndReceive))
				{
					readQueue.Formatter = new MsmqJsonLogEventMessageFormatter();

					var logEvents = new List<LogEvent>(2);
					logEvents.Add(new LogEvent()
					{
						EventName = "Test Event",
						Source = "Test Source"
					});
					logEvents.Add(new LogEvent()
					{
						EventName = "Test Event 2",
						Source = "Test Source"
					});
					writer.WriteBatch(logEvents);

					var result = readQueue.Receive(TimeSpan.FromSeconds(5));
					Assert.IsNotNull(result);

					var results = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent[]>(result.Body.ToString());
					Assert.AreEqual(2, results.Length);
					Assert.AreEqual("Test Event", results[0].EventName);
					Assert.AreEqual("Test Event 2", results[1].EventName);
				}
			}
		}

		#endregion

	}
}
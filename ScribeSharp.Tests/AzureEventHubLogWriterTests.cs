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
	public class AzureEventHubLogWriterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void AzureEventHubLogWriter_Constructor_ThrowsOnNullConnectionString()
		{
			var writer = new AzureEventHubLogWriter(null,  Formatters.JsonLogEventFormatter.DefaultInstance);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void AzureEventHubLogWriter_Constructor_ThrowsOnEmptyConnectionString()
		{
			var writer = new AzureEventHubLogWriter(String.Empty);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void AzureEventHubLogWriter_Constructor_ThrowsOnWhitespaceConnectionString()
		{
			var writer = new AzureEventHubLogWriter("  ", Formatters.JsonLogEventFormatter.DefaultInstance);
		}
		
		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		[TestCategory("ApiQualityTests")]
		public void AzureEventHubLogWriter_Constructor_ConstructsOkWithNullFormatter()
		{
			var writer = new AzureEventHubLogWriter(ConnectionString, null);
		}

		#endregion

		#region Write Tests

#if !RUN_AZUREEVENTHUBTESTS
		[Ignore]
#endif
		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		public void AzureEventHubLogWriter_Write_WritesToEventHub()
		{
			var writer = new AzureEventHubLogWriter(ConnectionString, null);
			writer.Write(new LogEvent()
			{
				DateTime = DateTime.Now,
				EventName = "Test event",
				EventType = LogEventType.SystemEvent,
				Source = "Test Source",
				Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
			});
		}

		#endregion

		#region Write Tests

#if !RUN_AZUREEVENTHUBTESTS
		[Ignore]
#endif
		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		public void AzureEventHubLogWriter_WriteBatch_WritesToEventHub()
		{
			var writer = new AzureEventHubLogWriter(ConnectionString, null);

			int itemsInBatch = 50;
			var batch = new List<LogEvent>(itemsInBatch);
			for (int cnt = 0; cnt < itemsInBatch; cnt++)
			{
				batch.Add(new LogEvent()
				{
					DateTime = DateTime.Now,
					EventName = "Test event " + cnt.ToString(),
					EventType = LogEventType.SystemEvent,
					Source = "Test Source",
					Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
				});
			}
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			writer.WriteBatch(batch);
			sw.Stop();
			System.Diagnostics.Trace.WriteLine($"Writing {itemsInBatch} log events to event hub took {sw.Elapsed.ToString()}");
		}

#if !RUN_AZUREEVENTHUBTESTS
		[Ignore]
#endif
		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory(nameof(AzureEventHubLogWriter))]
		public void AzureEventHubLogWriter_WriteBatch_WritesOverSizedBatch()
		{
			var writer = new AzureEventHubLogWriter(ConnectionString, null);

			int itemsInBatch = 2;
			var batch = new List<LogEvent>(itemsInBatch);
			for (int cnt = 0; cnt < itemsInBatch; cnt++)
			{
				batch.Add(new LogEvent()
				{
					DateTime = DateTime.Now,
					EventName = new string('A', 250 * 1024),
					EventType = LogEventType.SystemEvent,
					Source = "Test Source",
					Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
				});
			}
			writer.WriteBatch(batch);
		}

#endregion

#region Utility/Setup Stuff

		private const string EventHubName = "logevents";

		public string ConnectionString
		{
			get { return TestSecrets.TestEventHubConnectionString; }
		}

#endregion
	}
}
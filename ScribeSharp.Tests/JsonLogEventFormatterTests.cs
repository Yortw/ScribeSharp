using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class JsonLogEventFormatterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("Formatters")]
		[TestCategory("JsonLogEventFormatter")]
		public void JsonLogEventFormatter_Constructor_ConstructsOk()
		{
			var formatter = new Formatters.JsonLogEventFormatter();
		}

		#endregion

		#region Constructor Tests

		[TestMethod]
		[TestCategory("Formatters")]
		[TestCategory("JsonLogEventFormatter")]
		public void JsonLogEventFormatter_Constructor_FormatToStringWorksOk()
		{
			var formatter = new Formatters.JsonLogEventFormatter();

			var logEvent = new LogEvent()
			{
				EventName = "Test event.",
				Source = "Test source",
				Properties = new Dictionary<string, object>() { { "Test Property", "Test value" } }
			};
			var result = formatter.FormatToString(logEvent);

			var deserialisedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent>(result);

			Assert.AreEqual(logEvent.EventName, deserialisedItem.EventName);
			Assert.AreEqual(logEvent.DateTime, deserialisedItem.DateTime);
			Assert.AreEqual(logEvent.EventSeverity, deserialisedItem.EventSeverity);
			Assert.AreEqual(logEvent.EventType, deserialisedItem.EventType);
			Assert.AreEqual(logEvent.Source, deserialisedItem.Source);
			Assert.IsNotNull(deserialisedItem.Properties);
			Assert.AreEqual(1, deserialisedItem.Properties.Count);
			Assert.AreEqual("Test value", deserialisedItem.Properties["Test Property"]);
		}

		[TestMethod]
		[TestCategory("Formatters")]
		[TestCategory("JsonLogEventFormatter")]
		public void JsonLogEventFormatter_Constructor_FormatToWriterWorksOk()
		{
			var formatter = new Formatters.JsonLogEventFormatter();

			var logEvent = new LogEvent()
			{
				EventName = "Test event.",
				Source = "Test source",
				Properties = new Dictionary<string, object>() { { "Test Property", "Test value" } }
			};

			var sb = new System.Text.StringBuilder();
			using (var writer = new System.IO.StringWriter(sb))
			{
				formatter.FormatToTextWriter(logEvent, writer);

				var deserialisedItem = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent>(sb.ToString());

				Assert.AreEqual(logEvent.EventName, deserialisedItem.EventName);
				Assert.AreEqual(logEvent.DateTime, deserialisedItem.DateTime);
				Assert.AreEqual(logEvent.EventSeverity, deserialisedItem.EventSeverity);
				Assert.AreEqual(logEvent.EventType, deserialisedItem.EventType);
				Assert.AreEqual(logEvent.Source, deserialisedItem.Source);
				Assert.IsNotNull(deserialisedItem.Properties);
				Assert.AreEqual(1, deserialisedItem.Properties.Count);
				Assert.AreEqual("Test value", deserialisedItem.Properties["Test Property"]);
			}
		}

		#endregion

	}
}
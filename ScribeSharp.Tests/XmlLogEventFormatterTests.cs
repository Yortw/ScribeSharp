using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class XmlLogEventFormatterTests
	{

		#region Constructors

		[TestMethod]
		[TestCategory("XmlLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void XmlLogEventFormatter_Constructor_ConstructsOk()
		{
			var formatter = new XmlLogEventFormatter();
		}

		[TestMethod]
		[TestCategory("XmlLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void XmlLogEventFormatter_Constructor_ConstructsOkWithTypeRendererMap()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new XmlLogEventFormatter(rendererMap);
		}

		[TestMethod]
		[TestCategory("XmlLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void XmlLogEventFormatter_Constructor_ConstructsOkWithNullTypeRendererMap()
		{
			var formatter = new XmlLogEventFormatter(null);
		}

		#endregion

		#region FormatToString Tests

		[TestMethod]
		[TestCategory("XmlLogEventFormatter")]
		[TestCategory("Formatters")]
		public void XmlLogEventFormatter_FormatToString_FormatsExpectedOutput()
		{
			var logEvent = new LogEvent()
			{
				EventName = "Test event log.",
				Source = "Test Source",
				Properties = new Dictionary<string, object>() { { "Test Property", "Test Value" } },
				Exception = new System.InvalidOperationException()
			};

			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new XmlLogEventFormatter(rendererMap);

			var xdoc = System.Xml.Linq.XDocument.Parse(formatter.FormatToString(logEvent));

			Assert.AreEqual("Test event log.", (from n in xdoc.Descendants("EventName") select n).First().Value);
			var eventNode = (from n in xdoc.Descendants("LogEvent") select n).First();
			Assert.AreEqual(LogEventSeverity.Information.ToString(), eventNode.Attribute("Severity").Value);
			Assert.AreEqual((int)LogEventSeverity.Information, Convert.ToInt32(eventNode.Attribute("SeverityLevel").Value));
			Assert.AreEqual(LogEventType.ApplicationEvent.ToString(), eventNode.Attribute("EventType").Value);
			Assert.AreEqual("Test Source", eventNode.Attribute("Source").Value);
			Assert.AreEqual(logEvent.DateTime.ToString("O", System.Globalization.CultureInfo.CurrentCulture), eventNode.Attribute("EventDate").Value);

			var propNode = (from n in xdoc.Descendants("Property") where n.Attribute("Key")?.Value == "Test Property" select n).First();
			Assert.AreEqual("Test Value", propNode.Attribute("Value").Value);

			var exceptionNode = (from n in xdoc.Descendants("Exception") select n).First();
			Assert.AreEqual("System.InvalidOperationException", exceptionNode.Attribute("Type").Value);
			Assert.AreEqual("Operation is not valid due to the current state of the object.", exceptionNode.Descendants("Message").First().Value);
		}

		#endregion

		#region FormatToWriter Tests

		[TestMethod]
		[TestCategory("XmlLogEventFormatter")]
		[TestCategory("Formatters")]
		public void XmlLogEventFormatter_FormatToWriter_FormatsExpectedOutput()
		{
			var logEvent = new LogEvent()
			{
				EventName = "Test event log.",
				Source = "Test Source",
				Properties = new Dictionary<string, object>() { { "Test Property", "Test Value" } },
				Exception = new System.InvalidOperationException()
			};

			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new XmlLogEventFormatter(rendererMap);

			var sb = new StringBuilder();
			using (var writer = new System.IO.StringWriter(sb))
			{
				formatter.FormatToTextWriter(logEvent, writer);
				var xdoc = System.Xml.Linq.XDocument.Parse(sb.ToString());

				Assert.AreEqual("Test event log.", (from n in xdoc.Descendants("EventName") select n).First().Value);
				var eventNode = (from n in xdoc.Descendants("LogEvent") select n).First();
				Assert.AreEqual(LogEventSeverity.Information.ToString(), eventNode.Attribute("Severity").Value);
				Assert.AreEqual((int)LogEventSeverity.Information, Convert.ToInt32(eventNode.Attribute("SeverityLevel").Value));
				Assert.AreEqual(LogEventType.ApplicationEvent.ToString(), eventNode.Attribute("EventType").Value);
				Assert.AreEqual("Test Source", eventNode.Attribute("Source").Value);
				Assert.AreEqual(logEvent.DateTime.ToString("O", System.Globalization.CultureInfo.CurrentCulture), eventNode.Attribute("EventDate").Value);

				var propNode = (from n in xdoc.Descendants("Property") where n.Attribute("Key")?.Value == "Test Property" select n).First();
				Assert.AreEqual("Test Value", propNode.Attribute("Value").Value);

				var exceptionNode = (from n in xdoc.Descendants("Exception") select n).First();
				Assert.AreEqual("System.InvalidOperationException", exceptionNode.Attribute("Type").Value);
				Assert.AreEqual("Operation is not valid due to the current state of the object.", exceptionNode.Descendants("Message").First().Value);
			}
		}

		#endregion

	}
}
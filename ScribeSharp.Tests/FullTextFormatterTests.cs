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
	public class FullTextFormatterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		public void FullTextFormatter_Constructor_DefaultContructsOk()
		{
			var formatter = new FullTextLogEventFormatter();
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		public void FullTextFormatter_Constructor_ConstructsOkWithNullRendererMap()
		{
			var formatter = new FullTextLogEventFormatter(null);
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		public void FullTextFormatter_Constructor_ConstructsOkWithRendererMap()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
		}

		#endregion

		#region FormatToString Tests

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsEventName()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event."
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Event Name: Test log event."));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsSource()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource"
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Source: TestSource"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsSourceMethod()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod"
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Source Method: TestSourceMethod"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsSourceLineNumber()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod",
				SourceLineNumber = 50
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Source Line: 50"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsDateTime()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod",
				SourceLineNumber = 50,
				DateTime = new DateTime(2016, 05, 08, 12, 15, 36)
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Date: " + logEvent.DateTime.ToString("G", System.Globalization.CultureInfo.InvariantCulture)));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsSeverity()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod",
				SourceLineNumber = 50,
				DateTime = new DateTime(2016, 05, 08, 12, 15, 36),
				EventSeverity = LogEventSeverity.Diagnostic
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Severity: Diagnostic"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsType()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod",
				SourceLineNumber = 50,
				DateTime = new DateTime(2016, 05, 08, 12, 15, 36),
				EventSeverity = LogEventSeverity.Diagnostic,
				EventType = LogEventType.Start
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Type: Start"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_ContainsCustomProperties()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "TestSource",
				SourceMethod = "TestSourceMethod",
				SourceLineNumber = 50,
				DateTime = new DateTime(2016, 05, 08, 12, 15, 36),
				EventSeverity = LogEventSeverity.Diagnostic,
				EventType = LogEventType.Start,
				Properties = new Dictionary<string, object>() { { "Test Property", "Test Value" } }
			};
			var result = formatter.FormatToString(logEvent);
			Assert.IsTrue(result.Contains("Properties:"));
			Assert.IsTrue(result.Contains("Test Property: Test Value"));
		}

		[TestMethod]
		[TestCategory("FullTextFormatter")]
		[TestCategory("Formatters")]
		public void FullTextFormatter_FormatToString_UsesExceptionFormatter()
		{
			var rendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new FullTextLogEventFormatter(rendererMap);
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Exception = new System.InvalidOperationException()
			};
			var result = formatter.FormatToString(logEvent);
			var exceptionData = logEvent.Exception.ToXml();
			Assert.IsTrue(result.Contains(exceptionData));
		}

		#endregion

	}
}
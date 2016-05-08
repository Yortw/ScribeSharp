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
	public class SimpleLogEventFormatterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("SimpleLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void SimpleLogEventFormatter_Constructor_ConstructsOk()
		{
			var formatter = new SimpleLogEventFormatter();
		}

		[TestMethod]
		[TestCategory("SimpleLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void SimpleLogEventFormatter_Constructor_ConstructsOkWithNullTypeRendererMap()
		{
			var formatter = new SimpleLogEventFormatter(null);
		}

		[TestMethod]
		[TestCategory("SimpleLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void SimpleLogEventFormatter_Constructor_ConstructsOkWithValidTypeRendererMap()
		{
			var typeRendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new SimpleLogEventFormatter(typeRendererMap);
		}

		#endregion

		#region FormatToString Tests

		[TestMethod]
		[TestCategory("SimpleLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void SimpleLogEventFormatter_FormatToString_WritesExpectedOutput()
		{
			var typeRendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new SimpleLogEventFormatter(typeRendererMap);

			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "Test Source"
			};

			Assert.AreEqual("[01/01/0001 00:00:00] [Information] [ApplicationEvent] [Test Source] [] Test log event.\r\n", formatter.FormatToString(logEvent));
		}

		#endregion

		#region FormatToWriter Tests

		[TestMethod]
		[TestCategory("SimpleLogEventFormatter")]
		[TestCategory("Formatters")]
		[TestCategory("ApiQualityTests")]
		public void SimpleLogEventFormatter_FormatToWriter_WritesExpectedOutput()
		{
			var typeRendererMap = new ScribeSharp.PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new PropertyRenderers.ExceptionAsXmlRenderer()));
			var formatter = new SimpleLogEventFormatter(typeRendererMap);

			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				Source = "Test Source"
			};

			var sb = new System.Text.StringBuilder();
			using (var writer = new System.IO.StringWriter(sb))
			{
				formatter.FormatToTextWriter(logEvent, writer);

				Assert.AreEqual("[01/01/0001 00:00:00] [Information] [ApplicationEvent] [Test Source] [] Test log event.\r\n", sb.ToString());
			}
		}

		#endregion

	}
}
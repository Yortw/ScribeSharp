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
	public class MinimalLogEventFormatterTests
	{

		#region Constructors

		[TestMethod]
		[TestCategory("MinimalLogEventFormatter")]
		[TestCategory("Formatters")]
		public void MinimalLogEventFormatter_Constructor_ConstructsOk()
		{
			var formatter = new MinimalLogEventFormatter();
		}

		#endregion

		#region FormatToString Tests

		[TestMethod]
		[TestCategory("MinimalLogEventFormatter")]
		[TestCategory("Formatters")]
		public void MinimalLogEventFormatter_FormatToString_OutputsExpectedText()
		{
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				EventSeverity = LogEventSeverity.Warning,
				EventType = LogEventType.Performance
			};

			var formatter = new MinimalLogEventFormatter();
			var result = formatter.FormatToString(logEvent);

			Assert.AreEqual("[Warning] [Performance] Test log event.", result);
		}

		#endregion

		#region FormatToWriter Tests

		[TestMethod]
		[TestCategory("MinimalLogEventFormatter")]
		[TestCategory("Formatters")]
		public void MinimalLogEventFormatter_FormatToWriter_OutputsExpectedText()
		{
			var logEvent = new LogEvent()
			{
				EventName = "Test log event.",
				EventSeverity = LogEventSeverity.Warning,
				EventType = LogEventType.Performance
			};

			var sb = new System.Text.StringBuilder();
			using (var writer = new System.IO.StringWriter(sb))
			{
				var formatter = new MinimalLogEventFormatter();
				formatter.FormatToTextWriter(logEvent, writer);
			}

			Assert.AreEqual("[Warning] [Performance] Test log event.", sb.ToString());
		}

		#endregion
	}
}
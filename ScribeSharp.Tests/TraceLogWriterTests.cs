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
	public class TraceLogWriterTests
	{
		#region Write Tests

		[TestMethod]
		[TestCategory(nameof(TraceLogWriter))]
		[TestCategory("Writers")]
		public void TraceLogWriter_Write_NoExceptionWhenMessageContainsLeftBrace()
		{
			var writer = new TraceLogWriter(Formatters.FullTextLogEventFormatter.DefaultInstance);

			var logEvent = new LogEvent()
			{
				EventName = "Test event {."
			};
			writer.Write(logEvent);
		}

		[TestMethod]
		[TestCategory(nameof(TraceLogWriter))]
		[TestCategory("Writers")]
		public void TraceLogWriter_Write_NoExceptionWhenMessageContainsRightBrace()
		{
			var writer = new TraceLogWriter(Formatters.FullTextLogEventFormatter.DefaultInstance);

			var logEvent = new LogEvent()
			{
				EventName = "Test event }."
			};
			writer.Write(logEvent);
		}

		[TestMethod]
		[TestCategory(nameof(TraceLogWriter))]
		[TestCategory("Writers")]
		public void TraceLogWriter_Write_NoExceptionWhenMessageContainsBraces()
		{
			var writer = new TraceLogWriter(Formatters.FullTextLogEventFormatter.DefaultInstance);

			var logEvent = new LogEvent()
			{
				EventName = "Test event { some text }."
			};
			writer.Write(logEvent);
		}

		#endregion
	}
}
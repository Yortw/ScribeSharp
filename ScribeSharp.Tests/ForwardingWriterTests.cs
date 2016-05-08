using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Tests;
using ScribeSharp.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class ForwardingWriterTests
	{
		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestCategory("Writers")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("ForwardingWriter")]
		public void ForwardingWriter_Constructor_ThrowsOnNullLogger()
		{
			var writer = new ForwardingLogWriter(null);
		}

		[TestMethod]
		[TestCategory("Writers")]
		[TestCategory("ForwardingWriter")]
		public void ForwardingWriter_Write_WritesToInnerLogger()
		{
			var list = new List<LogEvent>();
			var listWriter = new ListLogWriter(list, 10);
			var policy = new LogPolicy()
			{
				LogWriter = listWriter
			};
			var innerLogger = new Logger(policy);
			var writer = new ForwardingLogWriter(innerLogger);

			var logEvent = new LogEvent()
			{
				EventName = "Test log event."
			};
			writer.Write(logEvent);

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("Test log event.", list[0].EventName);
		}

	}
}
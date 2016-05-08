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
	public class StreamLogWriterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void StreamLogWriter_Constructor_ThrowsOnNullStream()
		{
			var writer = new StreamLogWriter(null, System.Text.UTF8Encoding.UTF8, Formatters.SimpleLogEventFormatter.DefaultInstance);
		}

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		public void StreamLogWriter_Constructor_AllowsNullFormatter()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var writer = new StreamLogWriter(stream, System.Text.UTF8Encoding.UTF8, Formatters.SimpleLogEventFormatter.DefaultInstance);
			}
		}

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		public void StreamLogWriter_Constructor_AllowsNullEncoding()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var writer = new StreamLogWriter(stream, null, Formatters.SimpleLogEventFormatter.DefaultInstance);
			}
		}

		#endregion

		#region Write Tests

		[TestMethod]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		public void StreamLogWriter_Write_WritesToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var writer = new StreamLogWriter(stream, System.Text.UTF8Encoding.UTF8, Formatters.SimpleLogEventFormatter.DefaultInstance);
				var logEvent = new LogEvent()
				{
					EventName = "Test event."
				};
				writer.Write(logEvent);
				Assert.IsTrue(stream.Length > 0);
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				var result = System.Text.UTF8Encoding.UTF8.GetString(stream.GetBuffer());
				Assert.IsTrue(result.Contains("Test event."));
			}
		}

		#endregion

		#region WriteBatch Tests

		[TestMethod]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		public void StreamLogWriter_WriteBatch_WritesArrayToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var writer = new StreamLogWriter(stream, System.Text.UTF8Encoding.UTF8, Formatters.SimpleLogEventFormatter.DefaultInstance);
				var logEvents = new LogEvent[]
				{
					new LogEvent()
					{
						EventName = "Test event."
					},
					new LogEvent()
					{
						EventName = "Test event 2."
					}
				};
				writer.WriteBatch(logEvents, 2);
				Assert.IsTrue(stream.Length > 0);
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				var result = System.Text.UTF8Encoding.UTF8.GetString(stream.GetBuffer());
				Assert.IsTrue(result.Contains("Test event."));
				Assert.IsTrue(result.Contains("Test event 2."));
			}
		}

		[TestMethod]
		[TestCategory("StreamLogWriter")]
		[TestCategory("Writers")]
		public void StreamLogWriter_WriteBatch_WritesEnumerableToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var writer = new StreamLogWriter(stream, System.Text.UTF8Encoding.UTF8, Formatters.SimpleLogEventFormatter.DefaultInstance);
				var logEvents = new List<LogEvent>() 
				{
					new LogEvent()
					{
						EventName = "Test event."
					},
					new LogEvent()
					{
						EventName = "Test event 2."
					}
				};
				writer.WriteBatch(logEvents);
				Assert.IsTrue(stream.Length > 0);
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				var result = System.Text.UTF8Encoding.UTF8.GetString(stream.GetBuffer());
				Assert.IsTrue(result.Contains("Test event."));
				Assert.IsTrue(result.Contains("Test event 2."));
			}
		}

		#endregion

	}
}
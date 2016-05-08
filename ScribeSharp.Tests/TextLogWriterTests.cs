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
	public class TextLogWriterTests
	{
		#region Constructor Tests

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void TextLogWriter_Constructor_ThrowsOnNullWriter()
		{
			var writer = new TextLogWriter(null, Formatters.SimpleLogEventFormatter.DefaultInstance);
		}

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		public void TextLogWriter_Constructor_AllowsNullFormatter()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				using (var textWriter = new System.IO.StreamWriter(stream))
				{
					var writer = new TextLogWriter(textWriter, Formatters.SimpleLogEventFormatter.DefaultInstance);
				}
			}
		}

		[TestMethod]
		[TestCategory("ApiQualityTests")]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		public void TextLogWriter_Constructor_AllowsNullEncoding()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				using (var textWriter = new System.IO.StreamWriter(stream))
				{
					var writer = new TextLogWriter(textWriter, Formatters.SimpleLogEventFormatter.DefaultInstance);
				}
			}
		}

		#endregion

		#region Write Tests

		[TestMethod]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		public void TextLogWriter_Write_WritesToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				using (var textWriter = new System.IO.StreamWriter(stream))
				{
					var writer = new TextLogWriter(textWriter, Formatters.SimpleLogEventFormatter.DefaultInstance);
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
		}

		#endregion

		#region WriteBatch Tests

		[TestMethod]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		public void TextLogWriter_WriteBatch_WritesArrayToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				using (var textWriter = new System.IO.StreamWriter(stream))
				{
					var writer = new TextLogWriter(textWriter, Formatters.SimpleLogEventFormatter.DefaultInstance);
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
		}

		[TestMethod]
		[TestCategory("TextLogWriter")]
		[TestCategory("Writers")]
		public void TextLogWriter_WriteBatch_WritesEnumerableToStream()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				using (var textWriter = new System.IO.StreamWriter(stream))
				{
					var writer = new TextLogWriter(textWriter, Formatters.SimpleLogEventFormatter.DefaultInstance);
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
		}

		#endregion
	}
}
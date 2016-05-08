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
	public class ListLogWriterTests
	{

		[TestMethod]
		[TestCategory("ListLogWriter")]
		[TestCategory("Writers")]
		public void ListLogWriter_Write_WritesToList()
		{
			var list = new List<LogEvent>(10);
			var logWriter = new ListLogWriter(list, list.Capacity);

			var logEvent = new LogEvent();
			logEvent.EventName = "Test";
			logWriter.Write(logEvent);

			Assert.AreEqual(1, list.Count);
		}

		[TestMethod]
		[TestCategory("ListLogWriter")]
		[TestCategory("Writers")]
		public void ListLogWriter_Write_EnforcesCapacity()
		{
			var list = new List<LogEvent>(5);
			var logWriter = new ListLogWriter(list, list.Capacity);

			for (int cnt = 0; cnt < 10; cnt++)
			{
				var logEvent = new LogEvent();
				logEvent.EventName = "Test " + cnt.ToString();
				logWriter.Write(logEvent);
			}

			Assert.AreEqual(5, list.Count);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("ListLogWriter")]
		[TestCategory("Writers")]
		[TestMethod]
		public void ListLogWriter_Constructor_ThrowsOnNullList()
		{
			var logWriter = new ListLogWriter(null, 10);
		}

		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("ListLogWriter")]
		[TestCategory("Writers")]
		[TestMethod]
		public void ListLogWriter_Constructor_ThrowsOnInvalidCapacity()
		{
			var logWriter = new ListLogWriter(new List<LogEvent>(), 0);
		}


	}
}
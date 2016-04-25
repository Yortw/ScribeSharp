using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Formatters;
using ScribeSharp.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class ConsoleLogWriterTests
	{

		[TestMethod]
		public void ConsoleLogWriter_Write_Writers()
		{
			var formatter = SimpleLogEventFormatter.DefaultInstance;
			var logWriter = new ConsoleLogWriter(formatter);

			var logEvent = new LogEvent() { EventName = "Test" };
			var reader = new System.IO.StreamReader(Console.OpenStandardInput());
			logWriter.Write(logEvent);
			System.Threading.Thread.Sleep(10000);
			var s = reader.ReadLine();
			
		}
	}
}
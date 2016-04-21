using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
using System.Collections.Generic;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class LoggerTests
	{

		#region Constructor Tests

		[TestCategory("Logger")]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Logger_Constructor_ThrowsOnNullPolicy()
		{
			var logger = new Logger(null);
		}

		[TestCategory("Logger")]
		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Logger_Constructor_ThrowsOnNullWriter()
		{
			var policy = new LogPolicy()
			{
			};
			var logger = new Logger(policy);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_Constructor_DefaultsToEnabled()
		{
			var policy = GetSimpleListPolicy();
			var logger = new Logger(policy);
			Assert.IsTrue(logger.IsEnabled);
		}

		#endregion

		#region WriteEvent Tests

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_CallsLogWriter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.WriteEvent("Log this!");
			Assert.AreEqual(1, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_DoesNotCallLogWriterWhenDisabled()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.IsEnabled = false;
			logger.WriteEvent("Log this!");
			Assert.AreEqual(0, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_LogsPassedProperties()
		{
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			var propValue = System.Guid.NewGuid();
			logger.WriteEvent("Log this!", properties: new Dictionary<string, object>() { { "Test Id", propValue } });
			Assert.IsTrue(list[0].Properties.ContainsKey("Test Id"));
			Assert.AreEqual(propValue, list[0].Properties["Test Id"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_PerformanceCheck()
		{
			int testIterations = 100;
			TimeSpan maxTime = TimeSpan.FromMilliseconds(10); // This is generous, to allow for variations caused by things outside the logging system (GC/CLR/OS etc)
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			for (int cnt = 0; cnt < testIterations; cnt++)
			{
				logger.WriteEvent($"Log this {cnt.ToString()}!");
			}
			sw.Stop();
			Assert.IsTrue(sw.ElapsedMilliseconds < maxTime.TotalMilliseconds, $"Logging 100 simple calls took more than {maxTime.ToString()}");
			System.Diagnostics.Trace.WriteLine($"Logging 100 simple calls took {maxTime.ToString()}");
		}

		[TestCategory("Logger")]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Logger_WriteEvent_ThrowsOnNullLogEvent()
		{
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.WriteEvent((LogEvent)null);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesFilter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.Filter = new Filters.LogSeverityFilter(LogEventSeverity.Warning);
			var logger = new Logger(policy);
			logger.WriteEvent("Log this!");
			Assert.AreEqual(0, list.Count);
			logger.WriteEvent("Log this!", eventSeverity: LogEventSeverity.Warning);
			Assert.AreEqual(1, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesLogClock()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var myEpoch = new DateTimeOffset(635968343126200923, TimeSpan.Zero);
			policy.Clock = new MockLogClock(myEpoch);
			var logger = new Logger(policy);
			logger.WriteEvent("Log this!");
			Assert.AreEqual(myEpoch, list[0].DateTime);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesContext()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.MachineNameLogEventContextProvider() };
			var logger = new Logger(policy);
			logger.WriteEvent("Log this!");
			Assert.AreEqual(Environment.MachineName, list[0].Properties["Machine Name"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesContextFilter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.MachineNameLogEventContextProvider(new Filters.LogSeverityFilter(LogEventSeverity.Warning)) };
			var logger = new Logger(policy);
			logger.WriteEvent("Log this!", eventSeverity: LogEventSeverity.Information);
			Assert.IsFalse(list[0].Properties.ContainsKey("Machine Name"));
			logger.WriteEvent("Log this!", eventSeverity: LogEventSeverity.Warning);
			Assert.AreEqual(Environment.MachineName, list[1].Properties["Machine Name"]);
		}

		#endregion

		#region Private Support Methods

		private static LogPolicy GetSimpleListPolicy()
		{
			return GetSimpleListPolicy(new List<LogEvent>(10));
		}

		private static LogPolicy GetSimpleListPolicy(List<LogEvent> list)
		{
			return new LogPolicy()
			{
				LogWriter = new ListLogWriter(list, list.Capacity == 0 ? 10 : list.Capacity)
			};
		}

		#endregion

	}
}
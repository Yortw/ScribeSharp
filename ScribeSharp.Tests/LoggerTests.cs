using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
using System.Collections.Generic;
using System.Linq;

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
			logger.WriteEvent("Log this!", new KeyValuePair<string, object>("Test Id", propValue));
			Assert.IsTrue(list[0].Properties.ContainsKey("Test Id"));
			Assert.AreEqual(propValue, list[0].Properties["Test Id"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_PerformanceCheck()
		{
			int testIterations = 1000;
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
			Assert.IsTrue(sw.ElapsedMilliseconds < maxTime.TotalMilliseconds, $"Logging {testIterations} simple calls took more than {maxTime.Milliseconds} ms ({sw.ElapsedMilliseconds} ms).");
			System.Diagnostics.Trace.WriteLine($"Logging {testIterations} simple calls took {maxTime.ToString()}");
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

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesFirstChanceFilter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(LogEventSeverity.Information, null);
			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information);
			logger.WriteEvent("Log Verbose", eventSeverity: LogEventSeverity.Verbose);
			Assert.AreEqual(1, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_AppliesContextProperties()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.FixedValueLogEntryContextProvider("Test Prop", "Test Prop Value") };
			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information);
			Assert.AreEqual(1, list[0].Properties.Count);
			Assert.AreEqual("Test Prop Value", list[0].Properties["Test Prop"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEvent_UsesPropertyRenderers()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.FixedValueLogEntryContextProvider("Test Converted Prop", DateTime.MinValue) };
			policy.TypeRendererMap = new PropertyRenderers.TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(DateTime), new PropertyRenderers.ToStringRenderer("G")));

			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information);
			Assert.AreEqual(1, list[0].Properties.Count);
			Assert.AreEqual(DateTime.MinValue.ToString("G", System.Globalization.CultureInfo.InvariantCulture), list[0].Properties["Test Converted Prop"].ToString());
			Assert.IsTrue(list[0].Properties["Test Converted Prop"] is string);
		}

		#endregion

		#region WriteEventWithSource Tests

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_SetsSource()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.Source = null;
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.IsTrue(list.Last().Source.EndsWith("LoggerTests.cs"));
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_SetsSourceMethod()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.Source = null;
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual("Logger_WriteEventWithSource_SetsSourceMethod", list.Last().SourceMethod);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_SetsSourceLineNumber()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.Source = null;
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreNotEqual(-1, list.Last().SourceLineNumber);
			Assert.AreNotEqual(0, list.Last().SourceLineNumber);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_CallsLogWriter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual(1, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_DoesNotCallLogWriterWhenDisabled()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.IsEnabled = false;
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual(0, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_LogsPassedProperties()
		{
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			var propValue = System.Guid.NewGuid();
			logger.WriteEventWithSource("Log this!", properties: new KeyValuePair<string, object>("Test Id", propValue));
			Assert.IsTrue(list[0].Properties.ContainsKey("Test Id"));
			Assert.AreEqual(propValue, list[0].Properties["Test Id"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_PerformanceCheck()
		{
			int testIterations = 1000;
			TimeSpan maxTime = TimeSpan.FromMilliseconds(10); // This is generous, to allow for variations caused by things outside the logging system (GC/CLR/OS etc)
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			for (int cnt = 0; cnt < testIterations; cnt++)
			{
				logger.WriteEventWithSource($"Log this {cnt.ToString()}!");
			}
			sw.Stop();
			Assert.IsTrue(sw.ElapsedMilliseconds < maxTime.TotalMilliseconds, $"Logging {testIterations} simple calls took more than {maxTime.Milliseconds} ms ({sw.ElapsedMilliseconds} ms).");
			System.Diagnostics.Trace.WriteLine($"Logging {testIterations} simple calls took {maxTime.ToString()}");
		}

		[TestCategory("Logger")]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Logger_WriteEventWithSource_ThrowsOnNullLogEvent()
		{
			var list = new List<LogEvent>(1);
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);
			logger.WriteEventWithSource((LogEvent)null);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_UsesFilter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.Filter = new Filters.LogSeverityFilter(LogEventSeverity.Warning);
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual(0, list.Count);
			logger.WriteEventWithSource("Log this!", eventSeverity: LogEventSeverity.Warning);
			Assert.AreEqual(1, list.Count);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_UsesLogClock()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var myEpoch = new DateTimeOffset(635968343126200923, TimeSpan.Zero);
			policy.Clock = new MockLogClock(myEpoch);
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual(myEpoch, list[0].DateTime);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_UsesContext()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.MachineNameLogEventContextProvider() };
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!");
			Assert.AreEqual(Environment.MachineName, list[0].Properties["Machine Name"]);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_WriteEventWithSource_UsesContextFilter()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ContextProviders = new ILogEventContextProvider[] { new ContextProviders.MachineNameLogEventContextProvider(new Filters.LogSeverityFilter(LogEventSeverity.Warning)) };
			var logger = new Logger(policy);
			logger.WriteEventWithSource("Log this!", eventSeverity: LogEventSeverity.Information);
			Assert.IsFalse(list[0].Properties.ContainsKey("Machine Name"));
			logger.WriteEventWithSource("Log this!", eventSeverity: LogEventSeverity.Warning);
			Assert.AreEqual(Environment.MachineName, list[1].Properties["Machine Name"]);
		}

		#endregion

		#region BeginLoggedJob Tests

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_BeginLoggedJob_ReturnsNonNullToken()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();
			Assert.IsNotNull(logger.BeginLoggedJob("Test", jobId));
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_BeginLoggedJob_WritesStartAndStop()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();
			using (var loggedJob = logger.BeginLoggedJob("Test", jobId))
			{
			}

			Assert.AreEqual(2, list.Count);

			Assert.AreEqual(LogEventSeverity.Information, list[0].EventSeverity);
			Assert.AreEqual(LogEventType.Start, list[0].EventType);
			Assert.AreEqual(jobId, list[0].Properties["Job ID"]);

			Assert.AreEqual(LogEventSeverity.Information, list[1].EventSeverity);
			Assert.AreEqual(LogEventType.Completed, list[1].EventType);
			Assert.AreEqual(jobId, list[1].Properties["Job ID"]);
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_BeginLoggedJob_WritesExceptions()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();
			using (var loggedJob = logger.BeginLoggedJob("Test", jobId))
			{
				loggedJob.SetFailure(new InvalidOperationException());
			}

			Assert.AreEqual(3, list.Count);

			Assert.AreEqual(LogEventSeverity.Information, list[0].EventSeverity);
			Assert.AreEqual(LogEventType.Start, list[0].EventType);
			Assert.AreEqual(jobId, list[0].Properties["Job ID"]);

			Assert.AreEqual(LogEventSeverity.Error, list[1].EventSeverity);
			Assert.AreEqual(LogEventType.Failure, list[1].EventType);
			Assert.AreEqual(jobId, list[1].Properties["Job ID"]);

			Assert.AreEqual(LogEventSeverity.Error, list[2].EventSeverity);
			Assert.AreEqual(LogEventType.Completed, list[2].EventType);
			Assert.AreEqual(jobId, list[2].Properties["Job ID"]);
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_BeginLoggedJob_RecordsDuration()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();
			using (var loggedJob = logger.BeginLoggedJob("Test", jobId))
			{
			}

			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list[1].Properties.ContainsKey("Duration"));
			Assert.IsFalse(String.IsNullOrWhiteSpace(list[1].Properties["Duration"].ToString()));
		}

		#endregion

		#region BeginLoggedJob Tests

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_CreateChildJob_IncludesJobID()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();
			using (var rootJob = logger.BeginLoggedJob("Test", jobId))
			{
				var childJobId = Guid.NewGuid().ToString();
				using (var childJob = rootJob.CreateChildJob("Child Job Test", childJobId))
				{
					Assert.AreEqual(jobId, list.Last().Properties["Parent Job ID"]);
					Assert.AreEqual(childJobId, list.Last().Properties["Job ID"]);

					var grandChildJobId = Guid.NewGuid().ToString();
					using (var grandChildJob = childJob.CreateChildJob("Grandchild Job Test", grandChildJobId))
					{
						Assert.AreEqual(childJobId, list.Last().Properties["Parent Job ID"]);
						Assert.AreEqual(grandChildJobId, list.Last().Properties["Job ID"]);
					}
				}
			}
		}

		#endregion

		#region ExecuteLoggedJob Tests

		[TestMethod]
		[TestCategory("Logger")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Logger_ExecuteLoggedJob_ThrowsOnNullJob()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();

			logger.ExecuteLoggedJob(null, "Test", jobId);
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_ExecuteLoggedJob_ExecutesJob()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			bool wasExecuted = false;
			var jobId = Guid.NewGuid().ToString();

			logger.ExecuteLoggedJob(() =>
			{
				wasExecuted = true;
			},
			"Test", jobId);

			Assert.AreEqual(true, wasExecuted);
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_ExecuteLoggedJob_LogsExceptions()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			bool exceptionWasRethrown = false;
			var jobId = Guid.NewGuid().ToString();
			try
			{
				logger.ExecuteLoggedJob(() =>
				{
					throw new InvalidOperationException();
				},
				"Test", jobId);
			}
			catch (InvalidOperationException)
			{
				exceptionWasRethrown = true;
			}

			Assert.AreEqual(true, exceptionWasRethrown);

			Assert.AreEqual(3, list.Count);

			Assert.AreEqual(LogEventSeverity.Information, list[0].EventSeverity);
			Assert.AreEqual(LogEventType.Start, list[0].EventType);
			Assert.AreEqual(jobId, list[0].Properties["Job ID"]);

			Assert.AreEqual(LogEventSeverity.Error, list[1].EventSeverity);
			Assert.AreEqual(LogEventType.Failure, list[1].EventType);
			Assert.AreEqual(jobId, list[1].Properties["Job ID"]);

			Assert.AreEqual(LogEventSeverity.Error, list[2].EventSeverity);
			Assert.AreEqual(LogEventType.Completed, list[2].EventType);
			Assert.AreEqual(jobId, list[2].Properties["Job ID"]);
		}

		[TestMethod]
		[TestCategory("Logger")]
		public void Logger_ExecuteLoggedJob_IncludesAdditionalProperties()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			var logger = new Logger(policy);

			var jobId = Guid.NewGuid().ToString();

			logger.ExecuteLoggedJob(
				() =>
				{
				},
				"Test", jobId,
				new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Additional Property", "123") }
			);

			Assert.IsTrue(list[0].Properties.ContainsKey("Additional Property"));
			Assert.AreEqual("123", list[0].Properties["Additional Property"]);

			Assert.IsTrue(list[1].Properties.ContainsKey("Additional Property"));
			Assert.AreEqual("123", list[1].Properties["Additional Property"]);
		}

		#endregion

		#region Error Handling Tests

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_ErrorHandling_WriteEventWithSourceCallsErrorHandler()
		{
			var wasCalled = false;
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ErrorHandler = new SuppressingErrorHandler();
			policy.ErrorHandler.ErrorOccurred += (sender, args) => wasCalled = true;
			policy.Filter = new Filters.DelegateLogEventFilter((logEvent) => { throw new InvalidOperationException("Test exception"); });
			var logger = new Logger(policy);

			logger.WriteEventWithSource("Log this!");

			Assert.AreEqual(wasCalled, true);
		}

		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_ErrorHandling_DoesNotThrowWhenSuppressed()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ErrorHandler = new SuppressingErrorHandler();
			policy.Filter = new Filters.DelegateLogEventFilter((logEvent) => { throw new InvalidOperationException("Test exception"); });
			var logger = new Logger(policy);

			logger.WriteEventWithSource("Log this!");
		}


		[ExpectedException(typeof(LogException))]
		[TestCategory("Logger")]
		[TestMethod]
		public void Logger_ErrorHandling_ThrowsLogExceptionWhenNotSuppressed()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.ErrorHandler = new RethrowErrorHandler();
			policy.Filter = new Filters.DelegateLogEventFilter((logEvent) => { throw new InvalidOperationException("Test exception"); });
			var logger = new Logger(policy);

			logger.WriteEventWithSource("Log this!");
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
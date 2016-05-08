using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class StandardFirstChanceFilterTests
	{

		#region MinimumSeverity Tests

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_MinimumSeverity_PassesMinimumSeverity()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), null);
			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information);
			Assert.AreEqual(1, list.Count);
		}

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_MinimumSeverity_FiltersLowerSeverity()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), null);
			var logger = new Logger(policy);
			logger.WriteEvent("Log Verbose", eventSeverity: LogEventSeverity.Verbose);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_MinimumSeverity_PassesGreaterSeverity()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), null);
			var logger = new Logger(policy);
			logger.WriteEvent("Log Warning", eventSeverity: LogEventSeverity.Warning);
			Assert.AreEqual(1, list.Count);
		}

		#endregion

		#region MinimumSeverity Tests

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_AllowedEventTypes_PassesAnyEventTypeOnNullList()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), null);

			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.ApplicationEvent);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Canceled);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Completed);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Failure);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Pause);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Performance);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Resume);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.ScheduledEvent);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.SecurityAuditFailure);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.SecurityAuditSuccess);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Start);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.SystemEvent);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.UserAction);

			Assert.AreEqual(13, list.Count);
		}

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_AllowedEventTypes_PassesOnlyEventTypesInlist()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), new LogEventType[] { LogEventType.Performance });

			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.Performance);

			Assert.AreEqual(1, list.Count);
		}

		[TestMethod]
		[TestCategory("StandardFirstChanceFilter")]
		public void StandardFirstChanceFilter_AllowedEventTypes_FiltersEventTypesNotInlist()
		{
			var list = new List<LogEvent>();
			var policy = GetSimpleListPolicy(list);
			policy.FirstChanceFilter = new Filters.StandardFirstChanceFilter(new LogEventSeveritySwitch(LogEventSeverity.Information), new LogEventType[] { LogEventType.Performance });

			var logger = new Logger(policy);
			logger.WriteEvent("Log Info", eventSeverity: LogEventSeverity.Information, eventType: LogEventType.ScheduledEvent);

			Assert.AreEqual(0, list.Count);
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
				LogWriter = new ListLogWriter(list, list.Capacity == 0 ? 20: list.Capacity)
			};
		}

		#endregion

	}
}

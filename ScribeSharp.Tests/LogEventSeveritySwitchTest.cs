using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class LogEventSeveritySwitchTest
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("LogEventSeveritySwitch")]
		public void LogEventSeveritySwitch_Constructor_InitialisesWithSpecifiedSeverity()
		{
			var severitySwitch = new LogEventSeveritySwitch(LogEventSeverity.Error);
			Assert.AreEqual(LogEventSeverity.Error, severitySwitch.MinimumSeverity);
		}

		#endregion

		#region IsAllowed Tests

		[TestMethod]
		[TestCategory("LogEventSeveritySwitch")]
		public void LogEventSeveritySwitch_IsAllowed_AllowsMinimumSeverity()
		{
			var severitySwitch = new LogEventSeveritySwitch(LogEventSeverity.Error);
			Assert.IsTrue(severitySwitch.IsAllowed(severitySwitch.MinimumSeverity));
		}

		[TestMethod]
		[TestCategory("LogEventSeveritySwitch")]
		public void LogEventSeveritySwitch_IsAllowed_AllowsHigherSeverities()
		{
			var severitySwitch = new LogEventSeveritySwitch(LogEventSeverity.Error);
			Assert.IsTrue(severitySwitch.IsAllowed(LogEventSeverity.CriticalError));
			Assert.IsTrue(severitySwitch.IsAllowed(LogEventSeverity.FatalError));
		}

		[TestMethod]
		[TestCategory("LogEventSeveritySwitch")]
		public void LogEventSeveritySwitch_IsAllowed_DisallowsLowerSeverities()
		{
			var severitySwitch = new LogEventSeveritySwitch(LogEventSeverity.Error);
			Assert.IsFalse(severitySwitch.IsAllowed(LogEventSeverity.Information));
		}

		#endregion

	}
}
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
	public class AggregateLogWriterTests
	{
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AggregateLogWriter_Constructor_ThrowsOnNullChidlren()
		{
			var logWriter = new AggregateLogWriter(null);
		}

		[TestMethod]
		public void AggregateLogWriter_WriteEvent_WritersToMultipleChildren()
		{
			var child1 = new MockLogWriter();
			var child2 = new MockLogWriter();
			var logWriter = new AggregateLogWriter(new ILogWriter[] { child1, child2 } );
			var logEvent = new LogEvent() { EventName = "Test" };
			logWriter.Write(logEvent);

			Assert.AreEqual(logEvent, child1.LastEvent);
			Assert.AreEqual(logEvent, child2.LastEvent);
		}

		[TestMethod]
		public void AggregateLogWriter_WriteEvent_RequiresSynchronisationIsTrueIfAnyChildRequires()
		{
			var child1 = new MockLogWriter();
			var child2 = new MockLogWriter();
			child2.SetRequiresSynchronisation(true);
			var logWriter = new AggregateLogWriter(new ILogWriter[] { child1, child2 });

			Assert.AreEqual(true, logWriter.RequiresSynchronization);
		}

		[TestMethod]
		public void AggregateLogWriter_WriteEvent_RequiresSynchronisationIsFalseIfNoChildRequires()
		{
			var child1 = new MockLogWriter();
			var child2 = new MockLogWriter();
			var logWriter = new AggregateLogWriter(new ILogWriter[] { child1, child2 });

			Assert.AreEqual(false, logWriter.RequiresSynchronization);
		}

	}
}
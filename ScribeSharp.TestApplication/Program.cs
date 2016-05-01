using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			var policy = new LogPolicy()
			{
				LogWriter = new ScribeSharp.Writers.AggregateLogWriter(new ILogWriter[] 
				{
					new ScribeSharp.Writers.ConsoleLogWriter(null), 
					new ScribeSharp.Writers.WindowsEventLogWriter("TestLog", "Test Source", ".", true, System.Diagnostics.OverflowAction.OverwriteAsNeeded, 30, ScribeSharp.Formatters.SimpleLogEventFormatter.DefaultInstance, null)
				}
				),
				PropertyRenderers = new Dictionary<Type, IPropertyRenderer>() { { typeof(TestProperty), new PropertyRenderers.XmlPropertyRenderer(typeof(TestProperty)) } }
			};
			var logger = new Logger(policy);

			logger.WriteEventWithSource("Started");

			logger.WriteEventWithSource("Test", LogEventSeverity.Information);

			logger.WriteEventWithSource("Error", eventSeverity: LogEventSeverity.Error);

			var props = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Test1", "Test2"), new KeyValuePair<string, object>("Test2", new TestProperty()) };
			logger.WriteEventWithSource("Test", properties: props);

			var childLogger = logger.CreateChildLogger("childlogger");
			childLogger.WriteEventWithSource("Child logger test.");

			logger.WriteEventWithSource("Test", LogEventSeverity.Diagnostic, LogEventType.ScheduledEvent);
			logger.WriteEventWithSource("Test1", LogEventSeverity.Diagnostic);
			logger.WriteEvent("Test1", LogEventSeverity.Diagnostic, LogEventType.ScheduledEvent, new KeyValuePair<string, object>("Test2", "Test2"));

			logger.WriteEventWithSource("Stopped");

			Console.ReadLine();
		}
	}

	public class TestProperty
	{
		public string Name = "TestName";
		public int Sequence = 1;
	}
}
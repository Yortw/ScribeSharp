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
				LogWriter = new ScribeSharp.Writers.ConsoleLogWriter(null), //ScribeSharp.Formatters.SimpleLogEventFormatter.DefaultInstance)
				PropertyRenderers = new Dictionary<Type, IPropertyRenderer>() { { typeof(TestProperty), new PropertyRenderers.XmlPropertyRenderer(typeof(TestProperty)) } }
			};
			var logger = new Logger(policy);

			logger.WriteEvent("Started");

			logger.WriteEvent("Test", LogEventSeverity.Information);

			logger.WriteEvent("Error", eventSeverity: LogEventSeverity.Error);

			var props = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Test1", "Test2"), new KeyValuePair<string, object>("Test2", new TestProperty()) };
			logger.WriteEvent("Test", properties: props);

			var childLogger = logger.CreateChildLogger("childlogger");
			childLogger.WriteEvent("Child logger test.");

			logger.WriteEvent("Test", LogEventSeverity.Diagnostic, LogEventType.ScheduledEvent);
			logger.WriteEvent("Test1", LogEventSeverity.Diagnostic);
			logger.WriteEvent("Test1", LogEventSeverity.Diagnostic, LogEventType.ScheduledEvent, new KeyValuePair<string, object>("Test2", "Test2"));

			logger.WriteEvent("Stopped");

			Console.ReadLine();
		}


	}

	public class TestProperty
	{
		public string Name = "TestName";
		public int Sequence = 1;
	}
}
using ScribeSharp.Writers;
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
			//int testIterations = 100;
			//TimeSpan maxTime = TimeSpan.FromMilliseconds(10); // This is generous, to allow for variations caused by things outside the logging system (GC/CLR/OS etc)
			//var list = new List<LogEvent>(1);
			//var policy = GetSimpleListPolicy(list);
			//var logger = new Logger(policy);
			//var sw = new System.Diagnostics.Stopwatch();
			//sw.Start();
			//for (int cnt = 0; cnt < testIterations; cnt++)
			//{
			//	logger.WriteEvent($"Log this {cnt.ToString()}!");
			//	//logger.WriteEvent($"Log this!", LogEventSeverity.Information, LogEventType.ApplicationEvent, null, null);
			//}
			//sw.Stop();


			var policy = new LogPolicy()
			{
				LogWriter = new ScribeSharp.Writers.AggregateLogWriter(
					new ILogWriter[]
					{
						new ScribeSharp.Writers.ConsoleLogWriter(null),
						new ScribeSharp.Writers.WindowsEventLogWriter("TestLog", "Test Source", ".", true, System.Diagnostics.OverflowAction.OverwriteAsNeeded, 30, 
							new ScribeSharp.Formatters.XmlLogEventFormatter(), null)
					}
				),
				//PropertyRenderers = new Dictionary<Type, IPropertyRenderer>() { { typeof(TestProperty), new PropertyRenderers.XmlPropertyRenderer(typeof(TestProperty)) } }
			};
			var logger = new Logger(policy);

			logger.WriteEventWithSource("Started");

			logger.WriteEventWithSource("Test", LogEventSeverity.Information);

			logger.WriteEventWithSource("Error", eventSeverity: LogEventSeverity.Error);

			try
			{
				throw new System.IO.FileNotFoundException("File c:\\temp\\test.txt", "c:\\temp\\test.txt");
			}
			catch (Exception ex)
			{
				logger.WriteEvent(ex);
			}

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

		private static LogPolicy GetSimpleListPolicy(List<LogEvent> list)
		{
			return new LogPolicy()
			{
				LogWriter = new ListLogWriter(list, list.Capacity == 0 ? 10 : list.Capacity)
			};
		}

	}

	public class TestProperty
	{
		public string Name = "TestName";
		public int Sequence = 1;
	}
}
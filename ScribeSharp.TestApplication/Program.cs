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

			var props = new Dictionary<string, object>() { { "Test1", "Test2" }, { "Test2", new TestProperty() } };
			logger.WriteEvent("Test", properties: props);

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
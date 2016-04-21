using System.Collections.Generic;

namespace ScribeSharp
{
	public interface ILogWriter
	{
		void Write(LogEvent logEvent);

		bool RequiresSynchronisation { get; }
	}
}
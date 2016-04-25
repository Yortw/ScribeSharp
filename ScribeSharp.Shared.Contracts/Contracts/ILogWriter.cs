using System.Collections.Generic;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can output log events.
	/// </summary>
	public interface ILogWriter
	{
		/// <summary>
		/// Used to write a log event to the output locaton (screen, disk, network, memory etc).
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logEvent"/> is null.</exception>
		void Write(LogEvent logEvent);

		/// <summary>
		/// Returns a boolean indicating if this thread writer requires that only one thread accesses it at a time (true).
		/// </summary>
		bool RequiresSynchronization { get; }
	}
}
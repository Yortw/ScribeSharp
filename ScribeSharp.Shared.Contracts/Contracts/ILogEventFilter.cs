using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface used by components that can filter output or operations based on the content of a <see cref="LogEvent"/> instance.
	/// </summary>
	public interface ILogEventFilter
	{
		/// <summary>
		/// Returns a boolean indicating if the caller should output or act on <paramref name="logEvent"/>.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to analyse to decide of the output or operation should continue.</param>
		/// <returns>True if the log event should be processed/output, otherwise false.</returns>
		bool ShouldProcess(LogEvent logEvent);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components used to format a log event before outputting it to it's final destination.
	/// </summary>
	public interface ILogEventFormatter
	{
		/// <summary>
		/// Formats the <paramref name="logEvent"/> into a string which will be written to the final output location.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		/// <returns>A string containing the formatted data to output.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logEvent"/> is null.</exception>
		string Format(LogEvent logEvent);
	}
}
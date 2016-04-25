using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> instance to a json block.
	/// </summary>
	public sealed class JsonLogEventFormatter : ILogEventFormatter
	{
		/// <summary>
		/// Formats the <paramref name="logEvent"/> provided to a json string.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing a json representation of the <see cref="LogEvent"/>.</returns>
		public string Format(LogEvent logEvent)
		{
			//TODO: Serialise to json.
			throw new NotImplementedException();
		}
	}
}
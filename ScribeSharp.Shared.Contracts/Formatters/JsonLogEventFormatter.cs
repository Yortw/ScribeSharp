﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats a <see cref="LogEvent"/> instance to a json object.
	/// </summary>
	public sealed class JsonLogEventFormatter : LogEventFormatterBase
	{
		/// <summary>
		/// Writes the full log event as a Json object.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format and output.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			throw new NotImplementedException();
		}
	}
}
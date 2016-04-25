using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats <see cref="LogEvent"/> instances as XML blocks.
	/// </summary>
	public sealed class XmlLogEventFormatter : ILogEventFormatter
	{
		private static readonly System.Xml.Serialization.XmlSerializer _Serialiser = new System.Xml.Serialization.XmlSerializer(typeof(LogEvent));

		/// <summary>
		/// Returns a string containing an XML version of the <paramref name="logEvent"/> parameter.
		/// </summary>
		/// <param name="logEvent">The log event to format.</param>
		/// <returns>A string containing XML.</returns>
		public string Format(LogEvent logEvent)
		{
			using (var stream = new System.IO.MemoryStream())
			{
				_Serialiser.Serialize(stream, logEvent);
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				return System.Text.UTF8Encoding.UTF8.GetString(stream.GetBuffer());
			}
		}
	}
}
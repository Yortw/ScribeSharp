using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ScribeSharp.PropertyRenderers;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats <see cref="LogEvent"/> instances as XML blocks.
	/// </summary>
	public sealed class XmlLogEventFormatter : ILogEventFormatter
	{

		private static readonly PropertyRenderers.ToStringRenderer _AttributeValueRenderer = new ToStringRenderer();

		/// <summary>
		/// Returns a string containing an XML version of the <paramref name="logEvent"/> parameter.
		/// </summary>
		/// <param name="logEvent">The log event to format.</param>
		/// <returns>A string containing XML.</returns>
		public string Format(LogEvent logEvent)
		{
			var sb = new StringBuilder();
			using (var writer = System.Xml.XmlWriter.Create(sb, new System.Xml.XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
			{
				LogEventToXml(logEvent, writer);
				return sb.ToString();
			}
		}

		private void LogEventToXml(LogEvent logEvent, XmlWriter writer)
		{
			writer.WriteStartElement("LogEvent");

			WriteElementIfValueNotNull(writer, "EventName", logEvent.EventName);

			writer.WriteStartElement("Date");
			writer.WriteValue(logEvent.DateTime.ToLocalTime());
			writer.WriteEndElement();

			writer.WriteElementString("Severity", logEvent.EventSeverity.ToString());
			writer.WriteElementString("EventType", logEvent.EventType.ToString());

			WriteElementIfValueNotNull(writer, "Source", logEvent.Source);
			WriteElementIfValueNotNull(writer, "SourceMethod", logEvent.SourceMethod);

			writer.WriteStartElement("SourceLineNumber");
			writer.WriteValue(logEvent.SourceLineNumber);
			writer.WriteEndElement();

			if (logEvent.Exception != null)
				writer.WriteRaw(logEvent.Exception.ToXml());

			var properties = logEvent.Properties;
			if (properties != null)
			{
				foreach (var property in properties)
				{
					writer.WriteStartElement("Property");
					writer.WriteAttributeString("Key", property.Key);
					if (property.Value != null)
						writer.WriteAttributeString("Value", _AttributeValueRenderer.RenderValue(property.Value)?.ToString() ?? String.Empty);

					writer.WriteEndElement();
				}
			}
			
			writer.WriteEndElement();
			writer.Flush();
		}

		private void WriteElementIfValueNotNull(XmlWriter writer, string elementName, string value)
		{
			if (value == null) return;

			writer.WriteElementString(elementName, value);
		}
	}
}
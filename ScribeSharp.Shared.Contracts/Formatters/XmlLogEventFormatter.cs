using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ScribeSharp.PropertyRenderers;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Formats <see cref="LogEvent"/> instances as XML blocks.
	/// </summary>
	public sealed class XmlLogEventFormatter : LogEventFormatterBase
	{

		private static readonly PropertyRenderers.ToStringRenderer _AttributeValueRenderer = new ToStringRenderer();

		/// <summary>
		/// Default constructor.
		/// </summary>
		public XmlLogEventFormatter() : base()
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="exceptionRenderers">An <see cref="ITypeRendererMap"/> implementation used to locate renders used to format exceptions written to the log.</param>
		public XmlLogEventFormatter(ITypeRendererMap exceptionRenderers) : base(exceptionRenderers)
		{
		}

		/// <summary>
		/// Formats and outputs the log event to the specified writer as an XML node and attributes/child elements.
		/// </summary>
		/// <param name="logEvent">The log event to format and output.</param>
		/// <param name="writer">A <see cref="System.IO.TextWriter"/> to output to.</param>
		public override void FormatToTextWriter(LogEvent logEvent, TextWriter writer)
		{
			using (var xmlWriter = System.Xml.XmlWriter.Create(writer, new System.Xml.XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true, CloseOutput = false }))
			{
				LogEventToXml(logEvent, xmlWriter);
			}
		}

		private void LogEventToXml(LogEvent logEvent, XmlWriter writer)
		{
			writer.WriteStartElement("LogEvent");

			writer.WriteAttributeString("EventDate", logEvent.DateTime.ToString("O", System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteAttributeString("Severity", logEvent.EventSeverity.ToString());
			writer.WriteAttributeString("SeverityLevel", Convert.ToInt32(logEvent.EventSeverity, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteAttributeString("EventType", logEvent.EventType.ToString());

			WriteAttributeIfValueNotNull(writer, "Source", logEvent.Source);
			WriteAttributeIfValueNotNull(writer, "SourceMethod", logEvent.SourceMethod);
			if (logEvent.SourceLineNumber >= 0)
				WriteAttributeIfValueNotNull(writer, "SourceLineNumber", logEvent.SourceLineNumber.ToString(System.Globalization.CultureInfo.InvariantCulture));

			WriteElementIfValueNotNull(writer, "EventName", logEvent.EventName);

			if (logEvent.Exception != null)
			{
				if (!HasExceptionRenderers)
					writer.WriteValue(RenderException(logEvent.Exception));
				else
					writer.WriteRaw(logEvent.Exception.ToXml());
			}

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

		private static void WriteAttributeIfValueNotNull(XmlWriter writer, string attributeName, string attributeValue)
		{
			if (attributeValue == null) return;

			writer.WriteAttributeString(attributeName, attributeValue);
		}

		private static void WriteElementIfValueNotNull(XmlWriter writer, string elementName, string value)
		{
			if (value == null) return;

			writer.WriteElementString(elementName, value);
		}
	}
}
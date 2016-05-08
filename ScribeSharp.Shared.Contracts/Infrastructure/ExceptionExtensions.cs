using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Extensions to the <see cref="System.Exception"/> type.
	/// </summary>
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Returns a string containing an XML representation of the exception.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static string ToXml(this Exception exception)
		{
			var sb = new StringBuilder();
			using (var writer = System.Xml.XmlWriter.Create(sb, new System.Xml.XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
			{
				ExceptionToXml(exception, writer);
				return sb.ToString();
			}
		}

		private static void ExceptionToXml(Exception ex, System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Exception");

			writer.WriteAttributeString("Type", ex.GetType().FullName);
			if (ex.Message != null)
				writer.WriteElementString("Message", ex.Message);

			if (ex.Source != null)
				writer.WriteElementString("Source", ex.Source);

			if (ex.StackTrace != null)
				writer.WriteElementString("StackTrace", ex.StackTrace.ToString());

			if (ex.TargetSite != null)
				writer.WriteElementString("TargetSite", ex.TargetSite.ToString());

			if (ex.Data != null)
			{
				foreach (System.Collections.DictionaryEntry item in ex.Data)
				{
					if (item.Key != null && item.Value != null)
					{
						writer.WriteStartElement("Data");
						writer.WriteAttributeString("Key", item.Key.ToString());
						if (item.Value != null)
							writer.WriteAttributeString("Value", item.Value.ToString());
						writer.WriteEndElement();
					}
				}
			}

			foreach (var prop in ex.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
			{
				if (prop.Name != "Message" && prop.Name != "StackTrace" && prop.Name != "Source" && prop.Name != "Data" && prop.Name != "TargetSite")
				{
					var propValue = prop.GetValue(ex, null);
					if (propValue != null)
					{
						writer.WriteStartElement("Property");
						writer.WriteAttributeString("Name", prop.Name);
						writer.WriteAttributeString("Value", propValue.ToString());
						writer.WriteEndElement();
					}
				}
			}

			writer.WriteElementString("FullDetails", ex.ToString());

			if (ex.InnerException != null)
				ExceptionToXml(ex.InnerException, writer);

			writer.WriteEndElement();
			writer.Flush();
		}

		/// <summary>
		/// Returns true if the exception is of a type that indicates a serious system flaw, one that has likely resulted in unknown process state and should not attempt to be handled at all.
		/// </summary>
		/// <param name="exception">The exception to test.</param>
		/// <returns>True if the exception should be rethrown immediately, else false.</returns>
		public static bool ShouldRethrowImmediately(this Exception exception)
		{
			return exception is StackOverflowException
				|| exception is System.Threading.ThreadAbortException
				|| exception is AccessViolationException
				|| exception is OutOfMemoryException;
		}
	}
}
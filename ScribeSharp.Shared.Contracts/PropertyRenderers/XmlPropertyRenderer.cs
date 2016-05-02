using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Renders a type as an XML string.
	/// </summary>
  public class XmlPropertyRenderer : IPropertyRenderer
	{
		private System.Xml.Serialization.XmlSerializer _Serialiser;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="sourceType">The type to be rendered.</param>
		public XmlPropertyRenderer(Type sourceType)
		{			
			_Serialiser = new System.Xml.Serialization.XmlSerializer(sourceType);
		}

		/// <summary>
		/// Converts the provided object to an XML string.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A string containing an XML representation of <paramref name="value"/>.</returns>
		public object RenderValue(object value)
		{
			if (value == null) return null;

			using (var ms = new System.IO.MemoryStream())
			{
				_Serialiser.Serialize(ms, value);
				ms.Seek(0, System.IO.SeekOrigin.Begin);
				return System.Text.UTF8Encoding.UTF8.GetString(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
			}
		}
	}
}
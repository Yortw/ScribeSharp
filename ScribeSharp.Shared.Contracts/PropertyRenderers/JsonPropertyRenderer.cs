using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Formats a property value as a Json string.
	/// </summary>
	public class JsonPropertyRenderer : IPropertyRenderer
	{
		private JsonSerializer _Serialiser = new JsonSerializer();

		/// <summary>
		/// Renders the value to a Json string. If the value is null, null is returned.
		/// </summary>
		/// <param name="value">The value to render.</param>
		/// <returns>Either null, or a string containing a formatted Json version of <paramref name="value"/>.</returns>
		public object RenderValue(object value)
		{
			if (value == null) return null;

			using (var pooledWriter = Globals.TextWriterPool.Take())
			{
				using (var jsonWriter = new JsonTextWriter(pooledWriter.Value))
				{
					_Serialiser.Serialize(jsonWriter, value);
					jsonWriter.Flush();

					return pooledWriter.Value.GetText();
				}
			}
		}
	}
}
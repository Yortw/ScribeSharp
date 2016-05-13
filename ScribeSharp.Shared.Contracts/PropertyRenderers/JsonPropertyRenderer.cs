using ScribeSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Formats a property value as a Json string.
	/// </summary>
	public sealed class JsonPropertyRenderer : IPropertyRenderer
	{
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
				using (var jsonWriter = new JsonWriter(pooledWriter.Value, false))
				{
					jsonWriter.WriteJsonObject(value);
					jsonWriter.Flush();

					return pooledWriter.Value.GetText();
				}
			}
		}
	}
}
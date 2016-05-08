using System;
using System.Collections.Generic;
using System.Text;
using ScribeSharp;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Renders an exception to a string containing an XML node.
	/// </summary>
	public sealed class ExceptionAsXmlRenderer : IPropertyRenderer
	{
		/// <summary>
		/// Returns a string containing an XML representation of the provided an exception.
		/// </summary>
		/// <param name="value">A object inheriting from <see cref="System.Exception"/> that will be rendered to XML.</param>
		/// <returns>A string containing XML.</returns>
		public object RenderValue(object value)
		{
			return ((Exception)value).ToXml();
		}
	}
}
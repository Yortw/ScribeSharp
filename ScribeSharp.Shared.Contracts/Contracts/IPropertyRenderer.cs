using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can convert properties to an alternate format for outputting.
	/// </summary>
	public interface IPropertyRenderer
	{
		/// <summary>
		/// Renders the specified value to an alternate form.
		/// </summary>
		/// <param name="value">The value to render.</param>
		/// <returns>The rendered version of <paramref name="value"/>.</returns>
		object RenderValue(object value);
	}
}
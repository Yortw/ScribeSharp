
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Renders an object by calling it's ToString method.
	/// </summary>
	public sealed class ToStringRenderer : IPropertyRenderer
	{
		private string _FormatString;
		private System.IFormatProvider _FormatProvider;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ToStringRenderer()
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="formatString">A string to pass to the ToString method to control the format of the output string.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public ToStringRenderer(string formatString) : this(formatString, System.Globalization.CultureInfo.InvariantCulture)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="formatProvider">A <see cref="IFormatProvider"/> implementation used to format the property to a string.</param>
		/// <param name="formatString">A string to pass to the ToString method to control the format of the output string.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public ToStringRenderer(string formatString, System.IFormatProvider formatProvider)
		{
			_FormatString = formatString;
			_FormatProvider = formatProvider;
		}

		/// <summary>
		/// Returns either value.ToString(), value.ToString(_FormatString), value.ToString(_FormatProvider) or value.ToString(_FormatString, _FormatProvider) depending on what the type supports.
		/// </summary>
		/// <param name="value">The value to format.</param>
		/// <returns>Returns a string containing the formatted value.</returns>
		public object RenderValue(object value)
		{
			if (value == null) return null;

			if (!String.IsNullOrEmpty(_FormatString))
			{
				var formattable = value as IFormattable;
				if (formattable != null)
					return formattable.ToString(_FormatString, _FormatProvider);
				else
				{
					if (_FormatProvider != null)
						return System.Convert.ToString(value, _FormatProvider);
					else
						return value.ToString();
				}
			}
			else
				return value.ToString();
		}
	}
}
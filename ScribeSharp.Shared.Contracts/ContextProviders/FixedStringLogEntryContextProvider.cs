using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Applies a property with a specified name and string value.
	/// </summary>
	public sealed class FixedStringLogEntryContextProvider : ContextProviderBase
	{

		private string _PropertyName;
		private string _PropertyValue;

		/// <summary>
		/// Applies a property with the specified name and string value.
		/// </summary>
		/// <param name="propertyName">The name of the property to apply.</param>
		/// <param name="propertyValue">The value of the property to apply.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="propertyName"/> is empty or only whitespace.</exception>
		public FixedStringLogEntryContextProvider(string propertyName, string propertyValue) : this(propertyName, propertyValue, null)
		{
		}

		/// <summary>
		/// Applies a property with the specified name and string value.
		/// </summary>
		/// <param name="propertyName">The name of the property to apply.</param>
		/// <param name="propertyValue">The value of the property to apply.</param>
		/// <param name="filter">An optional <see cref="ILogEventFilter"/> used to decide if this property should be applied or not.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="propertyName"/> is empty or only whitespace.</exception>
		public FixedStringLogEntryContextProvider(string propertyName, string propertyValue, ILogEventFilter filter) : base(filter)
		{
			if (_PropertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(propertyName)));

			_PropertyName = propertyName;
			_PropertyValue = propertyValue;
		}

		/// <summary>
		/// Returns the property name provided via the constructor.
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return _PropertyName;
			}
		}

		/// <summary>
		/// Returns the property value provided via the constructor.
		/// </summary>
		public override object GetValue()
		{
			return _PropertyValue;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Applies a property with a specified name and string value.
	/// </summary>
	public sealed class FixedValueLogEntryContextProvider : ContextProviderBase
	{

		private string _PropertyName;
		private object _PropertyValue;

		/// <summary>
		/// Applies a property with the specified name and string value.
		/// </summary>
		/// <param name="propertyName">The name of the property to apply.</param>
		/// <param name="propertyValue">The value of the property to apply.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="propertyName"/> is empty or only whitespace.</exception>
		public FixedValueLogEntryContextProvider(string propertyName, object propertyValue) : this(propertyName, propertyValue, null)
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
		public FixedValueLogEntryContextProvider(string propertyName, object propertyValue, ILogEventFilter filter) : base(filter)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(propertyName)));

			_PropertyName = propertyName;
			_PropertyValue = propertyValue;
		}

		/// <summary>
		/// Adds a property with the name and the value supplied via the constructor.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, _PropertyName, _PropertyValue, rendererMap);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// A generic context provider that uses a delegate to provide the property value.
	/// </summary>
	public sealed class DelegateLogEventContextProvider : ContextProviderBase
	{
		private readonly string _PropertyName;
		private readonly Func<object> _RequestContextValueCallback;

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property to add.</param>
		/// <param name="requestContextValueCallback">A <see cref="Func{TResult}"/> to call to obtain the property value.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> or <paramref name="requestContextValueCallback"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="propertyName"/> is empty or only whitespace.</exception>
		public DelegateLogEventContextProvider(string propertyName, Func<object> requestContextValueCallback) : this(propertyName, requestContextValueCallback, null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property to add.</param>
		/// <param name="requestContextValueCallback">A <see cref="Func{TResult}"/> to call to obtain the property value.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> or <paramref name="requestContextValueCallback"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="propertyName"/> is empty or only whitespace.</exception>
		public DelegateLogEventContextProvider(string propertyName, Func<object> requestContextValueCallback, ILogEventFilter filter) : base(filter)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("propertyName cannot be null, empty or whitespace.", nameof(propertyName));
			if (requestContextValueCallback == null) throw new ArgumentNullException(nameof(requestContextValueCallback));

			_PropertyName = propertyName;
			_RequestContextValueCallback = requestContextValueCallback;
		}

		/// <summary>
		/// Adds a property with the name and the value of the function supplied via the constructor.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			var value = _RequestContextValueCallback();
			if (value != null)
				AddProperty(logEvent.Properties, _PropertyName, value, rendererMap);
		}
	}
}
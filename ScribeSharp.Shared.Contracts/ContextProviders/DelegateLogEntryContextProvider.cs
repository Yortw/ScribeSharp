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
		/// Returns the property name configured via the constructor.
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return _PropertyName;
			}
		}

		/// <summary>
		/// Calls the function provided in the constructor and returns it's value.
		/// </summary>
		/// <returns>The value of the configured function.</returns>
		public override object GetValue()
		{
			return _RequestContextValueCallback();
		}
	}
}
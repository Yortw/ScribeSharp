using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class DelegateLogEventContextProvider : ContextProviderBase
	{
		private readonly string _PropertyName;
		private readonly Func<object> _RequestContextValueCallback;

		public DelegateLogEventContextProvider(string propertyName, Func<object> requestContextValueCallback) : this(propertyName, requestContextValueCallback, null)
		{
		}

		public DelegateLogEventContextProvider(string propertyName, Func<object> requestContextValueCallback, ILogEventFilter filter) : base(filter)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("propertyName cannot be null, empty or whitespace.", nameof(propertyName));
			if (requestContextValueCallback == null) throw new ArgumentNullException(nameof(requestContextValueCallback));

			_PropertyName = propertyName;
			_RequestContextValueCallback = requestContextValueCallback;
		}

		public override string PropertyName
		{
			get
			{
				return _PropertyName;
			}
		}

		public override object GetValue()
		{
			return _RequestContextValueCallback();
		}
	}
}
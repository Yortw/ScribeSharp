using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public abstract class ContextProviderBase : ILogEventContextProvider
	{

		private readonly ILogEventFilter _Filter;

		protected ContextProviderBase()
		{
		}

		protected ContextProviderBase(ILogEventFilter filter)
		{
			_Filter = filter;
		}

		public ILogEventFilter Filter
		{
			get
			{
				return _Filter;
			}
		}

		public abstract string PropertyName
		{
			get;
		}

		public abstract object GetValue();
	}
}
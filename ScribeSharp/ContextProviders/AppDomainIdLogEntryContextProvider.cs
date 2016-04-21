using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class AppDomainIdLogEventContextProvider : ContextProviderBase
	{
		public AppDomainIdLogEventContextProvider() : base() { }
		public AppDomainIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "AppDomain Id";
			}
		}

		public override object GetValue()
		{
			return AppDomain.CurrentDomain.Id;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class AppDomainNameLogEventContextProvider : ContextProviderBase
	{

		public AppDomainNameLogEventContextProvider() : base() { }
		public AppDomainNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "AppDomain Name";
			}
		}

		public override object GetValue()
		{
			return AppDomain.CurrentDomain.FriendlyName;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class OsUserNameLogEventContextProvider : ContextProviderBase
	{
		public OsUserNameLogEventContextProvider() : base() { }
		public OsUserNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "OS User";
			}
		}

		public override object GetValue()
		{
			return (Environment.UserDomainName ?? String.Empty) + "\\" + Environment.UserName;
		}
	}
}
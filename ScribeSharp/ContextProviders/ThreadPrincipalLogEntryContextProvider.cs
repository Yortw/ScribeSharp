using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ThreadPrincipalLogEventContextProvider : ContextProviderBase
	{
		public ThreadPrincipalLogEventContextProvider() : base() { }
		public ThreadPrincipalLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Thread Principal";
			}
		}

		public override object GetValue()
		{
			return System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}
	}
}
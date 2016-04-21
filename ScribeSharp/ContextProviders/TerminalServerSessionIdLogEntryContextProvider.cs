using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class TerminalServerSessionIdLogEventContextProvider : ContextProviderBase
	{

		public TerminalServerSessionIdLogEventContextProvider() : base() { }
		public TerminalServerSessionIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Terminal Server Session Id";
			}
		}

		public override object GetValue()
		{
			return System.Diagnostics.Process.GetCurrentProcess().SessionId;
		}
	}
}
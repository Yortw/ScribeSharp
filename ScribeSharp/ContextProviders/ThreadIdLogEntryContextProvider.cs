using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ThreadIdLogEventContextProvider : ContextProviderBase
	{

		public ThreadIdLogEventContextProvider() : base() { }
		public ThreadIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Thread Id";
			}
		}

		public override object GetValue()
		{
			return System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
	}
}
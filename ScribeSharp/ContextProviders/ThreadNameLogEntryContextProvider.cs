using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ThreadNameLogEventContextProvider : ContextProviderBase
	{

		public ThreadNameLogEventContextProvider() : base() { }
		public ThreadNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Thread Name";
			}
		}

		public override object GetValue()
		{
			var currentThread = System.Threading.Thread.CurrentThread;
			return String.IsNullOrWhiteSpace(currentThread.Name) ? currentThread.ManagedThreadId.ToString() : currentThread.Name;
		}
	}
}
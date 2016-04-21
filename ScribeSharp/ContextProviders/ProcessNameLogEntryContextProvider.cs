using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ProcessNameLogEventContextProvider : ContextProviderBase
	{

		private string _ProcessName;

		public ProcessNameLogEventContextProvider() : base() { }
		public ProcessNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Process Name";
			}
		}

		public override object GetValue()
		{
			return _ProcessName ?? (_ProcessName = CachedCurrentProcess.CurrentProcess.MachineName);
		}
	}
}
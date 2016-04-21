using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ProcessIdLogEventContextProvider : ContextProviderBase
	{

		private int? _ProcessId;

		public ProcessIdLogEventContextProvider() : base() { }
		public ProcessIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Process Id";
			}
		}

		public override object GetValue()
		{
			return _ProcessId ?? (_ProcessId = CachedCurrentProcess.CurrentProcess.Id);
		}
	}
}
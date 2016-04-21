using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class MachineNameLogEventContextProvider : ContextProviderBase
	{
		public MachineNameLogEventContextProvider() : base() { }
		public MachineNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Machine Name";
			}
		}

		public override object GetValue()
		{
			return Environment.MachineName;
		}
	}
}
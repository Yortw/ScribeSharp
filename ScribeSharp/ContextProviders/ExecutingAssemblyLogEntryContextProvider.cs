using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class CallingAssemblyLogEventContextProvider : ContextProviderBase
	{
		public CallingAssemblyLogEventContextProvider() : base() { }
		public CallingAssemblyLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Assembly Name";
			}
		}

		public override object GetValue()
		{
			return Assembly.GetCallingAssembly().FullName;
		}
	}
}
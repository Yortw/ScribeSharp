using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class EntryAssemblyLogEventContextProvider : ContextProviderBase
	{

		private string _AssemblyName;

		public EntryAssemblyLogEventContextProvider() : base() { }
		public EntryAssemblyLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Entry Assembly Name";
			}
		}

		public override object GetValue()
		{
			return _AssemblyName ?? (_AssemblyName = Assembly.GetEntryAssembly().FullName);
		}
	}
}
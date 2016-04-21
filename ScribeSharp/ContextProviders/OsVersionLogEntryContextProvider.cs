using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class OsVersionLogEventContextProvider : ContextProviderBase
	{
		public OsVersionLogEventContextProvider() : base() { }
		public OsVersionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "OS Version";
			}
		}

		public override object GetValue()
		{
			return Environment.OSVersion.Version.ToString();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class OsVersionDescriptionLogEventContextProvider : ContextProviderBase
	{
		public OsVersionDescriptionLogEventContextProvider() : base() { }
		public OsVersionDescriptionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "OS Version Description";
			}
		}

		public override object GetValue()
		{
			return Environment.OSVersion.VersionString;
		}
	}
}
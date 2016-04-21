using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ApplicationNameLogEventContextProvider : ContextProviderBase
	{

		public ApplicationNameLogEventContextProvider() : base() { }
		public ApplicationNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "Application Name";
			}
		}

		public override object GetValue()
		{
			return AppDomain.CurrentDomain.ApplicationIdentity.FullName;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	public sealed class ClrVersionLogEventContextProvider : ContextProviderBase
	{
		private string _Version;

		public ClrVersionLogEventContextProvider() : base() { }
		public ClrVersionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		public override string PropertyName
		{
			get
			{
				return "CLR Version";
			}
		}

		public override object GetValue()
		{
			return _Version ?? (_Version = Environment.Version.ToString());
		}
	}
}
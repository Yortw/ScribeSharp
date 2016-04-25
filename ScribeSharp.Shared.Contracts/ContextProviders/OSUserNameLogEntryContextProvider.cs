using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a property called "OS User" containing the name of the current user according to the OS.
	/// </summary>
	public sealed class OSUserNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public OSUserNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public OSUserNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "OS User".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "OS User";
			}
		}

		/// <summary>
		/// Returns a string containing the domain (if any) and user name seperated by \.
		/// </summary>
		/// <returns>A string containing the domain and user name of the OS user.</returns>
		public override object GetValue()
		{
			return (Environment.UserDomainName ?? String.Empty) + "\\" + Environment.UserName;
		}
	}
}
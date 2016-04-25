using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds an "AppDomain Name" property containing the AppDomain.CurrentDomain.FriendlyName value.
	/// </summary>
	public sealed class AppDomainNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AppDomainNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public AppDomainNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "AppDomain Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "AppDomain Name";
			}
		}

		/// <summary>
		/// Returns AppDomain.CurrentDomain.FriendlyName.
		/// </summary>
		/// <returns>Returns the friendly name of the current app domain.</returns>
		public override object GetValue()
		{
			return AppDomain.CurrentDomain.FriendlyName;
		}
	}
}
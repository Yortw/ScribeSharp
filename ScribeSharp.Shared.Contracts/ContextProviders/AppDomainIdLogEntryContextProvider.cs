using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds an "AppDomain Id" property containing the AppDomain.CurrentDomain.Id value.
	/// </summary>
	public sealed class AppDomainIdLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AppDomainIdLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public AppDomainIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "AppDomain Id".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "AppDomain Id";
			}
		}

		/// <summary>
		/// Returns AppDomain.CurrentDomain.Id.
		/// </summary>
		/// <returns>The id of the current app domain.</returns>
		public override object GetValue()
		{
			return AppDomain.CurrentDomain.Id;
		}
	}
}
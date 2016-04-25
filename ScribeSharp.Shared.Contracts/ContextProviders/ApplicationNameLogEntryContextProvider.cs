using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Application Name" property.
	/// </summary>
	public sealed class ApplicationNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ApplicationNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ApplicationNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Application Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Application Name";
			}
		}

		/// <summary>
		/// Returns AppDomain.CurrentDomain.ApplicationIdentity.FullName.
		/// </summary>
		/// <returns>Returns the full name of the application.</returns>
		public override object GetValue()
		{
			return AppDomain.CurrentDomain.ApplicationIdentity.FullName;
		}
	}
}
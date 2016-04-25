using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a property called "Thread Principal" containing the name of the current principal identity for the current thread.
	/// </summary>
	public sealed class ThreadPrincipalLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ThreadPrincipalLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ThreadPrincipalLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Return "Thread Principal".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Thread Principal";
			}
		}

		/// <summary>
		/// The name of the current principal identity for the current thread.
		/// </summary>
		/// <returns>A string containing the name of the current principal identity for the current thread.</returns>
		public override object GetValue()
		{
			return System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a "Terminal Server Session Id" property containing the id of the terminal server session the current process is running under.
	/// </summary>
	public sealed class TerminalServerSessionIdLogEventContextProvider : ContextProviderBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TerminalServerSessionIdLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public TerminalServerSessionIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Terminal Server Session Id".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Terminal Server Session Id";
			}
		}

		/// <summary>
		/// Returns the current terminal server session id.
		/// </summary>
		/// <returns>A string containing the current terminal server session id.</returns>
		public override object GetValue()
		{
			return System.Diagnostics.Process.GetCurrentProcess().SessionId;
		}
	}
}
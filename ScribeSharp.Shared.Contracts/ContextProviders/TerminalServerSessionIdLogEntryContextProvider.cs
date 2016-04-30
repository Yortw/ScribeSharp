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
		/// Adds a property with the name "Terminal Server Session Id" and the value of System.Diagnostics.Process.GetCurrentProcess().SessionId.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "Terminal Server Session Id", CachedCurrentProcess.CurrentProcess.SessionId);
		}
	}
}
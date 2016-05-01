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
		/// Adds a property with the name "Thread Principal" and the value of System.Threading.Thread.CurrentPrincipal.Identity.Name.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "Thread Principal", System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
	}
}
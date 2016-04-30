using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a property called "Thread Name" containing the name of the current thread, or if no name is assigned then the current managed thread id.
	/// </summary>
	public sealed class ThreadNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ThreadNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ThreadNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds a property with the name "Thread Name" and either the name or the id of the current managed thread.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			var currentThread = System.Threading.Thread.CurrentThread;
			AddProperty(logEvent.Properties, "Thread Name", String.IsNullOrWhiteSpace(currentThread.Name) ? currentThread.ManagedThreadId.ToString(System.Globalization.CultureInfo.InvariantCulture) : currentThread.Name);
		}
	}
}
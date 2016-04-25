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
		/// Returns "Thread Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Thread Name";
			}
		}

		/// <summary>
		/// The name of the current thread, or if no name is assigned then the current managed thread id.
		/// </summary>
		/// <returns>A string containing the thread name.</returns>
		public override object GetValue()
		{
			var currentThread = System.Threading.Thread.CurrentThread;
			return String.IsNullOrWhiteSpace(currentThread.Name) ? currentThread.ManagedThreadId.ToString(System.Globalization.CultureInfo.InvariantCulture) : currentThread.Name;
		}
	}
}
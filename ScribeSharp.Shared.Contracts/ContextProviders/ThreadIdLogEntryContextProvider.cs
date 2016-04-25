using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Thread Id" property containing the id of the current managed thread.
	/// </summary>
	public sealed class ThreadIdLogEventContextProvider : ContextProviderBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ThreadIdLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ThreadIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Thread Id".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Thread Id";
			}
		}

		/// <summary>
		/// Returns the id of the current managed thread.
		/// </summary>
		/// <returns>A string containing the current managed thread id.</returns>
		public override object GetValue()
		{
			return System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
	}
}
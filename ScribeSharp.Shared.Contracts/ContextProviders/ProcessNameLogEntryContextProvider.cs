using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Process Name" property containing the name of the process the logger is running in.
	/// </summary>
	public sealed class ProcessNameLogEventContextProvider : ContextProviderBase
	{

		private string _ProcessName;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ProcessNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ProcessNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Process Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Process Name";
			}
		}

		/// <summary>
		/// Returns the current process name.
		/// </summary>
		/// <returns>A string containing the current process name.</returns>
		public override object GetValue()
		{
			return _ProcessName ?? (_ProcessName = CachedCurrentProcess.CurrentProcess.MachineName);
		}
	}
}
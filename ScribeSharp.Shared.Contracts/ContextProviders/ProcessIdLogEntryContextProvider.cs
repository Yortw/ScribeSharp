using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Process Id" property containing the unique ID of the process the logger is running in.
	/// </summary>
	public sealed class ProcessIdLogEventContextProvider : ContextProviderBase
	{

		private int? _ProcessId;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ProcessIdLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ProcessIdLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Process Id".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Process Id";
			}
		}

		/// <summary>
		/// Returns a string containing the id of the current process.
		/// </summary>
		/// <returns>A string containing the id of the current process.</returns>
		public override object GetValue()
		{
			return _ProcessId ?? (_ProcessId = CachedCurrentProcess.CurrentProcess.Id);
		}
	}
}
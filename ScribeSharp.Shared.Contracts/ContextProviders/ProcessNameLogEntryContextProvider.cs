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
		/// Adds a property with the name "Process Name" and the value of System.Diagnostics.Process.GetCurrentProcess().ProcessName.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "Process Name", _ProcessName ?? (_ProcessName = CachedCurrentProcess.CurrentProcess.ProcessName));
		}
	}
}
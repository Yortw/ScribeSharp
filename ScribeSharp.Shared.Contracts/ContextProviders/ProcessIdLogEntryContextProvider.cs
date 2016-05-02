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
		/// Adds a property with the name "Process Id" and the value of Environment.OSVersion.Version converted to a string.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, "Process Id", _ProcessId ?? (_ProcessId = CachedCurrentProcess.CurrentProcess.Id), rendererMap);
		}
	}
}
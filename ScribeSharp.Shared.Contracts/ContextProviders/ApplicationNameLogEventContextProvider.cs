#if SUPPORTS_PROCESS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Application Name" property.
	/// </summary>
	public sealed class ApplicationNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ApplicationNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ApplicationNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds an "Application Name" property containing the AppDomain.CurrentDomain.ApplicationIdentity.FullName value.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, 
				"Application Name", 
				AppDomain.CurrentDomain?.ApplicationIdentity?.FullName ?? CachedCurrentProcess.CurrentProcess.ProcessName, 
				rendererMap);
		}
	}
}

#endif
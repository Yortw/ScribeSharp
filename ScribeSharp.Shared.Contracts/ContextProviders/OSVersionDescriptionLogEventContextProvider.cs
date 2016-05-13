#if SUPPORTS_OSUSER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a property called "OS Version Description" containing the human readable name of the OS, i.e Windows 7 SP1.
	/// </summary>
	public sealed class OSVersionDescriptionLogEventContextProvider : ContextProviderBase
	{

		private string _OSVersion;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public OSVersionDescriptionLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public OSVersionDescriptionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds a property with the name "OS Version Description" and the value of Environment.OSVersion.VersionString.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, "OS Version Description", _OSVersion ?? (_OSVersion = Environment.OSVersion.VersionString), rendererMap);
		}
	}
}

#endif
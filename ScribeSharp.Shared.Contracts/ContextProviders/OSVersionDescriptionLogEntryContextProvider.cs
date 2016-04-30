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
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "OS Version Description", (_OSVersion = Environment.OSVersion.VersionString));
		}
	}
}
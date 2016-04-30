using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds an "OS Version" property containing the raw OS version number.
	/// </summary>
	public sealed class OSVersionLogEventContextProvider : ContextProviderBase
	{

		private string _OSVersion;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public OSVersionLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public OSVersionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds a property with the name "OS Version" and the value of Environment.OSVersion.Version converted to a string.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "OS Version", (_OSVersion = Environment.OSVersion.Version.ToString()));
		}
	}
}
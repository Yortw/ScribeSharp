using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "CLR Version" property.
	/// </summary>
	public sealed class ClrVersionLogEventContextProvider : ContextProviderBase
	{
		private string _Version;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ClrVersionLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ClrVersionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds an "CLR Version" property containing the Environment.Version value.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		protected override void AddPropertiesCore(LogEvent logEvent)
		{
			AddProperty(logEvent.Properties, "CLR Version", _Version ?? (_Version = Environment.Version.ToString()));
		}
	}
}
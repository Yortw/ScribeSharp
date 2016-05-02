using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds the name of the assembly that the app started from.
	/// </summary>
	public sealed class EntryAssemblyLogEventContextProvider : ContextProviderBase
	{

		private string _AssemblyName;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EntryAssemblyLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public EntryAssemblyLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds a property with the "Entry Assembly Name" and the value of Assembly.GetEntryAssembly().FullName.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, "Entry Assembly Name", _AssemblyName ?? (_AssemblyName = Assembly.GetEntryAssembly().FullName), rendererMap);
		}
	}
}
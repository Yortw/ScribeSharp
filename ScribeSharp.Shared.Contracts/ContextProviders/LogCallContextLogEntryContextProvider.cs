using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds all properties currently in the <see cref="LogCallContext.CurrentProperties"/>.
	/// </summary>
	public sealed class LogCallContextLogEntryContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogCallContextLogEntryContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public LogCallContextLogEntryContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds all properties currently in the <see cref="LogCallContext.CurrentProperties"/>.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			foreach (var property in LogCallContext.CurrentProperties)
			{
				AddProperty(logEvent.Properties, property.Key, property.Value, rendererMap);
			}
		}

	}
}
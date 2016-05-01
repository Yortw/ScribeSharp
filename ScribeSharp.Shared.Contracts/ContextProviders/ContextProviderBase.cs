using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// A base class for context providers containing common logic.
	/// </summary>
	public abstract class ContextProviderBase : ILogEventContextProvider
	{

		private readonly ILogEventFilter _Filter;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ContextProviderBase()
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		protected ContextProviderBase(ILogEventFilter filter)
		{
			_Filter = filter;
		}

		/// <summary>
		/// Returns the <see cref="ILogEventFilter"/> instance to use when deciding whether to apply context or not.
		/// </summary>
		public ILogEventFilter Filter
		{
			get
			{
				return _Filter;
			}
		}

		/// <summary>
		/// Called by the system to ask this context provider to add any properties it wants to apply.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> to add properties to.</param>
		public void AddProperties(LogEvent logEvent)
		{
			if ((_Filter?.ShouldProcess(logEvent) ?? true))
				AddPropertiesCore(logEvent);
		}

		/// <summary>
		/// Called when a log event requires properties added and the filter provided via the constructor has accepted the log event.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> to apply properties to.</param>
		protected abstract void AddPropertiesCore(LogEvent logEvent);

		/// <summary>
		/// Adds the property to the specified dictionary if a property with the same name doesn't already exist.
		/// </summary>
		/// <param name="properties">The dictionary to add to.</param>
		/// <param name="propertyName">The name of the property to add.</param>
		/// <param name="value">The value of the property to add.</param>
		protected static void AddProperty(IDictionary<string, object> properties, string propertyName, object value)
		{
			//if (_HasPropertyRenderers)
			//	properties[propertyName] = RenderProperty(value);
			//else
				properties.AddIfNotExists(propertyName, value);
		}
	}
}
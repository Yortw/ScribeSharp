using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a property called 'Event Instance ID' containing a new <see cref="System.Guid"/>.
	/// </summary>
	public sealed class GuidEventInstanceIdLogEntryContextProvider : ContextProviderBase
	{

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GuidEventInstanceIdLogEntryContextProvider() : this(null) { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public GuidEventInstanceIdLogEntryContextProvider(ILogEventFilter filter) : base(filter) { }

		#endregion

		#region Overrides

		/// <summary>
		/// Adds a property called 'Event Instance ID' containing a new <see cref="System.Guid"/>.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, "Event Instance ID", System.Guid.NewGuid(), rendererMap);
		}

		#endregion

	}
}
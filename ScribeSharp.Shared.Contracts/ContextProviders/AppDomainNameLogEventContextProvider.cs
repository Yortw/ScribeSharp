﻿#if SUPPORTS_APPDOMAINS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds an "AppDomain Name" property containing the AppDomain.CurrentDomain.FriendlyName value.
	/// </summary>
	public sealed class AppDomainNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AppDomainNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public AppDomainNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds an "AppDomain Name" property containing the AppDomain.CurrentDomain.FriendlyName value.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, "AppDomain Name", AppDomain.CurrentDomain.FriendlyName, rendererMap);
		}
	}
}

#endif
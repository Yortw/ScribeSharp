using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds an "Stacktrace" property containing the current stack trace as a string.
	/// </summary>
	/// <remarks>
	/// <para>Warning: Creating the stacktrace is an expensive operation, this provider should be used with a filter so it only applies on infrequent events.
	/// Also note that in optimized (release) builds the stack trace may be incomplete due to compiler optimizations.</para>
	/// </remarks>
	public sealed class StackTraceLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public StackTraceLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public StackTraceLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Adds a property with the name "Stacktrace" and the value of System.Diagnostics.StackTrace(true).ToString().
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
#if NETFX_CORE
			var stacktrace = new System.Diagnostics.StackTrace(new Exception(), true);
#else
			var stacktrace = new System.Diagnostics.StackTrace(true);
#endif
			AddProperty(logEvent.Properties, "Stacktrace", stacktrace.ToString(), rendererMap);
		}
	}
}
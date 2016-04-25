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
	public sealed class StackTraceLogEntryContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public StackTraceLogEntryContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public StackTraceLogEntryContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Stacktrace".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Stacktrace";
			}
		}

		/// <summary>
		/// Returns System.Diagnostics.StackTrace(true).ToString().
		/// </summary>
		/// <returns>The string representation of the current stack trace.</returns>
		public override object GetValue()
		{
			return new System.Diagnostics.StackTrace(true).ToString();
		}
	}
}
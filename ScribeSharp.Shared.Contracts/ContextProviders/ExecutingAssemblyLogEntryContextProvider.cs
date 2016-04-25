using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "Calling Assembly" property containing the name of the assembly of the method that invoked the log action.
	/// </summary>
	public sealed class CallingAssemblyLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CallingAssemblyLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public CallingAssemblyLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Calling Assembly".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Calling Assembly";
			}
		}

		/// <summary>
		/// Returns Assembly.GetCallingAssembly().FullName.
		/// </summary>
		/// <returns>A string containing the calling assembly name.</returns>
		public override object GetValue()
		{
			return Assembly.GetCallingAssembly().FullName;
		}
	}
}
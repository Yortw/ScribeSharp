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
		/// Returns "OS Version".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "OS Version";
			}
		}

		/// <summary>
		/// Returns <see cref="Environment.OSVersion"/> converted to a string.
		/// </summary>
		/// <returns>A string containing the OS version number as a string.</returns>
		public override object GetValue()
		{
			return Environment.OSVersion.Version.ToString();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Sets a property called "OS Version Description" containing the human readable name of the OS, i.e Windows 7 SP1.
	/// </summary>
	public sealed class OSVersionDescriptionLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public OSVersionDescriptionLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public OSVersionDescriptionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "OS Version Description".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "OS Version Description";
			}
		}

		/// <summary>
		/// Returns <see cref="Environment.OSVersion"/>.
		/// </summary>
		/// <returns>A string containing the human readable OS version.</returns>
		public override object GetValue()
		{
			return Environment.OSVersion.VersionString;
		}
	}
}
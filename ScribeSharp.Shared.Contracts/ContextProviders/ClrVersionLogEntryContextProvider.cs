using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a "CLR Version" property.
	/// </summary>
	public sealed class ClrVersionLogEventContextProvider : ContextProviderBase
	{
		private string _Version;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ClrVersionLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public ClrVersionLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returs "CLR Version".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "CLR Version";
			}
		}

		/// <summary>
		/// Returns <see cref="Environment.Version"/> as a string.
		/// </summary>
		/// <returns>A string containing the CLR version.</returns>
		public override object GetValue()
		{
			return _Version ?? (_Version = Environment.Version.ToString());
		}
	}
}
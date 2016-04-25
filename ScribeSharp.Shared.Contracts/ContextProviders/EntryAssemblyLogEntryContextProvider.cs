using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds the name of the assembly that the app started from.
	/// </summary>
	public sealed class EntryAssemblyLogEventContextProvider : ContextProviderBase
	{

		private string _AssemblyName;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EntryAssemblyLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public EntryAssemblyLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Entry Assembly Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Entry Assembly Name";
			}
		}

		/// <summary>
		/// Returns Assembly.GetEntryAssembly().FullName;
		/// </summary>
		/// <returns>A string containing the name of the assembly that started the app.</returns>
		public override object GetValue()
		{
			return _AssemblyName ?? (_AssemblyName = Assembly.GetEntryAssembly().FullName);
		}
	}
}
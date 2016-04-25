using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds the netbios name of the local device as a property called "Machine Name".
	/// </summary>
	public sealed class MachineNameLogEventContextProvider : ContextProviderBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public MachineNameLogEventContextProvider() : base() { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public MachineNameLogEventContextProvider(ILogEventFilter filter) : base(filter) { }

		/// <summary>
		/// Returns "Machine Name".
		/// </summary>
		public override string PropertyName
		{
			get
			{
				return "Machine Name";
			}
		}

		/// <summary>
		/// Returns a string cotaining the netbios name of the local machine.
		/// </summary>
		/// <returns>A string cotaining the netbios name of the local machine.</returns>
		public override object GetValue()
		{
			return Environment.MachineName;
		}
	}
}
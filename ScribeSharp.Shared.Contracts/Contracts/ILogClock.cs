using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can return the current time, usually to be applied to <see cref="LogEvent"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>Using different implementations allows for having log events written with dates and times in specific time zones.</para>
	/// </remarks>
	public interface ILogClock
	{
		/// <summary>
		/// Returns the current time.
		/// </summary>
		DateTimeOffset Now { get; }
	}
}
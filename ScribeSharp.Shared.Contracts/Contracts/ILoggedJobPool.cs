using PoolSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface abstracting an object pool for <see cref="LogEvent"/> instances.
	/// </summary>
	public interface ILoggedJobPool : PoolSharp.IPool<LoggedJob>
	{
	}
}
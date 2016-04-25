using PoolSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// A pool of <see cref="LogEvent"/> objects, used to reduce allocations under heavy logging.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public sealed class LogEventPool : Pool<PooledObject<LogEvent>>, ILogEventPool
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// <para>Creates a pool with a size of 100 items.</para>
		/// </remarks>
		public LogEventPool() : this(100)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="maximumPoolSize">The maximum number of items to keep in the pool at any time.</param>
		public LogEventPool(int maximumPoolSize) : base
			(
				new PoolPolicy<PooledObject<LogEvent>>()
				{
					Factory = (pool) => new PooledObject<LogEvent>(pool, new LogEvent()),
					InitializationPolicy = PooledItemInitialization.AsyncReturn,
					ReinitializeObject = (pooledEntry) =>
					{
						pooledEntry.Value.Clear();
					},
					MaximumPoolSize = maximumPoolSize
				} 
			)
		{
		}
	}
}
using PoolSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public sealed class LogEventPool : Pool<PooledObject<LogEvent>>, ILogEventPool
	{
		public LogEventPool() : base
			(
				new PoolPolicy<PooledObject<LogEvent>>()
				{
					Factory = (pool) => new PooledObject<LogEvent>(pool, new LogEvent()),
					InitializationPolicy = PooledItemInitialization.AsyncReturn,
					ReinitializeObject = (pooledEntry) =>
					{
						pooledEntry.Value.Clear();
					},
					MaximumPoolSize = 100
				} 
			)
		{
		}
	}
}
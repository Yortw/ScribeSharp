using PoolSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	internal static class Globals
	{

		private static Pool<PooledObject<RecyclableStringWriter>> s_TextWriterPool;
		private static object s_Synchroniser = new object();

		public static Pool<PooledObject<RecyclableStringWriter>> TextWriterPool
		{
			get
			{
				return s_TextWriterPool ?? (s_TextWriterPool = CreatePoolInstance());
			}
		}

		private static Pool<PooledObject<RecyclableStringWriter>> CreatePoolInstance()
		{
			lock (s_Synchroniser)
			{
				if (s_TextWriterPool != null) return s_TextWriterPool;

				var policy = new PoolPolicy<PooledObject<RecyclableStringWriter>>()
				{
					Factory = (pool) => new PooledObject<RecyclableStringWriter>(pool, new RecyclableStringWriter(System.Globalization.CultureInfo.CurrentCulture)),
					InitializationPolicy = PooledItemInitialization.Return,
					MaximumPoolSize = 50,
					ReinitializeObject = (poolItem) => poolItem.Value.Close()
				};

				return (s_TextWriterPool = new Pool<PooledObject<RecyclableStringWriter>>(policy));
			}
		}
	}
}
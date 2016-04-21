using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Provides configuration controlling how an object pool works.
	/// </summary>
	/// <typeparam name="T">The type of item being pooled.</typeparam>
	/// <seealso cref="PooledItemInitialization"/>
	/// <seealso cref="Pool{T}"/>
	public class PoolPolicy<T>
	{
		/// <summary>
		/// A function that returns a new item for the pool. Used when the pool is empty and a new item is requested.
		/// </summary>
		/// <remarks>
		/// <para>Should return a new, clean item, ready for use by the caller. Takes a single argument being a reference to the pool that was asked for the object, useful if you're creating <see cref="PooledObject{T}"/> instances.</para>
		/// <para>Cannot be null. If null when provided to a <see cref="Pool{T}"/> instance, an exception will be thrown.</para>
		/// </remarks>
		public Func<IPool<T>, T> Factory { get; set; }
		/// <summary>
		/// An action that re-initialises a pooled object (cleans up/resets object state) before it is reused by callers.
		/// </summary>
		/// <remarks>
		/// <para>Can be null if no re-initialisation is required, or if the client is expected to perform it's own initialisation.</para>
		/// <para>This action is not performed against newly created items, it is a *re*-initialisation, not an initialisation action.</para>
		/// </remarks>
		public Action<T> ReinitializeObject { get; set; }
		/// <summary>
		/// Determines the maximum number of items allowed in the pool. A value of zero indicates no explicit limit.
		/// </summary>
		/// <remarks>
		/// <para>This restricts the number of instances stored in the pool at any given time, it does not represent the maximum number of items that may be generated or exist in memory at any given time. If the pool is empty and a new item is requested, a new instance will be created even if pool was previously full and all it's instances have been taken already.</para>
		/// </remarks>
		public uint MaximumPoolSize { get; set; }
		/// <summary>
		/// A value from the <see cref="PooledItemInitialization"/> enum specifying when and how pooled items are re-initialised.
		/// </summary>
		public PooledItemInitialization InitializationPolicy { get; set; } 
	}

	/// <summary>
	/// Provides options for when and how pooled items are re-initialised before reuse.
	/// </summary>
	/// <seealso cref="PoolPolicy{T}"/>
	public enum PooledItemInitialization
	{
		/// <summary>
		/// Default. Items are re-initialised synchronously when returned to the pool. Reduces latency on operations requesting items from the pool, while ensuring items in the pool are not holding references that would be cleared by re-initialisation. Additionally this may return items to the pool faster than <see cref="AsyncReturn"/> as more than one thread can perform re-initialisation of independent values at a time, and this in turn may reduce allocations in busy pools.
		/// </summary>
		Return = 0,
		/// <summary>
		/// <para>Items are re-initialised asynchronously before being returned to the pool.</para>
		/// <para>This minimises time spent waiting on the pool either taking or returning items, but risks more allocations if items are requested from the pool while returned items are still being re-initialised. Recommended if re-initalisation is time consuming.</para>
		/// </summary>
		AsyncReturn,	
		/// <summary>
		/// <para>Items are re-initialised as they are retrieved from the pool. This can impose a performance penalty retrieving an item from the pool, but reduces the chance of a new allocation due to the pool being empty. Recommended if re-initialisation is fast.</para>
		/// </summary>
		Take
	}
}
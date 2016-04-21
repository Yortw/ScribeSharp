﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for a simple object pool.
	/// </summary>
	/// <typeparam name="T">The type of value being pooled.</typeparam>
	/// <seealso cref="Pool{T}"/>
	/// <seealso cref="PooledObject{T}"/>
	/// <seealso cref="PoolPolicy{T}"/>
	public interface IPool<T> 
	{
		/// <summary>
		/// Returns an item from the pool.
		/// </summary>
		/// <remarks>
		/// <para>If the pool is empty when the request is made, a new item is instantiated and returned.</para>
		/// </remarks>
		/// <returns>Returns an instance of {T} from the pool, or a new instance if the pool is empty.</returns>
		T Take();

		/// <summary>
		/// Returns an object to the pool so it can be reused.
		/// </summary>
		/// <param name="pooledObject"></param>
		/// <remarks>
		/// <para>Items will be returned to the pool if it is not full and the item is not already in the pool, otherwise no action is taken and no error is reported.</para>
		/// <para>If the policy for the pool specifies <see cref="PooledItemInitialization.AsyncReturn"/> the item will be queued for re-intialisation on a background thread before being returned to the pool, control will return to the caller once the item has been queued even if it has not yet been fully re-initialised and returned to the pool.</para>
		/// <para>If the item is NOT returned to the pool, and {T} implements <see cref="System.IDisposable"/>, the instance will be disposed before the method returns.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="pooledObject"/> is null.</exception>
		void Return(T pooledObject);
	}
}
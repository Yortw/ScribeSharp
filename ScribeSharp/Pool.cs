using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// A simple object pool.
	/// </summary>
	/// <typeparam name="T">The type of value being pooled.</typeparam>
	/// <seealso cref="PoolPolicy{T}"/>
	/// <seealso cref="IPool{T}"/>
	/// <seealso cref="PooledObject{T}"/>
	public class Pool<T> : IPool<T>, IDisposable 
	{

		#region Fields

		private readonly System.Collections.Concurrent.ConcurrentBag<T> _Pool;
		private readonly PoolPolicy<T> _PoolPolicy;
		private readonly System.Collections.Concurrent.BlockingCollection<T> _ItemsToInitialise;
		private readonly bool _IsPooledTypeDisposable;
		private readonly bool _IsPooledTypeWrapped;
		private bool _IsDisposed;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="poolPolicy">A <seealso cref="PoolPolicy{T}"/> instance containing configuration information for the pool.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="poolPolicy"/> argument is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if the <see cref="PoolPolicy{T}.Factory"/> property of the <paramref name="poolPolicy"/> argument is null.</exception>
		public Pool(PoolPolicy<T> poolPolicy)
		{
			if (poolPolicy == null) throw new ArgumentNullException(nameof(poolPolicy));
			if (poolPolicy.Factory == null) throw new ArgumentException("poolPolicy.Factory cannot be null");

			_IsPooledTypeWrapped = IsTypeWrapped(typeof(T));
			_IsPooledTypeDisposable = IsTypeDisposable(typeof(T));

			_PoolPolicy = poolPolicy;
			_Pool = new System.Collections.Concurrent.ConcurrentBag<T>();

			if (_PoolPolicy.InitializationPolicy == PooledItemInitialization.AsyncReturn)
			{
				_ItemsToInitialise = new System.Collections.Concurrent.BlockingCollection<T>();
				//TODO: Start initialise thread.
			}
		}

		#endregion

		#region IPool<T> Members

		/// <summary>
		/// Returns an item from the pool.
		/// </summary>
		/// <remarks>
		/// <para>If the pool is empty when the request is made, a new item is instantiated and returned.</para>
		/// </remarks>
		/// <returns>Returns an instance of {T} from the pool, or a new instance if the pool is empty.</returns>
		public T Take()
		{
			CheckDisposed();

			T retVal;

			if (_Pool.TryTake(out retVal))
			{
				if (_PoolPolicy.InitializationPolicy == PooledItemInitialization.Take && _PoolPolicy.ReinitializeObject != null)
					_PoolPolicy.ReinitializeObject(retVal);
			}
			else
				retVal = _PoolPolicy.Factory(this);

			return retVal;
		}

		/// <summary>
		/// Returns an object to the pool so it can be reused.
		/// </summary>
		/// <param name="value"></param>
		/// <remarks>
		/// <para>Items will be returned to the pool if it is not full and the item is not already in the pool, otherwise no action is taken and no error is reported.</para>
		/// <para>If the policy for the pool specifies <see cref="PooledItemInitialization.AsyncReturn"/> the item will be queued for re-intialisation on a background thread before being returned to the pool, control will return to the caller once the item has been queued even if it has not yet been fully re-initialised and returned to the pool.</para>
		/// <para>If the item is NOT returned to the pool, and {T} implements <see cref="System.IDisposable"/>, the instance will be disposed before the method returns.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="value"/> is null.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the <see cref="Dispose"/> method has been called.</exception>
		public void Return(T value)
		{
			CheckDisposed();

			if (value == null) throw new ArgumentNullException(nameof(value));

			if (ShouldReturnToPool(value))
			{
				if (_PoolPolicy.InitializationPolicy == PooledItemInitialization.Take || _PoolPolicy.ReinitializeObject == null)
					_Pool.Add(value);
				else if (_PoolPolicy.InitializationPolicy == PooledItemInitialization.AsyncReturn)
					SafeAddToReinitialiseQueue(value);
				else if (_PoolPolicy.InitializationPolicy == PooledItemInitialization.Return)
				{
					_PoolPolicy.ReinitializeObject(value);
					_Pool.Add(value);
				}
			}
			else
				SafeDispose(value);
		}

		#endregion

		#region IDisposable & Related Implementation 

		/// <summary>
		/// Returns a boolean indicating if this pool is disposed or not.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="Dispose(bool)"/>
		public bool IsDisposed
		{
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Disposes this pool and all contained objects (if they are disposable).
		/// </summary>
		/// <remarks>
		/// <para>A pool can only be disposed once, calling this method multiple times will have no effect after the first invocation.</para>
		/// </remarks>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="Dispose(bool)"/>
		public void Dispose()
		{
			if (_IsDisposed) return;

			try
			{
				_IsDisposed = true;

				Dispose(true);
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Performs dispose logic, can be overridden by derivded types.
		/// </summary>
		/// <param name="disposing">True if the pool is being explicitly disposed, false if it is being disposed from a finalizer.</param>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="IsDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_ItemsToInitialise?.CompleteAdding();

				if (_IsPooledTypeDisposable)
				{
					foreach (var item in _Pool)
					{
						SafeDispose(item);
					}
				}
			}
		}

		#endregion

		#region Private Methods

		private void SafeAddToReinitialiseQueue(T pooledObject)
		{
			try
			{
				if (!_ItemsToInitialise.IsAddingCompleted)
					_ItemsToInitialise.Add(pooledObject);
			}
			catch (InvalidOperationException) { } //Handle race condition on above if condition.
		}

		private void ProcessReturnedItems()
		{
			//Only bother reinitialising items while we're alive.
			//If we're shutdown, even with items left to process, then just ignore them.
			//We're not going to use them anyway.
			while (!_ItemsToInitialise.IsAddingCompleted)
			{
				ReinitialiseAndReturnToPoolOrDispose(_ItemsToInitialise.Take());
			}

			//If we're done but the there are disposable items in the queue,
			//dispose each one.
			if (!_ItemsToInitialise.IsCompleted && _IsPooledTypeDisposable)
			{
				while (!_ItemsToInitialise.IsCompleted)
				{
					SafeDispose(_ItemsToInitialise.Take());
				}
			}
		}

		private void ReinitialiseAndReturnToPoolOrDispose(T value)
		{
			_PoolPolicy.ReinitializeObject(value);
			if (ShouldReturnToPool(value))
				_Pool.Add(value);
			else
				SafeDispose(value);
		}

		private void SafeDispose(object pooledObject)
		{
			if (_IsPooledTypeWrapped)
				pooledObject = typeof(T).GetProperty("Value").GetValue(pooledObject, null);

			(pooledObject as IDisposable)?.Dispose();
		}

		private bool ShouldReturnToPool(T pooledObject)
		{
			return (_PoolPolicy.MaximumPoolSize == 0 || _Pool.Count < _PoolPolicy.MaximumPoolSize) && !_Pool.Contains(pooledObject);
		}

		private bool IsTypeWrapped(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PooledObject<>);
		}

		private bool IsTypeDisposable(Type type)
		{
			if (_IsPooledTypeWrapped)
				type = type.GetGenericArguments().First();

			return (from i in type.GetInterfaces() where i == typeof(IDisposable) select i).Any();
		}

		private void CheckDisposed()
		{
			if (_IsDisposed) throw new ObjectDisposedException(this.GetType().FullName);
		}

		#endregion
	}

	/// <summary>
	/// A wrapper for a pooled object that allows for easily retrieving and returning the item to the pool via the using statement.
	/// </summary>
	/// <remarks>
	/// <para>In order to make a pool more convenient to use, the pool can contain <see cref="PooledObject{T}"/> references instead of direct {T} references.</para>
	/// <para>When <see cref="Dispose"/> is called on a <see cref="PooledObject{T}"/> instance, it is returned to the associated pool automatically.</para>
	/// <code>
	/// using (var wrapper = _Pool.Take())
	/// {
	///		DoSomethingWithValue(wrapper.Value);
	/// } // Wrapper and it's value will be returned to the pool here.
	/// </code>
	/// </remarks>
	/// <typeparam name="T">The type of value being pooled.</typeparam>
	public sealed class PooledObject<T> : IDisposable
	{
		private readonly IPool<PooledObject<T>> _Pool;
		private readonly T _Value;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="pool">A reference to the pool the wrapper should be returned to when <see cref="Dispose"/> is called.</param>
		/// <param name="value">The actual value of interest to the caller.</param>
		public PooledObject(IPool<PooledObject<T>> pool, T value)
		{
			if (pool == null) throw new ArgumentNullException(nameof(pool));
			if (value == null) throw new ArgumentNullException(nameof(value));

			_Pool = pool;
			_Value = value;
		}

		/// <summary>
		/// The actual value of interest.
		/// </summary>
		public T Value { get { return _Value; } }

		/// <summary>
		/// Rather than disposing the wrapper or the <see cref="Value"/>, returns the wrapper to the pool specified in the wrapper's constructor.
		/// </summary>
		public void Dispose()
		{
			try
			{
				_Pool.Return(this);
			}
			catch (ObjectDisposedException)
			{
				(this.Value as IDisposable)?.Dispose();
			}
		}
	}

}
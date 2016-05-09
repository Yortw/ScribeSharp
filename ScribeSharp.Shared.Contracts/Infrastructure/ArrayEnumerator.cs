using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.Infrastructure
{
	/// <summary>
	/// An enumerator for arrays that allows enumerating from index 0 up to a certain number of items, ignoring the rest of the array.
	/// </summary>
	/// <typeparam name="T">The type of value in the array.</typeparam>
	public sealed class ArrayEnumerator<T> : IEnumerator<T>
	{
		private T[] _Items;
		private int _NumberOfItems;

		private int _CurrentIndex;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="values">The array to iterate over.</param>
		/// <param name="numberOfItems">The maximum number of items to iterate over.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="numberOfItems"/> is less than 0 or more than the length of <paramref name="values"/>.</exception>
		public ArrayEnumerator(T[] values, int numberOfItems)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			if (numberOfItems < 0 || numberOfItems > values.Length) throw new ArgumentOutOfRangeException(nameof(numberOfItems));
			
			_Items = values;
			_NumberOfItems = numberOfItems;
			_CurrentIndex = -1;
		}

		/// <summary>
		/// Returns the current item from the enumerator.
		/// </summary>
		public T Current
		{
			get
			{
				if (_CurrentIndex < 0) throw new InvalidOperationException("The enumerator has not started.");

				return _Items[_CurrentIndex];
			}
		}

		/// <summary>
		/// Returns the current item from the enumerator.
		/// </summary>
		object IEnumerator.Current
		{
			get
			{
				if (_CurrentIndex < 0) throw new InvalidOperationException("The enumerator has not started.");

				return _Items[_CurrentIndex];
			}
		}

		/// <summary>
		/// Disposes the iterator and all internal resources.
		/// </summary>
		public void Dispose()
		{
			_Items = null;
			_CurrentIndex = -1;
		}

		/// <summary>
		/// Attempts to move to the next item in the iterator, true if it succeed, or false if the end of the enumerable items has been reached.
		/// </summary>
		/// <returns>A boolean, true if there was a next item to move to.</returns>
		public bool MoveNext()
		{
			_CurrentIndex++;
			return _CurrentIndex < _NumberOfItems;
		}

		/// <summary>
		/// Resets the enumerator to the beginning.
		/// </summary>
		public void Reset()
		{
			_CurrentIndex = -1;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Extensions to dictionaries.
	/// </summary>
	public static class DictionaryExtensions
	{

		/// <summary>
		/// Adds a value to a dictionary if it does not already contain the specified key.
		/// </summary>
		/// <remarks>
		/// <para>This method is not thread safe.</para>
		/// </remarks>
		/// <typeparam name="K">The type of key in the dictionary.</typeparam>
		/// <typeparam name="V">The type of value in the dictionary.</typeparam>
		/// <param name="dictionary">The dictionary to add to.</param>
		/// <param name="key">The key of the item to add.</param>
		/// <param name="value">The value of the item to add.</param>
		/// <returns>A boolean, true if the item was added, else false if the provided key already exists in the dictionary.</returns>
		public static bool AddIfNotExists<K, V>(this IDictionary<K, V> dictionary, K key, V value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, value);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes a value from a dictionary if one with the specified key exists.
		/// </summary>
		/// <remarks>
		/// <para>This method is not thread safe.</para>
		/// </remarks>
		/// <typeparam name="K">The type of key in the dictionary.</typeparam>
		/// <param name="dictionary">The dictionary to add to.</param>
		/// <param name="key">The key of the item to remove.</param>
		/// <returns>A boolean, true if the item was removed, else false if the provided key already did not exist in the dictionary.</returns>
		public static bool RemoveIfExists<K, V>(this IDictionary<K, V> dictionary, K key)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
				return true;
			}

			return false;
		}

	}
}
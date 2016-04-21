using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	internal static class Utils
	{

		#region Any Overloads

		//Many of these avoid the cost of allocating/collecting enumerator instances etc.
		//They also return false if the source is null, instead of throwing;

		public static bool Any(System.Collections.IList source)
		{
			if (source == null) return false;
			return source.Count > 0;
		}

		public static bool Any(ICollection source)
		{
			if (source == null) return false;
			return source.Count > 0;
		}

		public static bool Any<T>(T[] source)
		{
			if (source == null) return false;
			return source.Length > 0;
		}

		public static bool Any<T>(System.Collections.IDictionary source)
		{
			if (source == null) return false;
			return source.Count > 0;
		}

		public static bool Any<T>(System.Collections.Generic.IEnumerable<T> source)
		{
			if (source == null) return false;

			var array = source as T[];
			if (array != null) return array.Length > 0;

			var list = source as IList;
			if (list != null) return list.Count > 0;

			var dictionary = source as IDictionary;
			if (dictionary != null) return dictionary.Count > 0;

			var collection = source as ICollection;
			if (collection != null) return collection.Count > 0;

			//We failed to do anything efficient, revert to original solution.
			return Enumerable.Any(source);
		}

		#endregion

	}
}
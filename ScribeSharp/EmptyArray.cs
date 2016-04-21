using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	internal static class EmptyArray
	{
		public static T[] Empty<T>()
		{
			return EmptyArrayHolder<T>.Value;
		}
	}

	internal static class EmptyArrayHolder<T>
	{
		public static readonly T[] Value = new T[0];
	}

}
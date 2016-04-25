using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Determines whether a filter condition includes or excludes items from a list.
	/// </summary>
	public enum FilterConditionStyle
	{
		/// <summary>
		/// Items in the list are included by the filter.
		/// </summary>
		Include = 0,
		/// <summary>
		/// Items in the list are excluded by the filter.
		/// </summary>
		Exclude
	}
}
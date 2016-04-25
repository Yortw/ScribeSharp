using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Represents an aggregate filter where ALL child filters must return true from their <see cref="ILogEventFilter.ShouldProcess(LogEvent)"/> method for the event to be processed.
	/// </summary>
	public sealed class AndFilter : ILogEventFilter
	{
		private ILogEventFilter[] _Filters;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filters">An enumerable set of child filters.</param>
		public AndFilter(IEnumerable<ILogEventFilter> filters)
		{
			_Filters = filters?.ToArray();
		}

		/// <summary>
		/// Returns true if ALL the child filters return true.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>Returns true if all of the child filters return true.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (_Filters == null) return true;

			for (int cnt = 0; cnt < _Filters.Length; cnt++)
			{
				if (!_Filters[cnt].ShouldProcess(logEvent)) return false;
			}

			return true;
		}
	}
}
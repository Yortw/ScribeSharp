using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Represents an aggregate filter where ANY child filters must return true from their <see cref="ILogEventFilter.ShouldProcess(LogEvent)"/> method for the event to be processed.
	/// </summary>
	public sealed class OrFilter : ILogEventFilter
	{
		private ILogEventFilter[] _Filters;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filters">An enumerable set of child filters.</param>
		public OrFilter(IEnumerable<ILogEventFilter> filters)
		{
			_Filters = filters?.ToArray();
		}

		/// <summary>
		/// Returns true if ANY the child filters return true.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>Returns true if any of the child filters return true.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (_Filters == null) return true;

			for (int cnt = 0; cnt < _Filters.Length; cnt++)
			{
				if (_Filters[cnt].ShouldProcess(logEvent)) return true;
			}

			return false;
		}
	}
}
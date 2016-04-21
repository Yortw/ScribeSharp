using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class AndFilter : ILogEventFilter
	{
		private ILogEventFilter[] _Filters;

		public AndFilter(IEnumerable<ILogEventFilter> filters)
		{
			_Filters = filters?.ToArray();
		}

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
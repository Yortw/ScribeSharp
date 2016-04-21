using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class DelegateLogEventFilter : ILogEventFilter
	{
		private readonly Func<LogEvent, bool> _FilterFunc;

		public DelegateLogEventFilter(Func<LogEvent, bool> filterFunc)
		{
			if (filterFunc == null) throw new ArgumentNullException(nameof(filterFunc));

			_FilterFunc = filterFunc;
		}

		public bool ShouldProcess(LogEvent logEvent)
		{
			return _FilterFunc(logEvent);
		}
	}
}
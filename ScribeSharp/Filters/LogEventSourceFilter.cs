using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	public sealed class LogEventSourceFilter : ILogEventFilter
	{
		private string[] _Sources;
		private FilterType _FilterType;

		public LogEventSourceFilter(IEnumerable<string> sources, FilterType filterType)
		{
			if (sources == null) throw new ArgumentNullException(nameof(sources));

			_Sources = sources.ToArray();
			_FilterType = filterType;
		}

		public bool ShouldProcess(LogEvent logEvent)
		{
			for (int cnt = 0; cnt < _Sources.Length; cnt++)
			{
				if (_Sources[cnt] == logEvent.Source)
				{
					return _FilterType == FilterType.Include;
				}
			}
			return _FilterType == FilterType.Exclude;
		}
	}

	public enum FilterType
	{
		Include = 0,
		Exclude
	}
}
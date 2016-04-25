using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// A filter the applies based on the <seealso cref="LogEvent.Source"/> property value.
	/// </summary>
	public sealed class LogEventSourceFilter : ILogEventFilter
	{
		private string[] _Sources;
		private FilterConditionStyle _FilterType;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="sources">A list of strings containing the sources checked by this filter. Matching is case sensitive.</param>
		/// <param name="filterType">A value from the <seealso cref="FilterConditionStyle"/> enum determining whether the values in <paramref name="sources"/> are included or excluded.</param>
		public LogEventSourceFilter(IEnumerable<string> sources, FilterConditionStyle filterType)
		{
			if (sources == null) throw new ArgumentNullException(nameof(sources));

			_Sources = sources.ToArray();
			_FilterType = filterType;
		}

		/// <summary>
		/// Returns a boolean based on whether the <seealso cref="LogEvent.Source"/> matches any of the configured sources.
		/// </summary>
		/// <param name="logEvent">The <seealso cref="LogEvent"/> instance to examine.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			for (int cnt = 0; cnt < _Sources.Length; cnt++)
			{
				if (_Sources[cnt] == logEvent.Source)
				{
					return _FilterType == FilterConditionStyle.Include;
				}
			}
			return _FilterType == FilterConditionStyle.Exclude;
		}
	}
}
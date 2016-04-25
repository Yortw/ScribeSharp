using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filters an event based on whether or not it contains a specified string, using case insensitive matching.
	/// </summary>
	public sealed class MessageContainsTextFilter : ILogEventFilter
	{
		private string _PartialText;
		private FilterConditionStyle _FilterCondition;

		/// <summary>
		/// Partial constructor. Takes a <see cref="LogEventType"/> value that will be included by this filter.
		/// </summary>
		/// <param name="partialText">The substring the <see cref="LogEvent.EventName"/> property must contain (cased ignored) to pass the filter.</param>
		public MessageContainsTextFilter(string partialText) : this(partialText, FilterConditionStyle.Include)
		{
		}

		/// <summary>
		/// Full constructor. Takes a <see cref="LogEventType"/> value that will be included or excluded by this filter.
		/// </summary>
		/// <param name="partialText">The substring the <see cref="LogEvent.EventName"/> property must contain (cased ignored) to pass the filter.</param>
		/// <param name="filterCondition">A <see cref="FilterConditionStyle"/> value that indicates whether <paramref name="partialText"/> is included or excluded.</param>
		public MessageContainsTextFilter(string partialText, FilterConditionStyle filterCondition) 
		{
			if (partialText == null) throw new ArgumentNullException(nameof(partialText));
			if (String.IsNullOrWhiteSpace(partialText)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, partialText), nameof(partialText));

			_PartialText = partialText;
			_FilterCondition = filterCondition;
		}

		/// <summary>
		/// Returns a boolean based on whether the <seealso cref="LogEvent"/> passes the filter.
		/// </summary>
		/// <param name="logEvent">The <seealso cref="LogEvent"/> instance to examine.</param>
		/// <returns>True if the log event passed the filter's rules.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
			var retVal = logEvent.EventName.IndexOf(_PartialText, StringComparison.OrdinalIgnoreCase) >= 0;
			return _FilterCondition == FilterConditionStyle.Include ? retVal : !retVal;
		}
	}
}
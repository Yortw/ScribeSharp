using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filters an event based on whether or not it has a particular property, regardless of that property's value.
	/// </summary>
	public sealed class HasPropertyFilter : ILogEventFilter
	{
		private string _PropertyName;
		private FilterConditionStyle _FilterCondition;

		/// <summary>
		/// Partial constructor. Takes a <see cref="LogEventType"/> value that will be included by this filter.
		/// </summary>
		/// <param name="propertyName">The name of the property that must exist in the log event for it to pass the filter.</param>
		public HasPropertyFilter(string propertyName) : this(propertyName, FilterConditionStyle.Include)
		{
		}

		/// <summary>
		/// Full constructor. Takes a <see cref="LogEventType"/> value that will be included or excluded by this filter.
		/// </summary>
		/// <param name="propertyName">The name of the property that must exist in the log event for it to pass the filter.</param>
		/// <param name="filterCondition">A <see cref="FilterConditionStyle"/> value that indicates whether <paramref name="propertyName"/> is included or excluded.</param>
		public HasPropertyFilter(string propertyName, FilterConditionStyle filterCondition) 
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, propertyName), nameof(propertyName));

			_PropertyName = propertyName;
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
			var retVal = logEvent.Properties.ContainsKey(_PropertyName);
			return _FilterCondition == FilterConditionStyle.Include ? retVal : !retVal;
		}
	}
}
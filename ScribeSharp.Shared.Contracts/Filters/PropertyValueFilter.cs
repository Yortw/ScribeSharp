using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// Filters an event based on the value of a particular property.
	/// </summary>
	public sealed class PropertyValueFilter : ILogEventFilter
	{
		private string _PropertyName;
		private object _PropertyValue;
		private FilterConditionStyle _FilterCondition;

		/// <summary>
		/// Partial constructor. Takes a <see cref="LogEventType"/> value that will be included by this filter.
		/// </summary>
		/// <param name="propertyName">The name of the property that must exist in the log event for it to pass the filter.</param>
		/// <param name="propertyValue">The value the property must equal to pass the filter.</param>
		public PropertyValueFilter(string propertyName, object propertyValue) : this(propertyName, propertyValue, FilterConditionStyle.Include)
		{
		}

		/// <summary>
		/// Full constructor. Takes a <see cref="LogEventType"/> value that will be included or excluded by this filter.
		/// </summary>
		/// <param name="propertyName">The name of the property that must exist in the log event for it to pass the filter.</param>
		/// <param name="propertyValue">The value the property must equal to pass the filter.</param>
		/// <param name="filterCondition">A <see cref="FilterConditionStyle"/> value that indicates whether <paramref name="propertyName"/> is included or excluded.</param>
		public PropertyValueFilter(string propertyName, object propertyValue, FilterConditionStyle filterCondition) 
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, propertyName), nameof(propertyName));

			_PropertyName = propertyName;
			_PropertyValue = propertyValue;
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

			var retVal = false;
			if (logEvent.Properties.ContainsKey(_PropertyName))
			{
				if (_PropertyValue == null)
					retVal = logEvent.Properties[_PropertyName] == null;
				else
					retVal = _PropertyValue.Equals(logEvent.Properties[_PropertyName]);
			}

			return _FilterCondition == FilterConditionStyle.Include ? retVal : !retVal;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Filters
{
	/// <summary>
	/// A filter that uses a delegate to decide if a <see cref="LogEvent"/> should be processed.
	/// </summary>
	/// <seealso cref="LogEvent"/>
	/// <seealso cref="ILogEventFilter"/>
	public sealed class DelegateLogEventFilter : ILogEventFilter
	{
		private readonly Func<LogEvent, bool> _FilterFunc;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filterFunc">A <seealso cref="System.Func{T, TResult}"/> instance that takes a <seealso cref="LogEvent"/> and returns a boolean. Used by the <seealso cref="ShouldProcess(LogEvent)"/> to return it's result.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="filterFunc"/> is null.</exception>
		public DelegateLogEventFilter(Func<LogEvent, bool> filterFunc)
		{
			if (filterFunc == null) throw new ArgumentNullException(nameof(filterFunc));

			_FilterFunc = filterFunc;
		}

		/// <summary>
		/// Returns the value of the <seealso cref="System.Func{T, TResult}"/> provided in the constructor.
		/// </summary>
		/// <param name="logEvent">A <seealso cref="LogEvent"/> instance to evaluate.</param>
		/// <returns>A boolean indicating whether or not the log event passed the filter.</returns>
		public bool ShouldProcess(LogEvent logEvent)
		{
			return _FilterFunc(logEvent);
		}
	}
}
using ScribeSharp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Specifies settings and behaviour for a <see cref="ILogger"/> implementation.
	/// </summary>
	public class LogPolicy
	{
		/// <summary>
		/// Sets or returns a <see cref="ILogWriter"/> implementation to output log events to.
		/// </summary>
		/// <remarks>
		/// <para>To output to multiple logs, use an <see cref="ScribeSharp.Writers.AggregateLogWriter"/> instance.</para>
		/// </remarks>
		public ILogWriter LogWriter { get; set; }
		/// <summary>
		/// Sets or returns a <see cref="ILogClock"/> implementation used to set the current date and time on new log entries.
		/// </summary>
		public ILogClock Clock { get; set; }
		/// <summary>
		/// Sets or returns an <see cref="ILogEventFilter"/> implementation used to filter out log events that should not be output to the <see cref="LogWriter"/>.
		/// </summary>
		/// <remarks>
		/// <para>For multiple filters, use either a <see cref="Filters.AndFilter"/> or <see cref="Filters.OrFilter"/> instance.</para>
		/// <para>If null, no logging is performed.</para>
		/// </remarks>
		public ILogEventFilter Filter { get; set; }
		/// <summary>
		/// A special filter applied early in the logging process. This filter is limited in values it can check, but filtering out log events early can improve performance.
		/// </summary>
		/// <remarks>
		/// <para>If null, no logging is performed.</para>
		/// </remarks>
		public IFirstChanceLogFilter FirstChanceFilter { get; set; }
		/// <summary>
		/// A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="LogWriter"/>.
		/// </summary>
		public IEnumerable<ILogEventContextProvider> ContextProviders { get; set; }
		/// <summary>
		/// Sets or returns the source to apply to all log events written by associated loggers. If null, the system will attempt to automatically apply a source.
		/// </summary>
		public string Source { get; set; }
		/// <summary>
		/// Sets or returns the error handler to use when an error occurs within the logging system.
		/// </summary>
		public ILoggingErrorHandler ErrorHandler { get; set; }
		/// <summary>
		/// Sets or returns a
		/// </summary>
		public ITypeRendererMap TypeRendererMap {get;set;}
		/// <summary>
		/// An optional renderer to use for <see cref="System.Exception"/> or any derived type that does not have a property renderer in the type map.
		/// </summary>
		public IPropertyRenderer DefaultExceptionRenderer { get; set; }
		/// <summary>
		/// Sets or returns the maximum size of the object pool used for <see cref="LogEvent"/> instances.
		/// </summary>
		/// <remarks>
		/// <para>Larger pool sizes decrease the total number of allocations and reduce the chance of garbage collection, but risk consuming more memory.
		/// Larger values should also be used when buffering, async or queued log writers are used. Use a value of zero to allow the pool an unlimited capacity, or a negative value to prevent pooling.</para>
		/// <para>The default value is zero, so the pool will grow as large as needed.</para>
		/// </remarks>
		public int LogEventPoolCapacity { get; set; }
		/// <summary>
		/// Sets or returns the maximum size of the object pool used for <see cref="LoggedJob"/> instances.
		/// </summary>
		/// <remarks>
		/// <para>Larger pool sizes decrease the total number of allocations and reduce the chance of garbage collection, but risk consuming more memory.
		/// Larger values should also be used when buffering, async or queued log writers are used. Use a value of zero to allow the pool an unlimited capacity, or a negative value to prevent pooling.</para>
		/// <para>The default value is zero, so the pool will grow as large as needed.</para>
		/// </remarks>
		public int JobPoolCapacity { get; set; }
	}
}
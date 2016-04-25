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
		/// </remarks>
		public ILogEventFilter Filter { get; set;}
		/// <summary>
		/// A set of <see cref="ILogEventContextProvider"/> instances used to add additional information to log events before they are written to the <see cref="LogWriter"/>.
		/// </summary>
		public IEnumerable<ILogEventContextProvider> ContextProviders { get; set; }
		/// <summary>
		/// A dictionary of <see cref="IPropertyRenderer"/> implementations keyed by the type of value they know how to render.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IDictionary<Type, IPropertyRenderer> PropertyRenderers { get; set; }
		/// <summary>
		/// Sets or returns the source to apply to all log events written by associated loggers. If null, the system will attempt to automatically apply a source.
		/// </summary>
		public string Source { get; set; }
	}
}
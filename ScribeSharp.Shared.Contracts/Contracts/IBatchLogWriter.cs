using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface implemented by components that can perform efficient batching when writing log events.
	/// </summary>
	public interface IBatchLogWriter : ILogWriter
	{
		/// <summary>
		/// Writes all log events from a <see cref="IEnumerable{T}"/> batch.
		/// </summary>
		/// <param name="logEvents">An enumerable set of <see cref="LogEvent"/> instances.</param>
		void WriteBatch(IEnumerable<LogEvent> logEvents);

		/// <summary>
		/// Writes log events from a <see cref="IEnumerable{T}"/> batch up to the specified index.
		/// </summary>
		/// <remarks>
		/// <para>This method allows writing batches from an array that is reused, to reduce allocations. Writers implementing this method should only write events from the zero based index up to the index specified by <paramref name="length"/>.</para>
		/// </remarks>
		/// <param name="logEvents">An array of events to be written.</param>
		/// <param name="length">The last index within the array that should be written in this batch.</param>
		void WriteBatch(LogEvent[] logEvents, int length);
	}
}
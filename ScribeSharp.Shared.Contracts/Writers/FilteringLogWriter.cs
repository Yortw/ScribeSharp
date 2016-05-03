using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer that filters events before passing them on.
	/// </summary>
	public class FilteringLogWriter : IBatchLogWriter
	{

		private readonly ILogEventFilter _Filter;
		private readonly IBatchLogWriter _LogWriter;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">The filter to check before writing events. Only events that are passed by the filter are written.</param>
		/// <param name="logWriter">Another <seealso cref="ILogWriter"/> implementation that filtered events will be passed to.</param>
		public FilteringLogWriter(ILogEventFilter filter, ILogWriter logWriter)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (logWriter == null) throw new ArgumentNullException(nameof(logWriter));

			_Filter = filter;
			_LogWriter = logWriter as IBatchLogWriter;
			if (_LogWriter == null)
				_LogWriter = new BatchLogWriterAdapter(logWriter);
		}

		/// <summary>
		/// Returns the <see cref="ILogWriter.RequiresSynchronization"/> value from the inner log writer passed via the constructor.
		/// </summary>
		public bool RequiresSynchronization
		{
			get
			{
				return _LogWriter.RequiresSynchronization;
			}
		}

		/// <summary>
		/// Writes an event, but only if it passes the filter provided via the constructor.
		/// </summary>
		/// <param name="logEvent">The log event to write.</param>
		public void Write(LogEvent logEvent)
		{
			if (_Filter.ShouldProcess(logEvent))
				_LogWriter.Write(logEvent);
		}

		/// <summary>
		/// Writes all log events from a <see cref="IEnumerable{T}"/> batch.
		/// </summary>
		/// <param name="logEvents">An enumerable set of <see cref="LogEvent"/> instances.</param>
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			if (logEvents == null) return;

			try
			{
				var filteredEvents = (from logEvent
															in logEvents
															where _Filter.ShouldProcess(logEvent)
															select logEvent);

				_LogWriter.WriteBatch(filteredEvents);
			}
			catch (LogWriterException) { throw; }
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				throw new LogWriterException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Writes log events from a <see cref="IEnumerable{T}"/> batch up to the specified index.
		/// </summary>
		/// <remarks>
		/// <para>This method allows writing batches from an array that is reused, to reduce allocations. Writers implementing this method should only write events from the zero based index up to the index specified by <paramref name="length"/>.</para>
		/// </remarks>
		/// <param name="logEvents">An array of events to be written.</param>
		/// <param name="length">The last index within the array that should be written in this batch.</param>
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null || length == 0) return;

			try
			{
				var filteredEvents = logEvents.Where(
					(logEvent, index) => index < length
						&& _Filter.ShouldProcess(logEvent)
				);

				_LogWriter.WriteBatch(filteredEvents);
			}
			catch (LogWriterException) { throw; }
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				throw new LogWriterException(ex.Message, ex);
			}
		}

	}
}
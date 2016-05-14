using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A batched log writer used to wrap another log writer that does not support batching, for convenience rather than performance.
	/// </summary>
	public sealed class BatchLogWriterAdapter : IBatchLogWriter, IDisposable
	{

		private ILogWriter _LogWriter;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="logWriter">The <see cref="ILogWriter"/> implementation to use to write events.</param>
		public BatchLogWriterAdapter(ILogWriter logWriter)
		{
			if (logWriter == null) throw new ArgumentNullException(nameof(logWriter));

			_LogWriter = logWriter;
		}

		/// <summary>
		/// Returns the <see cref="ILogWriter.RequiresSynchronization"/> property of the log writer provided in the constructor.
		/// </summary>
		public bool RequiresSynchronization
		{
			get
			{
				return _LogWriter.RequiresSynchronization;
			}
		}

		/// <summary>
		/// Pass the <paramref name="logEvent"/> to the inner log writer.
		/// </summary>
		/// <param name="logEvent">The event to log.</param>
		public void Write(LogEvent logEvent)
		{
			try
			{
				_LogWriter.Write(logEvent);
			}
			catch (LogWriterException)
			{
				throw;
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				throw new LogWriterException(ex.Message, ex);
			}
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
				foreach (var logEvent in logEvents)
				{
					Write(logEvent);
				}
			}
			catch (LogWriterException)
			{
				throw;
			}
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
		/// <param name="length">The number of items, starting from index zero, to write from the array. The contents of the array after this item should be ignored.</param>
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null) return;

			try
			{
				for (int cnt = 0; cnt < length; cnt++)
				{
					Write(logEvents[cnt]);
				}
			}
			catch (LogWriterException)
			{
				throw;
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				throw new LogWriterException(ex.Message, ex);
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes this instance and the underlying log writer.
		/// </summary>
		public void Dispose()
		{
			(_LogWriter as IDisposable)?.Dispose();
		}

		#endregion

	}
}
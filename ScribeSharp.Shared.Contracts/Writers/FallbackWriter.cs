using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer that attempts to write to a child log writer, and if an error occurs falls back to a second log writer.
	/// </summary>
	public sealed class FallbackWriter : ILogWriter, IBatchLogWriter
	{

		#region Fields

		private IBatchLogWriter _PrimaryWriter;
		private IBatchLogWriter _SecondaryWriter;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="primaryWriter">The <see cref="ILogWriter"/> instance to first attempt to write to.</param>
		/// <param name="secondaryWriter">The <see cref="ILogWriter"/> instance to attempt to write to if the first write attempt fails.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="primaryWriter"/> or <paramref name="secondaryWriter"/> is null.</exception>
		public FallbackWriter(ILogWriter primaryWriter, ILogWriter secondaryWriter)
		{
			if (primaryWriter == null) throw new ArgumentNullException(nameof(primaryWriter));
			if (secondaryWriter == null) throw new ArgumentNullException(nameof(secondaryWriter));

			var batchWriter = primaryWriter as IBatchLogWriter;
			if (batchWriter == null)
				batchWriter = new BatchLogWriterAdapter(primaryWriter);

			_PrimaryWriter = batchWriter;

			batchWriter = primaryWriter as IBatchLogWriter;
			if (batchWriter == null)
				batchWriter = new BatchLogWriterAdapter(secondaryWriter);

			_SecondaryWriter = batchWriter;
		}

		#endregion

		#region ILogWriter Members

		/// <summary>
		/// Returns false. This writer will perform it's own locking depending on the <see cref="ILogWriter.RequiresSynchronization"/> value of each writer used.
		/// </summary>
		public bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Attempts to write to the primary writer, if an exception occurs attempts to write to the secondary writer. If the secondary writer also throws an exception, it is wrapped in a <see cref="LogWriterException"/> and rethrown.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to be written.</param>
		public void Write(LogEvent logEvent)
		{
			try
			{
				WriteToWriter(_PrimaryWriter, logEvent);
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				try
				{
					WriteToWriter(_SecondaryWriter, logEvent);
				}
				catch (LogWriterException)
				{
					throw;
				}
				catch (Exception swex) when (!ex.ShouldRethrowImmediately())
				{
					throw new LogWriterException(swex.Message, swex);
				}
			}
		}

		#endregion

		#region IBatchLogWriter Members

		/// <summary>
		/// Writes all log events from a <see cref="IEnumerable{T}"/> batch.
		/// </summary>
		/// <param name="logEvents">An enumerable set of <see cref="LogEvent"/> instances.</param>
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			try
			{
				WriteBatchToWriter(_PrimaryWriter, logEvents);
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				try
				{
					WriteBatchToWriter(_SecondaryWriter, logEvents);
				}
				catch (LogWriterException)
				{
					throw;
				}
				catch (Exception swex) when (!ex.ShouldRethrowImmediately())
				{
					throw new LogWriterException(swex.Message, swex);
				}
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
			try
			{
				WriteBatchToWriter(_PrimaryWriter, logEvents, length);
			}
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				try
				{
					WriteBatchToWriter(_PrimaryWriter, logEvents, length);
				}
				catch (LogWriterException)
				{
					throw;
				}
				catch (Exception swex) when (!ex.ShouldRethrowImmediately())
				{
					throw new LogWriterException(swex.Message, swex);
				}
			}
		}

		#endregion

		#region Private Methods

		private static void WriteToWriter(ILogWriter writer, LogEvent logEvent)
		{
			if (writer.RequiresSynchronization)
			{
				lock (writer)
				{
					writer.Write(logEvent);
				}
			}
			else
				writer.Write(logEvent);
		}

		private static void WriteBatchToWriter(IBatchLogWriter writer, IEnumerable<LogEvent> logEvents)
		{
			if (writer.RequiresSynchronization)
			{
				lock (writer)
				{
					writer.WriteBatch(logEvents);
				}
			}
			else
				writer.WriteBatch(logEvents);
		}

		private static void WriteBatchToWriter(IBatchLogWriter writer, LogEvent[] logEvents, int length)
		{
			if (writer.RequiresSynchronization)
			{
				lock (writer)
				{
					writer.WriteBatch(logEvents, length);
				}
			}
			else
				writer.WriteBatch(logEvents, length);
		}

		#endregion

	}
}
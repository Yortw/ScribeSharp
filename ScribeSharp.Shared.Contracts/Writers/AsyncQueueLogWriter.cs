using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Uses a queue and background thread to write log events. Improves performance as the client doesn't have to wait for the I/O of the log writers to complete.
	/// </summary>
	/// <remarks>
	/// <para>While this can improve client performance by eliminate the waiting on I/O to complete, it will allow the client processing to run ahead of the log.
	/// If the client crashes or quits before the log writer is finished then log events may be lost.</para>
	/// </remarks>
	public sealed class AsyncQueueLogWriter : LogWriterBase, IDisposable
	{

		#region Fields

		private ILogWriter _LogWriter;
		private System.Collections.Concurrent.ConcurrentQueue<LogEvent> _BufferedLogEvents;
		private System.Threading.ManualResetEvent _WriteBufferedEventsSignal;
		private System.Threading.Timer _BufferTimeoutTimer;

		private volatile bool _IsDisposed;
		private int _BatchSize;
		private TimeSpan _WriteTimeout;

#if SUPPORTS_THREAD
		private System.Threading.Thread _BackgroundWriteThread;
#else
		private System.Threading.Tasks.Task _BackgroundWriteTask;
#endif

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="logWriter">The log writer to forward events to from the background thread. If multiple output locations are required, usea  <see cref="AggregateLogWriter"/> instance.</param>
		/// <param name="batchSize">The number of items in the queue before they are actually passed to <paramref name="logWriter"/>.</param>
		/// <param name="writeTimeout">The time after the last event was logged to wait before writing items in the queue, even if the queue size is still less than <param ref="batchSize"/>.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logWriter"/> is null.</exception>
		public AsyncQueueLogWriter(ILogWriter logWriter, int batchSize, TimeSpan writeTimeout) : this(logWriter, batchSize, writeTimeout, null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="logWriter">The log writer to forward events to from the background thread. If multiple output locations are required, usea  <see cref="AggregateLogWriter"/> instance.</param>
		/// <param name="batchSize">The number of items in the queue before they are actually passed to <paramref name="logWriter"/>.</param>
		/// <param name="writeTimeout">The time after the last event was logged to wait before writing items in the queue, even if the queue size is still less than <param ref="batchSize"/>.</param>
		/// <param name="filter">An <see cref="ILogEventFilter"/> to apply to events. Only events that pass the filter will be written. If null, no filtering is performed.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logWriter"/> is null.</exception>
		public AsyncQueueLogWriter(ILogWriter logWriter, int batchSize, TimeSpan writeTimeout, ILogEventFilter filter) : base(filter)
		{
			if (logWriter == null) throw new ArgumentNullException(nameof(logWriter));

			_BatchSize = batchSize;
			_WriteTimeout = writeTimeout;

			if (writeTimeout.TotalMilliseconds > 0)
				_BufferTimeoutTimer = new System.Threading.Timer((reserved) => SetWriteEventsSignal(), null, TimeSpan.Zero, TimeSpan.Zero);

			_WriteBufferedEventsSignal = new System.Threading.ManualResetEvent(false);
			_BufferedLogEvents = new System.Collections.Concurrent.ConcurrentQueue<LogEvent>();
			_LogWriter = logWriter;

			StartBackgroundWriterThread();
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Queues the <paramref name="logEvent"/> for writing to the child log writers.
		/// </summary>
		/// <remarks>
		/// <para>Queues the item and also notifies the write thread to start work if queue size is at least equal to the configured batch size, otherwise starts/resets a timer to kick off the write thread after the configured timeout.</para>
		/// </remarks>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			CheckIsDisposed();

			_BufferedLogEvents.Enqueue(logEvent);
			if (_BufferedLogEvents.Count >= _BatchSize)
				SetWriteEventsSignal();
			else
				StartTimeoutTimer();
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Disposes this instance and all internal resources.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_BufferTimeoutTimer", Justification = "It is disposed, CA just can't tell.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_WriteBufferedEventsSignal", Justification = "It is disposed, CA just can't tell.")]
		public void Dispose()
		{
			if (!_IsDisposed)
			{
				_IsDisposed = true;
				_BufferTimeoutTimer?.Dispose();

				_WriteBufferedEventsSignal?.Set();
				WaitForBackgroundThreadToExit();

				(_LogWriter as IDisposable)?.Dispose();
				_WriteBufferedEventsSignal?.Dispose();
			}
		}

		private void WaitForBackgroundThreadToExit()
		{
#if SUPPORTS_THREAD
			_BackgroundWriteThread.Join();
#else
			_BackgroundWriteTask.Wait();	
#endif
		}

		#endregion

		#region Private Methods

		private void StartBackgroundWriterThread()
		{
#if SUPPORTS_THREAD
			var thread = new System.Threading.Thread(BackgroundWriteEvents);
			thread.Name = nameof(AsyncQueueLogWriter) + " Background Write Thread";
			thread.IsBackground = true;
			thread.Start();
			_BackgroundWriteThread = thread;
#else
			_BackgroundWriteTask = System.Threading.Tasks.Task.Factory.StartNew(BackgroundWriteEvents, System.Threading.Tasks.TaskCreationOptions.LongRunning);
#endif
		}

		private void StartTimeoutTimer()
		{
			_BufferTimeoutTimer?.Change(TimeSpan.Zero, _WriteTimeout);
		}

		private void SetWriteEventsSignal()
		{
			StopWriteTimeoutTimer();
			_WriteBufferedEventsSignal.Set();
		}

		private void StopWriteTimeoutTimer()
		{
			_BufferTimeoutTimer?.Change(TimeSpan.Zero, TimeSpan.Zero);
		}

		private void BackgroundWriteEvents()
		{
			while (!_IsDisposed)
			{
				_WriteBufferedEventsSignal?.WaitOne();

				LogEvent logEvent = null;
				while (_BufferedLogEvents.TryDequeue(out logEvent))
				{
					_LogWriter.Write(logEvent);
				}
			}
		}

		private void CheckIsDisposed()
		{
			if (_IsDisposed) throw new ObjectDisposedException(nameof(AsyncQueueLogWriter));
		}

		#endregion

	}
}
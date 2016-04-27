using PoolSharp;
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
	public sealed class AsyncQueueLogWriter : LogWriterBase, IFlushable, IDisposable
	{

		#region Fields

		private static readonly TimeSpan InfiniteTimespan = TimeSpan.FromMilliseconds(-1);

		private ILogWriter _LogWriter;
		private System.Collections.Concurrent.ConcurrentQueue<PooledObject<LogEvent>> _BufferedLogEvents;
		private System.Threading.ManualResetEvent _WriteBufferedEventsSignal;
		private System.Threading.ManualResetEvent _QueueEmptySignal;
		private System.Threading.Timer _BufferTimeoutTimer;

		private volatile bool _IsDisposed;
		private int _BatchSize;
		private TimeSpan _WriteTimeout;

		private ILogEventPool _LogEventPool;

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
		/// <param name="batchSize">The number of items in the queue before they are actually passed to <paramref name="logWriter"/>. Specify 1 to have entries written immediately.</param>
		/// <param name="writeTimeout">The time after the last event was logged to wait before writing items in the queue, even if the queue size is still less than <param ref="batchSize"/>. Specify <see cref="TimeSpan.Zero"/> to have entries written immediately, or a time span with a negative value to have no timeout applied.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logWriter"/> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="batchSize"/> is less than or equal to zero.</exception>
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
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="batchSize"/> is less than or equal to zero.</exception>
		public AsyncQueueLogWriter(ILogWriter logWriter, int batchSize, TimeSpan writeTimeout, ILogEventFilter filter) : base(filter)
		{
			if (logWriter == null) throw new ArgumentNullException(nameof(logWriter));
			if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

			_BatchSize = batchSize;
			_WriteTimeout = writeTimeout;
			_LogEventPool = new LogEventPool(batchSize * 2);

			if (writeTimeout.TotalMilliseconds > 0)
				_BufferTimeoutTimer = new System.Threading.Timer((reserved) => SetWriteEventsSignal(), null, InfiniteTimespan, InfiniteTimespan);

			_WriteBufferedEventsSignal = new System.Threading.ManualResetEvent(false);
			_BufferedLogEvents = new System.Collections.Concurrent.ConcurrentQueue<PooledObject<LogEvent>>();
			_LogWriter = logWriter;

			StartBackgroundWriterThread();

			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
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
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			CheckIsDisposed();

			var pooledEvent = _LogEventPool.Take();
			logEvent.Clone(pooledEvent.Value);

			_BufferedLogEvents.Enqueue(pooledEvent);
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
				return _LogWriter.RequiresSynchronization;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Disposes this instance and all internal resources.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_LogEventPool")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_BufferTimeoutTimer", Justification = "It is disposed, CA just can't tell.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_WriteBufferedEventsSignal", Justification = "It is disposed, CA just can't tell.")]
		public void Dispose()
		{
			if (!_IsDisposed)
			{
				AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
				AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
				AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;

				_IsDisposed = true;
				_BufferTimeoutTimer?.Dispose();

				_WriteBufferedEventsSignal?.Set();
				WaitForBackgroundThreadToExit();

				(_LogWriter as IDisposable)?.Dispose();
				_WriteBufferedEventsSignal?.Dispose();

				var emptySignal = _QueueEmptySignal;
				_QueueEmptySignal = null;
				emptySignal?.Dispose();

				_LogEventPool?.Dispose();
			}
		}

		/// <summary>
		/// Starts writing queued events regardless of other settings, and waits for the queue to be empty.
		/// </summary>
		public void Flush()
		{
			StopWriteTimeoutTimer();

			if (_BufferedLogEvents.Count > 0)
			{
#if SUPPORTS_THREAD
				if (_BackgroundWriteThread?.IsAlive ?? false)
				{
#else
				if ((_BackgroundWriteTask?.Status ?? System.Threading.Tasks.TaskStatus.Running) == System.Threading.Tasks.TaskStatus.Running)
				{
#endif
					lock (_BufferedLogEvents)
					{
						if (_QueueEmptySignal == null)
							_QueueEmptySignal = new System.Threading.ManualResetEvent(false);
						else
							_QueueEmptySignal.Reset();
					}

					_QueueEmptySignal.WaitOne();
				}
			}
		}

		#endregion

		#region Private Methods

		private void WaitForBackgroundThreadToExit()
		{
#if SUPPORTS_THREAD
			_BackgroundWriteThread.Join();
#else
			_BackgroundWriteTask.Wait();	
#endif
		}

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
			_BufferTimeoutTimer?.Change(_WriteTimeout, InfiniteTimespan);
		}

		private void SetWriteEventsSignal()
		{
			StopWriteTimeoutTimer();
			_WriteBufferedEventsSignal.Set();
		}

		private void StopWriteTimeoutTimer()
		{
			_BufferTimeoutTimer?.Change(InfiniteTimespan, InfiniteTimespan);
		}

		private void BackgroundWriteEvents()
		{
			while (!_IsDisposed)
			{
				try
				{
					_WriteBufferedEventsSignal?.WaitOne();
				}
				catch (ObjectDisposedException) { }
				catch (System.Threading.AbandonedMutexException) { }
				catch (System.InvalidOperationException) { }

				PooledObject<LogEvent> pooledLogEvent = null;
				while (_BufferedLogEvents.TryDequeue(out pooledLogEvent))
				{
					_LogWriter.Write(pooledLogEvent.Value);
					pooledLogEvent.Dispose();
				}
				_QueueEmptySignal?.WaitOne();
			}
		}

		private void CheckIsDisposed()
		{
			if (_IsDisposed) throw new ObjectDisposedException(nameof(AsyncQueueLogWriter));
		}

		#endregion

		#region Event Handlers

		private void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			Flush();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Flush();
		}

		private void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			Flush();
		}

		#endregion

	}
}
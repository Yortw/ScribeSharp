using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class BufferedLogWriter : LogWriterBase
	{

		#region Fields

		private ILogWriter _LogWriter;
		private System.Collections.Concurrent.ConcurrentQueue<LogEvent> _BufferedLogEvents;
		private System.Threading.ManualResetEvent _WriteBufferedEventsSignal;
		private System.Threading.Timer _BufferTimeoutTimer;

		private int _BatchSize;
		private TimeSpan _WriteTimeout;

		private bool _IsTimerRunning;

		#endregion

		#region Constructors
		public BufferedLogWriter(ILogWriter logWriter, int batchSize, TimeSpan writeTimeout) : this(logWriter, batchSize, writeTimeout, null)
		{
		}

		public BufferedLogWriter(ILogWriter logWriter, int batchSize, TimeSpan writeTimeout, ILogEventFilter filter) : base(filter)
		{
			if (logWriter == null) throw new ArgumentNullException(nameof(logWriter));

			_BatchSize = batchSize;
			_WriteTimeout = writeTimeout;

			if (writeTimeout.TotalMilliseconds > 0)
				_BufferTimeoutTimer = new System.Threading.Timer((reserved) => SetWriteEventsSignal(), null, TimeSpan.Zero, TimeSpan.Zero);

			_WriteBufferedEventsSignal = new System.Threading.ManualResetEvent(false);
			_BufferedLogEvents = new System.Collections.Concurrent.ConcurrentQueue<LogEvent>();
			_LogWriter = logWriter;
			
			//TODO: Start thread that does write here.
			//TODO: Implement IDisposable.
		}

		#endregion

		#region Overrides

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			_BufferedLogEvents.Enqueue(logEvent);
			if (_BufferedLogEvents.Count > _BatchSize)
				SetWriteEventsSignal();
			else
				StartTimeoutTimer();
		}

		public override bool RequiresSynchronisation
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Private Methods

		private void StartTimeoutTimer()
		{
			if (!_IsTimerRunning)
			{
				_BufferTimeoutTimer.Change(TimeSpan.Zero, _WriteTimeout);
				_IsTimerRunning = true;
			}
		}

		private void SetWriteEventsSignal()
		{
			StopWriteTimeoutTimer();
			_WriteBufferedEventsSignal.Set();
		}

		private void StopWriteTimeoutTimer()
		{
			if (_BufferTimeoutTimer != null)
			{
				_BufferTimeoutTimer.Change(TimeSpan.Zero, TimeSpan.Zero);
				_IsTimerRunning = false;
			}
		}

		#endregion

	}
}
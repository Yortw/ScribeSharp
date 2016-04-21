using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Writers
{
	public sealed class AggregateLogWriter : LogWriterBase
	{
		#region Fields

		private IList<ILogWriter> _Writers;
		private AggregateLoggerWriteOptions _WriteOptions;
		private bool _RequiresSynchronisation;

		#endregion

		#region Constructors

		public AggregateLogWriter(IEnumerable<ILogWriter> writers) : this(writers, AggregateLoggerWriteOptions.Parallel, null)
		{
		}

		public AggregateLogWriter(IEnumerable<ILogWriter> writers, AggregateLoggerWriteOptions writeOptions, ILogEventFilter filter) : base(filter)
		{
			if (writers == null) throw new ArgumentNullException(nameof(writers));

			_WriteOptions = writeOptions;
			_Writers = new List<ILogWriter>(writers);
			_RequiresSynchronisation = _Writers.Any((w) => w.RequiresSynchronisation);
		}

		#endregion

		#region Overrides

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (_RequiresSynchronisation)
			{
				lock (_Writers)
				{
					UnsynchronisedWrite(logEvent);
				}
			}
			else
				UnsynchronisedWrite(logEvent);
		}

		public override bool RequiresSynchronisation
		{
			get
			{
				return _RequiresSynchronisation;
			}
		}

		#endregion

		#region Private Methods

		private void UnsynchronisedWrite(LogEvent logEvent)
		{
			if (_WriteOptions == AggregateLoggerWriteOptions.Parallel)
				WriteParallel(logEvent);
			else
			{
				WriteSerial(logEvent);
			}
		}

		private void WriteSerial(LogEvent logEvent)
		{
			List<LogWriterException> errors = null;
			foreach (var writer in _Writers)
			{
				try
				{
					writer.Write(logEvent);
				}
				catch (StackOverflowException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (_WriteOptions == AggregateLoggerWriteOptions.SerialToFirstError)
						throw;
					else
					{
						if (errors == null) errors = new List<LogWriterException>(_Writers.Count);
						errors.Add(new LogWriterException(writer, ex));
					}
				}
			}

			if (errors != null && errors.Count > 0)
				throw new AggregateException(errors.ToArray());
		}

		private void WriteParallel(LogEvent logEvent)
		{
			Parallel.For(0, _Writers.Count, (index) => _Writers[index].Write(logEvent));
		}

		#endregion

	}

	public enum AggregateLoggerWriteOptions
	{
		Parallel = 0,
		SerialToFirstError,
		SerialWithAggregateError 
	}
}
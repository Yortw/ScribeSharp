using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Writes events to multiple child <see cref="ILogWriter"/> implementations.
	/// </summary>
	public sealed class AggregateLogWriter : LogWriterBase, IDisposable
	{
		#region Fields

		private IList<ILogWriter> _Writers;
		private AggregateLoggerWriteOption _WriteOptions;
		private bool _RequiresSynchronisation;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <remarks>
		/// <para>The writer will be configured to write to the child writers in parallel.</para>
		/// </remarks>
		/// <param name="writers">An enumerable set of child <see cref="ILogWriter"/> implementations to write to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="writers"/> argument is null.</exception>
		public AggregateLogWriter(IEnumerable<ILogWriter> writers) : this(writers, AggregateLoggerWriteOption.Parallel)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="writers">An enumerable set of child <see cref="ILogWriter"/> implementations to write to.</param>
		/// <param name="writeOptions">A value from the <see cref="AggregateLoggerWriteOption"/> specifying how child writers are called and exceptions reported.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="writers"/> argument is null.</exception>
		public AggregateLogWriter(IEnumerable<ILogWriter> writers, AggregateLoggerWriteOption writeOptions) 
		{
			if (writers == null) throw new ArgumentNullException(nameof(writers));

			_WriteOptions = writeOptions;
			_Writers = new List<ILogWriter>(writers);
			_RequiresSynchronisation = _Writers.Any((w) => w.RequiresSynchronization);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Called when the <see cref="ILogWriter.Write(LogEvent)"/> method has been called and the writer's filter has passed the event.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
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

		/// <summary>
		/// Returns true if any of the child log writers require synchronisation.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return _RequiresSynchronisation;
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Dispsoes this instance and the underlying log writers.
		/// </summary>
		public void Dispose()
		{
			foreach (var writer in _Writers)
			{
				(writer as IDisposable)?.Dispose();
			}
		}

		#endregion

		#region Private Methods

		private void UnsynchronisedWrite(LogEvent logEvent)
		{
			if (_WriteOptions == AggregateLoggerWriteOption.Parallel)
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
				catch (Exception ex) when (!ex.ShouldRethrowImmediately())
				{
					if (_WriteOptions == AggregateLoggerWriteOption.SerialToFirstError)
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

	/// <summary>
	/// Specifies how a <see cref="AggregateLogWriter"/> instance writes to it's children and handles any resulting errors.
	/// </summary>
	public enum AggregateLoggerWriteOption
	{
		/// <summary>
		/// Log writers are written to in parallel. If any exceptions occur, an <see cref="AggregateException"/> will be thrown.
		/// </summary>
		Parallel = 0,
		/// <summary>
		/// Each log writer is written to one at a time in the order provided. If an exception occurs it is re-thrown and subsequent writers are not called.
		/// </summary>
		SerialToFirstError,
		/// <summary>
		/// Each log writer is written to one at a time in the order provided. All writer are called and all exceptions are rethrown inside an <see cref="AggregateException"/> after the last writer is called.
		/// </summary>
		SerialWithAggregateError
	}
}
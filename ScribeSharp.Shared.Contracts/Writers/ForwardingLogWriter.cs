using PoolSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer that writes to another <see cref="ILogger"/> implementation.
	/// </summary>
	public sealed class ForwardingLogWriter : LogWriterBase
	{

		#region Fields

		private ILogger _Logger;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="logger">A <see cref="ILogger"/> implementation to forward log events to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
		public ForwardingLogWriter(ILogger logger) : this(logger, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="logger">A <see cref="ILogger"/> implementation to forward log events to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to filter events before writing. If null no filtering is performed.</param>
		public ForwardingLogWriter(ILogger logger, ILogEventFilter filter) : base(filter)
		{
			if (logger == null) throw new ArgumentNullException("logger");

			_Logger = logger;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns true.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Writes the event to the configured <see cref="ILogger"/>.
		/// </summary>
		/// <param name="logEvent">A <see cref="LogEvent"/> instance to write.</param>
		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			logEvent.Clone(logEvent);
			_Logger.WriteEventWithSource(logEvent, logEvent.Source, logEvent.SourceMethod);
		}

		#endregion

	}
}
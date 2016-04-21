using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public sealed class ForwardingLogWriter : LogWriterBase
	{

		#region Fields

		private ILogger _Logger;

		#endregion

		#region Constructors

		public ForwardingLogWriter(ILogger logger) : this(logger, null)
		{
		}

		public ForwardingLogWriter(ILogger logger, ILogEventFilter filter) : base(filter)
		{
			if (logger == null) throw new ArgumentNullException("logger");

			_Logger = logger;
		}

		#endregion

		#region Overrides

		public override bool RequiresSynchronisation
		{
			get
			{
				return true;
			}
		}

		protected override void WriteFilteredEvent(LogEvent logEvent)
		{
			_Logger.WriteEvent(logEvent, logEvent.Source, logEvent.SourceMethod);
		}

		#endregion

	}
}
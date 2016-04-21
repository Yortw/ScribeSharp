using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	public abstract class LogWriterBase : ILogWriter
	{

		private readonly ILogEventFilter _Filter;

		public LogWriterBase()
		{
		}

		public LogWriterBase(ILogEventFilter filter)
		{
			_Filter = filter;
		}

		public abstract bool RequiresSynchronisation
		{
			get;
		}

		public void Write(LogEvent logEvent)
		{
			try
			{
				if (_Filter?.ShouldProcess(logEvent) ?? true)
				{
					WriteFilteredEvent(logEvent);
				}
			}
			catch (StackOverflowException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LogWriterException(this, ex);
			}
		}

		protected abstract void WriteFilteredEvent(LogEvent logEvent);
	}
}
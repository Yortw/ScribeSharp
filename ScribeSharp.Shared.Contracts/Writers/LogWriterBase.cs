using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A base close for <see cref="ILogWriter"/> implementations containing reusable logic.
	/// </summary>
	public abstract class LogWriterBase : ILogWriter
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected LogWriterBase()
		{
		}

		/// <summary>
		/// Abstract method, must be overriden by derived classes. See <see cref="ILogWriter"/>.
		/// </summary>
		public abstract bool RequiresSynchronization
		{
			get;
		}

		/// <summary>
		/// Checks the supplied <see cref="LogEvent"/> instance against the configured <see cref="ILogWriter"/> instance and calls <see cref="WriteEventInternal(LogEvent)"/> if the filter passes.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to write.</param>
		public void Write(LogEvent logEvent)
		{
			try
			{
				WriteEventInternal(logEvent);
			}
			catch (LogWriterException) { throw; }
			catch (Exception ex) when (!ex.ShouldRethrowImmediately())
			{
				throw new LogWriterException(this, ex);
			}
		}

		/// <summary>
		/// Abstract method to be overridden by derived classes. Should actually write the specified <see cref="LogEvent"/> to the output location.
		/// </summary>
		/// <param name="logEvent"></param>
		protected abstract void WriteEventInternal(LogEvent logEvent);
	}
}
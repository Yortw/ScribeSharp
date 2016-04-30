using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that log, report, and decide how to handle errors reporting by the logging system.
	/// </summary>
	public interface ILoggingErrorHandler
	{

		/// <summary>
		/// Event raised when <see cref="ReportError(Exception)"/> is called.
		/// </summary>
		/// <seealso cref="ReportError(Exception)"/>
		event EventHandler<LoggingErrorEventArgs> Error;

		/// <summary>
		/// Called by logging components to report errors and determine how they should be handled.
		/// </summary>
		/// <param name="exception"></param>
		/// <seealso cref="Error"/>
		LoggingErrorPolicy ReportError(Exception exception);

	}

	/// <summary>
	/// Event arguments for the <see cref="ILoggingErrorHandler.Error"/> event.
	/// </summary>
	public class LoggingErrorEventArgs : EventArgs
	{
		private readonly Exception _Exception;
		private LoggingErrorPolicy _HandleErrorRecommendation;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="exception">The exception that was reported.</param>
		public LoggingErrorEventArgs(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			_Exception = exception;
		}

		/// <summary>
		/// Returns the exception that was reported and caused the event to be raised.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _Exception;
			}
		}

		/// <summary>
		/// Sets or returns a recommendation/request for how the error should be handled. 
		/// </summary>
		/// <remarks>
		/// <para>The component that raised the event can choose whether to use the recommendation provided or make it's own decision.</para>
		/// </remarks>
		public LoggingErrorPolicy HandleErrorRecommendation
		{
			get
			{
				return _HandleErrorRecommendation;
			}

			set
			{
				_HandleErrorRecommendation = value;
			}
		}
	}

	/// <summary>
	/// Specifies how a logging error is handled.
	/// </summary>
	public enum LoggingErrorPolicy
	{
		/// <summary>
		/// Indicates no preference specified. The behaviour is undefined and may vary depending on where the error is handled.
		/// </summary>
		Default,
		/// <summary>
		/// The error is ignored.
		/// </summary>
		Suppress,
		/// <summary>
		/// The error is re-thrown. If it is not already a <see cref="LogException"/> or derived exception, it is wrapped in a new <see cref="LogException"/> instance.
		/// </summary>
		Rethrow
	}
}
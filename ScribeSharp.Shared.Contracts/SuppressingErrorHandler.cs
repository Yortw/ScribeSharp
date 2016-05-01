using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An <see cref="ILoggingErrorHandler"/> implementation that always suppresses reported exceptions.
	/// </summary>
	public class SuppressingErrorHandler : ILoggingErrorHandler
	{
		private static SuppressingErrorHandler s_DefaultInstance;

		/// <summary>
		/// Raised when the <see cref="ReportError(Exception)"/> method is called.
		/// </summary>
		public event EventHandler<LoggingErrorEventArgs> ErrorOccurred;

		/// <summary>
		/// Raises the <see cref="ErrorOccurred"/> event, but always returns <see cref="LoggingErrorPolicy.Suppress"/>.
		/// </summary>
		/// <param name="exception">The exception that occurred.</param>
		/// <returns>Returns <see cref="LoggingErrorPolicy.Suppress"/>.</returns>
		public LoggingErrorPolicy ReportError(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			System.Diagnostics.Trace.WriteLine("Logging Error: " + exception.ToString(), "loggingerror");

			ErrorOccurred?.Invoke(this, new LoggingErrorEventArgs(exception));

			return LoggingErrorPolicy.Suppress;
		}

		/// <summary>
		/// Returns a single default instance. Using this instance can reduce allocations and improve performance.
		/// </summary>
		public static SuppressingErrorHandler DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new SuppressingErrorHandler()); }
		}
	}
}
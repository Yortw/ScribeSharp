﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An <see cref="ILoggingErrorHandler"/> implementation that always rethrows reported exceptions.
	/// </summary>
	public class RethrowErrorHandler : ILoggingErrorHandler
	{
		private static RethrowErrorHandler s_DefaultInstance;

		/// <summary>
		/// Raised when the <see cref="ReportError(Exception)"/> method is called.
		/// </summary>
		public event EventHandler<LoggingErrorEventArgs> ErrorOccurred;

		/// <summary>
		/// Raises the <see cref="ErrorOccurred"/> event, but always returns <see cref="LoggingErrorPolicy.Rethrow"/>.
		/// </summary>
		/// <param name="exception">The exception that occurred.</param>
		/// <returns>Returns <see cref="LoggingErrorPolicy.Rethrow"/>.</returns>
		public LoggingErrorPolicy ReportError(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

#if SUPPORTS_TRACE   
			System.Diagnostics.Trace.WriteLine("Logging Error: " + exception.ToString(), "loggingerror");
#endif

			ErrorOccurred?.Invoke(this, new LoggingErrorEventArgs(exception));

			return LoggingErrorPolicy.Rethrow;
		}

		/// <summary>
		/// Returns a single default instance. Using this instance can reduce allocations and improve performance.
		/// </summary>
		public static RethrowErrorHandler DefaultInstance
		{
			get { return s_DefaultInstance ?? (s_DefaultInstance = new RethrowErrorHandler()); }
		}
	}
}
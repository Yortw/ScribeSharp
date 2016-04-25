using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// An exception that occured while trying to output a <see cref="LogEvent"/> to a final output location.
	/// </summary>
#if SUPPORTS_SERIALISATION
	[Serializable]
#endif
	public class LogWriterException : LogException
	{
		[NonSerialized]
		private readonly ILogWriter _LogWriter;

		/// <summary>
		/// Recommended constructor. Takes a reference to the <see cref="ILogWriter"/> that failed to output the event, and the exception that log write threw.
		/// </summary>
		/// <param name="logWriter">A reference to the <see cref="ILogWriter"/> that failed to output the event.</param>
		/// <param name="innerException">The exception that occurred outputting the event.</param>
		public LogWriterException(ILogWriter logWriter, Exception innerException) : this(innerException?.Message ?? Properties.Resources.GenericLogWriterExceptionMessage, innerException)
		{
			_LogWriter = logWriter;
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogWriterException() : this(Properties.Resources.GenericLogWriterExceptionMessage) { }
		/// <summary>
		/// Partial constructor which takes a custom error message.
		/// </summary>
		/// <param name="message">The error message to associated with this exception instance.</param>
		public LogWriterException(string message) : base(message) { }
		/// <summary>
		/// Full constructor taking a custom error message and the original exception wrapped by thsi instance.
		/// </summary>
		/// <param name="message">The custom error message. If null will be replaced by the inner exception error message.</param>
		/// <param name="inner">The exception that is being wrapped by this instance.</param>
		public LogWriterException(string message, Exception inner) : base(message, inner) { }

#if SUPPORTS_SERIALISATION
		/// <summary>
		/// Deserialisation cosntructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LogWriterException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
#endif

		/// <summary>
		/// A reference to the <see cref="ILogWriter"/> that failed to output the log event.
		/// </summary>
		/// <remarks>
		/// <para>This property does not serialise.</para>
		/// </remarks>
		public ILogWriter LogWriter
		{
			get
			{
				return _LogWriter;
			}
		}
	}
}
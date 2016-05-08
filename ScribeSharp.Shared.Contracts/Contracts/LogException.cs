using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Represents an error that occurred within the logging system.
	/// </summary>
	/// <remarks>
	/// <para>See the <see cref="Exception.InnerException"/> property for details of the error that occurred.</para>
	/// </remarks>
#if SUPPORTS_SERIALISATION
	[Serializable]
#endif
	public class LogException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogException() : this(Properties.Resources.ResourceManager.GetString("GenericLogExceptionMessage")) { } 
		/// <summary>
		/// Partial constructor which takes a custom error message.
		/// </summary>
		/// <param name="message">The error message to associated with this exception instance.</param>
		public LogException(string message) : base(message) { }
		/// <summary>
		/// Full constructor taking a custom error message and the original exception wrapped by thsi instance.
		/// </summary>
		/// <param name="message">The custom error message. If null will be replaced by the inner exception error message.</param>
		/// <param name="inner">The exception that is being wrapped by this instance.</param>
		public LogException(string message, Exception inner) : base(message ?? inner?.Message, inner) { }

#if SUPPORTS_SERIALISATION
		/// <summary>
		/// Deserialisation cosntructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LogException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
#endif
	}
}
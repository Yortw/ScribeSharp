using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	[Serializable]
	public class LogWriterException : LogException
	{
		[NonSerialized]
		private readonly ILogWriter _LogWriter;

		public LogWriterException(ILogWriter logWriter, Exception innerException) : this(innerException.Message, innerException)
		{
			_LogWriter = logWriter;
		}

		public LogWriterException() { }
		public LogWriterException(string message) : base(message) { }
		public LogWriterException(string message, Exception inner) : base(message, inner) { }
		protected LogWriterException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }


		public ILogWriter LogWriter
		{
			get
			{
				return _LogWriter;
			}
		}
	}
}
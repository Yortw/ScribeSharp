using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	[Serializable]
	public class LogException : Exception
	{
		public LogException() { }
		public LogException(string message) : base(message) { }
		public LogException(string message, Exception inner) : base(message, inner) { }
		protected LogException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public sealed class JsonLogEventFormatter : ILogEventFormatter
	{
		
		public string Format(LogEvent logEvent)
		{
			//TODO: Serialise to json.
			throw new NotImplementedException();
		}
	}
}
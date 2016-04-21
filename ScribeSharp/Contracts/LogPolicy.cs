using ScribeSharp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public class LogPolicy
	{
		public ILogWriter LogWriter { get; set; }
		public ILogClock Clock { get; set; }
		public ILogEventFilter Filter { get; set;}
		public IEnumerable<ILogEventContextProvider> ContextProviders { get; set; }
	}
}
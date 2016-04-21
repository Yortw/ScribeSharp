using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public interface ILogEventFormatter
	{
		string Format(LogEvent logEvent);
	}
}
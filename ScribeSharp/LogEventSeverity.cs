using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	public enum LogEventSeverity
	{
		Diagnostic = 0,
		Verbose,
		Debug,
		Information,
		Warning,
		Error,
		CriticalError,
		FatalError
	}
}
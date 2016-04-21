using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	internal static class CachedCurrentProcess
	{
		private static System.Diagnostics.Process _CurrentProcess;

		public static System.Diagnostics.Process CurrentProcess
		{
			get { return _CurrentProcess ?? (_CurrentProcess = System.Diagnostics.Process.GetCurrentProcess()); }
		}
	}
}
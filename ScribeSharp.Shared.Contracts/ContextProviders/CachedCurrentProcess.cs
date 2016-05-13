#if SUPPORTS_PROCESS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Internal class used to cache the current process name improve performance of repeat access.
	/// </summary>
	internal static class CachedCurrentProcess
	{
		private static System.Diagnostics.Process _CurrentProcess;

		/// <summary>
		/// Returns the name of the current process.
		/// </summary>
		public static System.Diagnostics.Process CurrentProcess
		{
			get { return _CurrentProcess ?? (_CurrentProcess = System.Diagnostics.Process.GetCurrentProcess()); }
		}
	}
}

#endif
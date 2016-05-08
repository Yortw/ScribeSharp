using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// A class that represents a minimum severity level which can be changed at runtime.
	/// </summary>
	public class LogEventSeveritySwitch : ILogEventSeveritySwitch
	{

		private LogEventSeverity _MinimumSeverity;
		
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="minimumSeverity">The initial value for the <see cref="MinimumSeverity"/> property.</param>
		public LogEventSeveritySwitch(LogEventSeverity minimumSeverity)
		{
			_MinimumSeverity = minimumSeverity;
		}

		/// <summary>
		/// Sets or returns the minimum severity level allowed.
		/// </summary>
		public LogEventSeverity MinimumSeverity
		{
			get { return _MinimumSeverity; }
			set { _MinimumSeverity = value; }
		}

		/// <summary>
		/// Returns true if the specified <see cref="LogEventSeverity"/> value is allowed based on the <see cref="MinimumSeverity"/> property.
		/// </summary>
		/// <param name="severity">The <see cref="LogEventSeverity"/> vaalue to test.</param>
		/// <returns>True if the severity provided is allowed by this switch (is at least the minimum level currently specified).</returns>
		public bool IsAllowed(LogEventSeverity severity)
		{
			return severity >= _MinimumSeverity;
		}
	}
}
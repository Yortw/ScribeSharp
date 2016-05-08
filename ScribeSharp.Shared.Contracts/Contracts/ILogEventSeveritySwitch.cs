namespace ScribeSharp
{
	/// <summary>
	/// An interface for obects that represent a minimum severity level which can be changed at runtime.
	/// </summary>
	public interface ILogEventSeveritySwitch
	{
		/// <summary>
		/// The minimum allowed log event severity.
		/// </summary>
		LogEventSeverity MinimumSeverity { get; set; }

		/// <summary>
		/// Returns true if the specified <see cref="LogEventSeverity"/> value is allowed based on the <see cref="MinimumSeverity"/> property.
		/// </summary>
		/// <param name="severity">The <see cref="LogEventSeverity"/> vaalue to test.</param>
		/// <returns>True if the severity provided is allowed by this switch (is at least the minimum level currently specified).</returns>
		bool IsAllowed(LogEventSeverity severity);
	}
}
namespace ScribeSharp.Writers
{
	/// <summary>
	/// Events arguments for the <see cref="ListLogWriter.LogEventWritten"/> event.
	/// </summary>
	public class LogEventWrittenEventArgs : System.EventArgs
	{
		private readonly LogEvent logEvent;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance that was written.</param>
		/// <seealso cref="LogEvent"/>
		public LogEventWrittenEventArgs(LogEvent logEvent)
		{
			this.logEvent = logEvent;
		}

		/// <summary>
		/// The <see cref="LogEvent"/> instance that was written.
		/// </summary>
		/// <seealso cref="LogEventWrittenEventArgs(LogEvent)"/>
		public LogEvent LogEvent
		{
			get
			{
				return logEvent;
			}
		}
	}
}
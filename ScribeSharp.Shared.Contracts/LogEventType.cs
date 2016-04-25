namespace ScribeSharp
{
	/// <summary>
	/// Specifies a general type or category of event being logged.
	/// </summary>
	public enum LogEventType
	{
		/// <summary>
		/// Something the application did that is not specifically related to user activity. This is the default event type.
		/// </summary>
		ApplicationEvent = 0,
		/// <summary>
		/// A scheduled job, or action taken at a scheduled time.
		/// </summary>
		ScheduledEvent,
		/// <summary>
		/// A record of something the user explicitly started or did, such as a button click or hot key press.
		/// </summary>
		UserAction,
		/// <summary>
		/// An event started by the system, perhaps as the result of a network call or change in configuration.
		/// </summary>
		SystemEvent,
		/// <summary>
		/// Indicate the log entry contains information about or relates to performance of the system.
		/// </summary>
		Performance,
		/// <summary>
		/// Indicates an action that failed but is not considered an error.
		/// </summary>
		Failure,
		/// <summary>
		/// Indicates the permission, elevation or some other security access was succesfully granted to the application or user.
		/// </summary>
		SecurityAuditSuccess,
		/// <summary>
		/// Indicates the permission, elevation or some other security access was denied to the application or user.
		/// </summary>
		SecurityAuditFailure,
		/// <summary>
		/// Indicates the start of an application, job or action.
		/// </summary>
		Start,
		/// <summary>
		/// Indicates an application, job or action was paused.
		/// </summary>
		Pause,
		/// <summary>
		/// Indicates an application, job or action was resumed.
		/// </summary>
		Resume,
		/// <summary>
		/// Indicates an application, job or action was cancelled.
		/// </summary>
		Canceled,
		/// <summary>
		/// Indicates an application, job or action finished.
		/// </summary>
		Completed
	}
}
namespace ScribeSharp
{
	public enum LogEventType
	{
		ApplicationEvent = 0,
		ScheduledEvent,
		UserAction,
		SystemEvent,
		Performance,
		Failure,
		SecurityAuditSuccess,
		SecurityAuditFailure,
		Start,
		Pause,
		Resume,
		Stop
	}
}
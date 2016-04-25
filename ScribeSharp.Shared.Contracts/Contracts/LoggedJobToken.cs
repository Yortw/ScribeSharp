using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ScribeSharp
{
	/// <summary>
	/// A small token used to represent an unit of work (a "job") that may have multiple log entries associated with it.
	/// Automatically includes the duration of the job from start to finish, and additional properties for the job such as name and id.
	/// Implements <see cref="IDisposable"/> pattern for easy tracking of job start and end, as well as additional methods and properties for easily logging events that will be related to the job in the log file.
	/// </summary>
	/// <remarks>
	/// <para>Typically a <see cref="LoggedJobToken"/> is created by calling <see cref="ILogger.BeginLoggedJob(string, string)"/> or one of it's overloads. The job should be completed by calling the <see cref="Dispose"/> method.</para>
	/// <para>Call the <see cref="SetFailure(Exception)"/> method to record exceptions/failures that occur.</para>
	/// <para>Use the <see cref="Logger"/> property to log events and have them include the job details.</para>
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification="Really there is no 'value' based equality for this type, reference equality is correct.")]
	public struct LoggedJobToken : IDisposable
	{

		#region Fields

		private ILogger _Logger;
		private ILogger _JobLogger;

		private string _JobName;
		private string _JobId;
		private IEnumerable<KeyValuePair<string, object>> _Properties;
		private Exception _Exception;
		private System.Diagnostics.Stopwatch _Stopwatch;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="logger">The logger associated with the job.</param>
		/// <param name="jobName">The name/type/description of the job running.</param>
		/// <param name="jobId">A unique identifier to apply to the job.</param>
		/// <param name="properties">An additional set of property to be included on all log entries related to this job.</param>
		public LoggedJobToken(ILogger logger, string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			_Stopwatch = new System.Diagnostics.Stopwatch();
			_Logger = logger;
			_JobId = jobId;
			_JobName = jobName;
			_Properties = GetExtendedProperties(properties,
				new KeyValuePair<string, object>("Job Name", jobName),
				new KeyValuePair<string, object>("Job Id", jobId)
			).ToArray();
			_JobLogger = null;
			_Exception = null;

			_Logger.WriteEvent(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobStartedEventMessage, jobName, jobId),
				eventSeverity: LogEventSeverity.Information,
				eventType: LogEventType.Start,
				properties: _Properties
			);

			_Stopwatch.Start();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Returns a logger based on the original that automatically includes the job id, job name and any additional job properties.
		/// </summary>
		public ILogger Logger
		{
			get
			{
				if (_JobLogger == null)
				{
					var list = new List<ILogEventContextProvider>(2);
					if (!String.IsNullOrWhiteSpace(Properties.Resources.JobNamePropertyName))
						list.Add(new ContextProviders.FixedStringLogEntryContextProvider(Properties.Resources.JobNamePropertyName, _JobName));
					if (!String.IsNullOrWhiteSpace(Properties.Resources.JobNamePropertyName))
						list.Add(new ContextProviders.FixedStringLogEntryContextProvider(Properties.Resources.JobIdPropertyName, _JobId));

					_JobLogger = _Logger.CreateChildLogger(new LogPolicy()
					{
						ContextProviders = list
					});
				}

				return _JobLogger;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Records a failure associated with the job. Will also cause the job completion message to be written with an error severity instead of information.
		/// </summary>
		/// <param name="exception">The exception representing the failure.</param>
		/// <remarks>
		/// <para>If multiple exceptions are recorded, each will be logged individually but the completion message will reference only the first exception.</para>
		/// </remarks>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
		public void SetFailure(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));

			if (_Exception == null)
				_Exception = exception;

			_Logger.WriteEvent(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobFailureEventMessage, _JobName, _JobId, exception.GetType().FullName + ":" + exception.Message),
				eventSeverity: LogEventSeverity.Error,
				eventType: LogEventType.Failure,
				properties: GetExtendedProperties(_Properties, new KeyValuePair<string, object>("Exception", exception))
			);
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Stops timing and writes the job completed event.
		/// </summary>
		public void Dispose()
		{
			_Stopwatch.Stop();

			_Logger.WriteEvent(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobFinishedEventMessage, _JobName, _JobId),
				eventSeverity: _Exception == null ? LogEventSeverity.Information : LogEventSeverity.Error,
				eventType: LogEventType.Completed,
				properties: GetExtendedProperties(_Properties,
				new KeyValuePair<string, object>("Exception", _Exception),
				new KeyValuePair<string, object>("Duration", _Stopwatch.Elapsed))
			);
		}

		#endregion

		#region Private Methods

		private static IEnumerable<KeyValuePair<string, object>> GetExtendedProperties(IEnumerable<KeyValuePair<string, object>> properties, params KeyValuePair<string, object>[] additonalProperties)
		{
			return (properties ?? EmptyArray.Empty<KeyValuePair<string, object>>()).Union(
				additonalProperties
			);
		}

		#endregion

	}
}
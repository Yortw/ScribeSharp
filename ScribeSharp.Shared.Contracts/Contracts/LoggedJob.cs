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
	/// <para>Typically a <see cref="LoggedJob"/> is created by calling <see cref="ILogger.BeginLoggedJob(string, string)"/> or one of it's overloads. The job should be completed by calling the <see cref="Dispose"/> method.</para>
	/// <para>Call the <see cref="SetFailure(Exception)"/> method to record exceptions/failures that occur.</para>
	/// <para>Use the <see cref="Logger"/> property to log events and have them include the job details.</para>
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification="Really there is no 'value' based equality for this type, reference equality is correct.")]
	public sealed class LoggedJob : IDisposable
	{

		#region Fields

		private ILogger _Logger;
		private ILogger _JobLogger;

		private string _JobName;
		private string _JobId;
		private KeyValuePair<string, object>[] _Properties;
		private Exception _Exception;
		private System.Diagnostics.Stopwatch _Stopwatch;
		private ILoggedJobPool _ParentPool;
		private bool _Cancelled;
		private TimeSpan _MaxExpectedDuration;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoggedJob()
		{
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
						list.Add(new ContextProviders.FixedValueLogEventContextProvider(Properties.Resources.JobNamePropertyName, _JobName));
					if (!String.IsNullOrWhiteSpace(Properties.Resources.JobNamePropertyName))
						list.Add(new ContextProviders.FixedValueLogEventContextProvider(Properties.Resources.JobIdPropertyName, _JobId));

					_JobLogger = _Logger.CreateChildLogger(new LogPolicy()
					{
						ContextProviders = list
					});
				}

				return _JobLogger;
			}
		}

		/// <summary>
		/// Returns the unique id of this job.
		/// </summary>
		public string Id { get { return _JobId; } }

		/// <summary>
		/// Returns the human readable name of this job.
		/// </summary>
		public string Name { get { return _JobId; } }
		
		/// <summary>
		/// Returns the length of time this job has currently been running, or how long it ran for if it has already completed.
		/// </summary>
		public TimeSpan Duration { get { return _Stopwatch.Elapsed; } }

		#endregion

		#region Public Methods

		/// <summary>
		/// Initialises this instance with values relating to a particular job.
		/// </summary>
		/// <param name="logger">The logger associated with the job.</param>
		/// <param name="jobName">The name/type/description of the job running.</param>
		/// <param name="jobId">A unique identifier to apply to the job.</param>
		/// <param name="properties">An additional set of property to be included on all log entries related to this job.</param>
		/// <param name="parentPool">Either null, or pool instance the job should be returned to when complete, so this instance can be reused.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="logger"/> or <paramref name="jobName"/> arguments are null.</exception>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="jobName"/> argument is empty or only whitespace.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public void Initialize(ILogger logger, string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties, ILoggedJobPool parentPool)
		{
			Initialize(logger, jobName, jobId, properties, parentPool, TimeSpan.Zero);
		}

		/// <summary>
		/// Initialises this instance with values relating to a particular job.
		/// </summary>
		/// <param name="logger">The logger associated with the job.</param>
		/// <param name="jobName">The name/type/description of the job running.</param>
		/// <param name="jobId">A unique identifier to apply to the job.</param>
		/// <param name="properties">An additional set of property to be included on all log entries related to this job.</param>
		/// <param name="parentPool">Either null, or pool instance the job should be returned to when complete, so this instance can be reused.</param>
		/// <param name="maxExpectedDuration">The maximum expected duration of the work. If the work takes longer than this, the job complete event will be marked with a warning severity.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="logger"/> or <paramref name="jobName"/> arguments are null.</exception>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="jobName"/> argument is empty or only whitespace.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public void Initialize(ILogger logger, string jobName, string jobId, IEnumerable<KeyValuePair<string, object>> properties, ILoggedJobPool parentPool, TimeSpan maxExpectedDuration)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (jobName == null) throw new ArgumentNullException(nameof(jobName));
			if (String.IsNullOrWhiteSpace(jobName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, jobName), nameof(jobName));

			if (String.IsNullOrWhiteSpace(jobId)) jobId = Guid.NewGuid().ToString();

			_ParentPool = parentPool;
			_Stopwatch = new System.Diagnostics.Stopwatch();
			_Logger = logger;
			_JobId = jobId;
			_JobName = jobName;
			_MaxExpectedDuration = maxExpectedDuration;
			_Properties = GetExtendedProperties(properties,
				new KeyValuePair<string, object>(Properties.Resources.JobNamePropertyName, jobName),
				new KeyValuePair<string, object>(Properties.Resources.JobIdPropertyName, jobId)
			).ToArray();

			_Logger.WriteEventWithSource(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobStartedEventMessage, jobName, jobId),
				eventSeverity: LogEventSeverity.Information,
				eventType: LogEventType.Start,
				properties: _Properties
			);

			_Stopwatch.Start();
		}

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

			_Logger.WriteEventWithSource(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobFailureEventMessage, _JobName, _JobId, exception.GetType().FullName + ":" + exception.Message),
				eventSeverity: LogEventSeverity.Error,
				eventType: LogEventType.Failure,
				properties: GetExtendedProperties(_Properties, new KeyValuePair<string, object>("Exception", exception)).ToArray()
			);
		}

		/// <summary>
		/// Clears or resets all internal state of this instance so it can be reused.
		/// </summary>
		public void Reset()
		{
			_Logger = null;
			_JobLogger = null;
			_JobName = null;
			_JobId = null;
			_Exception = null;
			_Properties = null;
			_Stopwatch.Reset();
		}

		/// <summary>
		/// Marks the job as cancelled, changing the event written when the job completes.
		/// </summary>
		public void Cancel()
		{
			_Cancelled = true;
		}

		#region CreateChildJob Overloads

		/// <summary>
		/// Creates a new job from the same pool, using the same logger and original properties as well as the new properties provided, and with a "Parent Job ID" property containing the Id of this job for correlation purposes.
		/// </summary>
		/// <param name="jobName">The name or type of the job to create.</param>
		/// <param name="jobId">A unique id of the job used to correlate log entries.</param>
		/// <returns>A <see cref="LoggedJob"/> instance used to track the job and log related entries.</returns>
		public LoggedJob CreateChildJob(string jobName, string jobId)
		{
			return CreateChildJob(jobName, jobId, null);
		}

		/// <summary>
		/// Creates a new job from the same pool, using the same logger and original properties as well as the new properties provided, and with a "Parent Job ID" property containing the Id of this job for correlation purposes.
		/// </summary>
		/// <param name="jobName">The name or type of the job to create.</param>
		/// <param name="jobId">A unique id of the job used to correlate log entries.</param>
		/// <param name="properties">Null, or a collection key value pairs to associate with the child job.</param>
		/// <returns>A <see cref="LoggedJob"/> instance used to track the job and log related entries.</returns>
		public LoggedJob CreateChildJob(string jobName, string jobId, params KeyValuePair<string, object>[] properties)
		{
			var retVal = _ParentPool?.Take() ?? new LoggedJob();
			IEnumerable<KeyValuePair<string, object>> allProperties = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(Properties.Resources.ParentJobIdPropertyName, this._JobId) };
			if (properties != null)
				allProperties = allProperties.Union(GetNonJobProperties(properties));
			if (_Properties != null)
				allProperties = allProperties.Union(GetNonJobProperties(_Properties));

			retVal.Initialize(_Logger, jobName, jobId, allProperties, _ParentPool, TimeSpan.Zero);
			return retVal;
		}

		#endregion

		#endregion

		#region IDisposable

		/// <summary>
		/// Stops timing and writes the job completed event. If a parent pool was supplied, returns the item to the pool.
		/// </summary>
		public void Dispose()
		{
			_Stopwatch.Stop();

			var severity = LogEventSeverity.Information;
			if (_Exception != null)
				severity = LogEventSeverity.Error;
			else if (_Cancelled || (_Stopwatch.Elapsed > _MaxExpectedDuration && _MaxExpectedDuration != TimeSpan.Zero))
				severity = LogEventSeverity.Warning;

			_Logger.WriteEventWithSource(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.JobFinishedEventMessage, _JobName, _JobId),
				eventSeverity: severity,
				eventType: _Cancelled ? LogEventType.Canceled : LogEventType.Completed,
				properties: GetExtendedProperties(
					_Properties,
					new KeyValuePair<string, object>("Exception", _Exception),
					new KeyValuePair<string, object>("Duration", _Stopwatch.Elapsed),
					new KeyValuePair<string, object>("Outcome", _Exception != null ? Properties.Resources.FailedOutcome : (_Cancelled ? Properties.Resources.CancelledOutcome : Properties.Resources.CompletedOutcome))
				).ToArray()
			);

			_ParentPool?.Add(this);
		}

		#endregion

		#region Private Methods

		private static IEnumerable<KeyValuePair<string, object>> GetNonJobProperties(IEnumerable<KeyValuePair<string, object>> properties)
		{
			return (from p
							in properties
							where p.Key != Properties.Resources.JobIdPropertyName
								&& p.Key != Properties.Resources.JobNamePropertyName
								&& p.Key != Properties.Resources.ParentJobIdPropertyName
							select p);
		}

		private static IEnumerable<KeyValuePair<string, object>> GetExtendedProperties(IEnumerable<KeyValuePair<string, object>> properties, params KeyValuePair<string, object>[] additonalProperties)
		{
			return (properties ?? EmptyArray.Empty<KeyValuePair<string, object>>()).Union(
				additonalProperties
			);
		}

		#endregion

	}
}
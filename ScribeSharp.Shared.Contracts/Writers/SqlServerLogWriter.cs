#if SUPPORTS_SQLCLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using ScribeSharp.Infrastructure;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A log writer for writing log events to a Microsoft Sql Server table.
	/// </summary>
	public sealed class SqlServerLogWriter : LogWriterBase, IBatchLogWriter
	{

		#region Fields

		private string _ConnectionString;
		private string _TableName;
		private ILogEventFormatter _LogEventFormatter;

		private IDictionary<string, string> _ColumnMappings;

		private const string SourceColumn_SerialisedLogEvent = "FullDetails";
		private const string SourceColumn_LogSeverityLevel = "SeverityLevel";

		#region Sql Server Exception Constants

		#region Retryable exceptions

		private const int SQL_EXCEPTION_NOLOCKDATAMOVEMENT = 601;
		private const int SQL_EXCEPTION_TIMEOUTBUFFERLATCH1 = 845;
		private const int SQL_EXCEPTION_TIMEOUTBUFFERLATCH2 = 846;
		private const int SQL_EXCEPTION_TIMEOUTBUFFERLATCH3 = 847;
		private const int SQL_EXCEPTION_DATABASETRANSITION = 913;
		private const int SQL_EXCEPTION_DATABASETNOTYETRECOVERED = 921;
		private const int SQL_EXCEPTION_DATABASETNOTYETRECOVERED1 = 922;
		private const int SQL_EXCEPTION_ATTEMPTEDTOLOCKRESOURCENOTOWNED = 1203;
		private const int SQL_EXCEPTION_LOCKCOULDNOTBEOBTAINED = 1204;
		private const int SQL_EXCEPTION_DEADLOCKED = 1205;
		private const int SQL_EXCEPTION_ATTEMPTEDTORELEASELOCKSNOTINTRANSACTION = 1221;
		private const int SQL_EXCEPTION_LOCKREQUESTTIMEOUT = 1222;
		private const int SQL_EXCEPTION_DATABASEMIRRORBUSY = 1404;
		private const int SQL_EXCEPTION_REMOTECOMMUNICATIONSFAILEDBEFOREMIRRORINGSTARTED = 1413;
		private const int SQL_EXCEPTION_REMOTECOMMUNICATIONSTIMEOUT = 1421;
		private const int SQL_EXCEPTION_PARTNERANDWITNESSUNAVAILABLE = 1431;
		private const int SQL_EXCEPTION_CANNOTSHAREEXTENT = 1533;
		private const int SQL_EXCEPTION_PAGEEXTENTNOTFOUND = 1534;
		private const int SQL_EXCEPTION_CANNOTSHAREEXTENTDIRECTORYFULL = 1535;
		private const int SQL_EXCEPTION_CANNOTEXCLUSIVLEYLOCKDATABASE = 1807;
		private const int SQL_EXCEPTION_DBCCMEMOBJLISTFAILED = 2502;
		private const int SQL_EXCEPTION_DBCCFAILEDINTERNALQUERYERROR = 2509;
		private const int SQL_EXCEPTION_TRANSACTIONDOOMEDINTRIGGER = 3616;
		private const int SQL_EXCEPTION_FLOATINGPOINTEXCEPTION = 3628;
		private const int SQL_EXCEPTION_ERRORPROCESSINGMETADATA = 3635;
		private const int SQL_EXCEPTION_SNAPSHOTFAILEDWHENTRANSACTIONSTARTED = 3957;
		private const int SQL_EXCEPTION_INTERNALERRORDURINGREMOTEQUERY = 4128;
		private const int SQL_EXCEPTION_DATABASECOULDNOTBEEXCLUSIVLEYLOCKED = 5030;
		private const int SQL_EXCEPTION_FILEAUTOGROWINPROGRESS = 5034;
		private const int SQL_EXCEPTION_DBCCLOCKREQUESTTIMEOUT = 5245;
		private const int SQL_EXCEPTION_TIMEOUTOPTIMISINGQUERY = 8628;
		private const int SQL_EXCEPTION_TIMEOUTWAITINGFORMEMORY = 8645;
		private const int SQL_EXCEPTION_COULDNOTEXECUTETRIGGER = 20041;
		private const int SQL_EXCEPTION_TRANSPORTLEVELERROR = 233;
		private const int SQL_EXCEPTION_TRANSPORTLEVELERROR2 = 232;
		private const int SQL_EXCEPTION_INTERNALQUERYPROCESSORUNEXPECTEDERROR = 8630;

		private static readonly int[] RetryableSqlErrorCodes = new int[]
		{
			SQL_EXCEPTION_NOLOCKDATAMOVEMENT,
			SQL_EXCEPTION_TIMEOUTBUFFERLATCH1,
			SQL_EXCEPTION_TIMEOUTBUFFERLATCH2,
			SQL_EXCEPTION_TIMEOUTBUFFERLATCH3,
			SQL_EXCEPTION_DATABASETRANSITION,
			SQL_EXCEPTION_DATABASETNOTYETRECOVERED,
			SQL_EXCEPTION_DATABASETNOTYETRECOVERED1,
			SQL_EXCEPTION_ATTEMPTEDTOLOCKRESOURCENOTOWNED,
			SQL_EXCEPTION_LOCKCOULDNOTBEOBTAINED,
			SQL_EXCEPTION_DEADLOCKED,
			SQL_EXCEPTION_ATTEMPTEDTORELEASELOCKSNOTINTRANSACTION,
			SQL_EXCEPTION_LOCKREQUESTTIMEOUT,
			SQL_EXCEPTION_DATABASEMIRRORBUSY,
			SQL_EXCEPTION_REMOTECOMMUNICATIONSFAILEDBEFOREMIRRORINGSTARTED,
			SQL_EXCEPTION_REMOTECOMMUNICATIONSTIMEOUT,
			SQL_EXCEPTION_PARTNERANDWITNESSUNAVAILABLE,
			SQL_EXCEPTION_CANNOTSHAREEXTENT,
			SQL_EXCEPTION_PAGEEXTENTNOTFOUND,
			SQL_EXCEPTION_CANNOTSHAREEXTENTDIRECTORYFULL,
			SQL_EXCEPTION_CANNOTEXCLUSIVLEYLOCKDATABASE,
			SQL_EXCEPTION_DBCCMEMOBJLISTFAILED,
			SQL_EXCEPTION_DBCCFAILEDINTERNALQUERYERROR,
			SQL_EXCEPTION_TRANSACTIONDOOMEDINTRIGGER,
			SQL_EXCEPTION_FLOATINGPOINTEXCEPTION,
			SQL_EXCEPTION_ERRORPROCESSINGMETADATA,
			SQL_EXCEPTION_SNAPSHOTFAILEDWHENTRANSACTIONSTARTED,
			SQL_EXCEPTION_INTERNALERRORDURINGREMOTEQUERY,
			SQL_EXCEPTION_DATABASECOULDNOTBEEXCLUSIVLEYLOCKED,
			SQL_EXCEPTION_FILEAUTOGROWINPROGRESS,
			SQL_EXCEPTION_DBCCLOCKREQUESTTIMEOUT,
			SQL_EXCEPTION_TIMEOUTOPTIMISINGQUERY,
			SQL_EXCEPTION_TIMEOUTWAITINGFORMEMORY,
			SQL_EXCEPTION_COULDNOTEXECUTETRIGGER,
			SQL_EXCEPTION_TRANSPORTLEVELERROR,
			SQL_EXCEPTION_TRANSPORTLEVELERROR2,
			SQL_EXCEPTION_INTERNALQUERYPROCESSORUNEXPECTEDERROR
		};

		#endregion

		#endregion

		#endregion

		#region Contructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="connectionString">The connection string to the database.</param>
		/// <param name="tableName">Then name of the table to log to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are empty or only contain whitespace.</exception>
		/// <remarks>
		/// <para>You can use "FullDetails" as a source column name to have the entire log event serialised into a single column. You can also use "SeverityLevel" to have the severity written as an integer rather than a string.</para>
		/// </remarks>
		public SqlServerLogWriter(string connectionString, string tableName) : this(connectionString, tableName, null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="connectionString">The connection string to the database.</param>
		/// <param name="tableName">Then name of the table to log to.</param>
		/// <param name="columnMappings">An <see cref="IDictionary{TKey, TValue}"/> implementation where each key is a property name from the <see cref="LogEvent"/> class or it's <see cref="LogEvent.Properties"/> collection, and the value is the name of the column in the destination table. If null, only columns whose names match exactly to <see cref="LogEvent"/> properties or keys in the <see cref="LogEvent.Properties"/> dictionary will be written to the table.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are empty or only contain whitespace.</exception>
		/// <remarks>
		/// <para>You can use "FullDetails" as a source column name to have the entire log event serialised into a single column. You can also use "SeverityLevel" to have the severity written as an integer rather than a string.</para>
		/// </remarks>
		public SqlServerLogWriter(string connectionString, string tableName, IDictionary<string, string> columnMappings) : this(connectionString, tableName, columnMappings, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="connectionString">The connection string to the database.</param>
		/// <param name="tableName">Then name of the table to log to.</param>
		/// <param name="columnMappings">An <see cref="IDictionary{TKey, TValue}"/> implementation where each key is a property name from the <see cref="LogEvent"/> class or it's <see cref="LogEvent.Properties"/> collection, and the value is the name of the column in the destination table. If null, only columns whose names match exactly to <see cref="LogEvent"/> properties or keys in the <see cref="LogEvent.Properties"/> dictionary will be written to the table.</param>
		/// <param name="logEventFormatter">A <see cref="ILogEventFormatter"/> immplementation to use when serialising an entire <see cref="LogEvent"/> instance into the "FullDetails" column. If none is provided <see cref="Formatters.JsonLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> or <paramref name="tableName"/> are empty or only contain whitespace.</exception>
		/// <remarks>
		/// <para>You can use "FullDetails" as a source column name to have the entire log event serialised into a single column. You can also use "SeverityLevel" to have the severity written as an integer rather than a string.</para>
		/// </remarks>
		public SqlServerLogWriter(string connectionString, string tableName, IDictionary<string, string> columnMappings, ILogEventFormatter logEventFormatter)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (String.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(connectionString)));
			if (tableName == null) throw new ArgumentNullException(nameof(tableName));
			if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(tableName)));

			_ConnectionString = connectionString;
			_TableName = tableName;
			_ColumnMappings = columnMappings;
			_LogEventFormatter = logEventFormatter;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns false.
		/// </summary>
		public override bool RequiresSynchronization
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Writes the log event to the database.
		/// </summary>
		/// <param name="logEvent"></param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			WriteBatch(new LogEvent[] { logEvent }, 1);
		}

		#endregion

		#region IBatchLogWriter Members

		/// <summary>
		/// Writes the log event batch to the database.
		/// </summary>
		/// <param name="logEvents">The events to be written.</param>
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			if (logEvents == null) return;

			ExecuteWithRetries((retryCount) =>
			{
				using (var bco = CreateBulkCopyOperation(retryCount < 2))
				{
					var sw = new System.Diagnostics.Stopwatch();
					sw.Start();
					bco.WriteToServer(new LogEventReader(logEvents.GetEnumerator(), _LogEventFormatter));
					sw.Stop();
				}
			});
		}

		/// <summary>
		/// Writes the log event batch to the database.
		/// </summary>
		/// <param name="logEvents">The events to be written.</param>
		/// <param name="length">The number of events (starting from index 0) to actually write.</param>
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null) return;

			ExecuteWithRetries((int retryCount) =>
			{
				using (var bco = CreateBulkCopyOperation(retryCount < 2 && logEvents.Length > 1))
				{
					var sw = new System.Diagnostics.Stopwatch();
					sw.Start();
					bco.WriteToServer(new LogEventReader(new ArrayEnumerator<LogEvent>(logEvents, length), _LogEventFormatter));
					sw.Stop();
				}
			});
		}

		#endregion

		#region Private Methods

		private static void ExecuteWithRetries(Action<int> work)
		{
			int retryCount = 0;
			int maxRetries = 4;
			while (retryCount < maxRetries)
			{
				try
				{
					work(retryCount);
					break;
				}
				catch (System.Data.SqlClient.SqlException sqlex)
				{
					if (!IsTransientSqlException(sqlex))
						throw;

					retryCount++;
					if (retryCount >= maxRetries)
						throw;
				}
				catch (TimeoutException)
				{
					retryCount++;
					if (retryCount >= maxRetries)
						throw;
				}
				catch (InvalidOperationException)
				{
					retryCount++;
					if (retryCount >= maxRetries)
						throw;
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					retryCount++;
					if (retryCount >= maxRetries)
						throw;
				}

				System.Threading.Thread.Sleep(250 * retryCount);
			}
		}

		private static bool IsTransientSqlException(SqlException exception)
		{
			bool retVal = false;

			//TFW - Class = Severity in Sql Server
			retVal = exception.Class != 0 && exception.Class < 13;

			if (!retVal)
				retVal = RetryableSqlErrorCodes.Contains(exception.Number);

			if (!retVal)
				retVal = IsSqlTimeoutException(exception);

			if (!retVal)
			{
				//It appears some errors do not have sql error numbers (mostly network related ones where the driver returns the error not
				//sql server itself) and the error code property is generic, so best thing we know to do at the moment is check for the 
				//error message in english (yuck! if we can find a better detection mechanism, we should use it).
				retVal = exception.Message != null &&
					(exception.Message.StartsWith("A severe error", StringComparison.OrdinalIgnoreCase)
					|| exception.Message.StartsWith("A transport-level error", StringComparison.OrdinalIgnoreCase));
			}

			return retVal;
		}

		private static bool IsSqlTimeoutException(SqlException exception)
		{
			if (exception.Errors == null) return false;

			bool retVal = false;
			//Check for .net provider time out exception
			foreach (System.Data.SqlClient.SqlError error in exception.Errors)
			{
				if (error.Number == -2 && error.Class == 11) // -2 = Timeout
				{
					retVal = true;
					break;
				}
			}
			return retVal;
		}

		private System.Data.SqlClient.SqlBulkCopy CreateBulkCopyOperation(bool useTableLock)
		{
			var retVal = new System.Data.SqlClient.SqlBulkCopy(_ConnectionString, useTableLock ? System.Data.SqlClient.SqlBulkCopyOptions.TableLock : System.Data.SqlClient.SqlBulkCopyOptions.Default);
			retVal.DestinationTableName = _TableName;

			if (_ColumnMappings != null)
			{
				foreach (var kvp in _ColumnMappings)
				{
					retVal.ColumnMappings.Add(kvp.Key, kvp.Value);
				}
			}

			return retVal;
		}

		#endregion

		#region LogEventReader Class

		private sealed class LogEventReader : IDataReader
		{

			private bool _IsClosed;
			private IEnumerator _Enumerator;
			private ILogEventFormatter _LogEventFormatter;

			private static List<string> OrderedFields = new List<string>
			{
				nameof(LogEvent.DateTime),
				nameof(LogEvent.EventName),
				nameof(LogEvent.EventSeverity),
				nameof(LogEvent.EventType),
				nameof(LogEvent.Exception),
				nameof(LogEvent.Properties),
				nameof(LogEvent.Source),
				nameof(LogEvent.SourceLineNumber),
				nameof(LogEvent.SourceMethod),
				SourceColumn_SerialisedLogEvent,
				SourceColumn_LogSeverityLevel
			};

			public LogEventReader(IEnumerator enumerator, ILogEventFormatter logEventFormatter)
			{
				if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));

				_Enumerator = enumerator;
				_LogEventFormatter = logEventFormatter ?? Formatters.JsonLogEventFormatter.DefaultInstance;
			}

			public object this[string name]
			{
				get
				{
					return GetValueByName(name);
				}
			}

			public object this[int i]
			{
				get
				{
					return GetValue(i);
				}
			}

			public int Depth
			{
				get
				{
					return 0;
				}
			}

			public int FieldCount
			{
				get
				{
					return OrderedFields.Count;
				}
			}

			public bool IsClosed
			{
				get
				{
					return _IsClosed;
				}
			}

			public int RecordsAffected
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public void Close()
			{
				_IsClosed = true;
				(_Enumerator as IDisposable)?.Dispose();
				_Enumerator = null;
			}

			public void Dispose()
			{
				Close();
			}

			public bool GetBoolean(int i)
			{
				return Convert.ToBoolean(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public byte GetByte(int i)
			{
				return Convert.ToByte(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
			{
				throw new NotSupportedException();
			}

			public char GetChar(int i)
			{
				return Convert.ToChar(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
			{
				throw new NotSupportedException();
			}

			public IDataReader GetData(int i)
			{
				throw new NotSupportedException();
			}

			public string GetDataTypeName(int i)
			{
				throw new NotSupportedException();
			}

			public DateTime GetDateTime(int i)
			{
				return Convert.ToDateTime(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public decimal GetDecimal(int i)
			{
				return Convert.ToDecimal(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public double GetDouble(int i)
			{
				return Convert.ToDouble(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public Type GetFieldType(int i)
			{
				return GetValue(i).GetType();
			}

			public float GetFloat(int i)
			{
				return Convert.ToSingle(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public Guid GetGuid(int i)
			{
				return (Guid)GetValue(i);
			}

			public short GetInt16(int i)
			{
				return Convert.ToInt16(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public int GetInt32(int i)
			{
				return Convert.ToInt32(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public long GetInt64(int i)
			{
				return Convert.ToInt64(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
			}

			public string GetName(int i)
			{
				return OrderedFields[i];
			}

			public int GetOrdinal(string name)
			{
				var retVal = OrderedFields.IndexOf(name);
				if (retVal < 0)
				{
					OrderedFields.Add(name);
					retVal = OrderedFields.IndexOf(name);
				}
				return retVal;
			}

			public DataTable GetSchemaTable()
			{
				throw new NotSupportedException();
			}

			public string GetString(int i)
			{
				return GetValue(i)?.ToString();
			}

			public object GetValue(int i)
			{
				return GetValueByName(OrderedFields[i]);
			}

			public int GetValues(object[] values)
			{
				values = new object[OrderedFields.Count];
				for (int cnt = 0; cnt < OrderedFields.Count; cnt++)
				{
					values[cnt] = GetValue(cnt);
				}
				return OrderedFields.Count;
			}

			public bool IsDBNull(int i)
			{
				return false;
			}

			public bool NextResult()
			{
				return false;
			}

			public bool Read()
			{
				if (_IsClosed) throw new ObjectDisposedException(nameof(LogEventReader));

				return _Enumerator.MoveNext();
			}

			private LogEvent CurrentEvent
			{
				get { return (LogEvent)_Enumerator.Current; }
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Really, it's not that complex regardless of what the stats say, and it is reasonably efficient.")]
			private object GetValueByName(string name)
			{
				switch (name)
				{
					case nameof(LogEvent.DateTime):
						return CurrentEvent.DateTime.DateTime;

					case nameof(LogEvent.EventName):
						return CurrentEvent.EventName ?? String.Empty;

					case nameof(LogEvent.EventSeverity):
						return CurrentEvent.EventSeverity;

					case nameof(LogEvent.EventType):
						return CurrentEvent.EventType;

					case nameof(LogEvent.Exception):
						return CurrentEvent.Exception;

					case nameof(LogEvent.Source):
						return CurrentEvent.Source ?? String.Empty;

					case nameof(LogEvent.SourceLineNumber):
						return CurrentEvent.SourceLineNumber;

					case nameof(LogEvent.SourceMethod):
						return CurrentEvent.SourceMethod;

					case SourceColumn_SerialisedLogEvent:
						return _LogEventFormatter.FormatToString(CurrentEvent);

					case SourceColumn_LogSeverityLevel:
						return Convert.ToInt32(CurrentEvent.EventSeverity, System.Globalization.CultureInfo.InvariantCulture);

					default:
						var properties = CurrentEvent.Properties;
						if (properties.ContainsKey(name))
							return properties[name] ?? String.Empty;
						else
							return null;
				}
			}
		}

		#endregion

	}
}

#endif
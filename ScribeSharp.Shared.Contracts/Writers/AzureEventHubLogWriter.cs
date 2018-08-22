#if SUPPORTS_AZURESERVICEBUS

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ScribeSharp.Infrastructure;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Globalization;
using System.Net;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Sends events to an Azure event hub for processing in the cloud.
	/// </summary>
	/// <remarks>
	/// <para>Note that while the log writer supports sending the event hub data using gzip compression and an appropriate content-encoding header, event hub does not decompress the data when received. This makes it unsuitable for use with anything that does not decompress the data itself (such as Azure Stream Analytics).</para>
	/// </remarks>
	public sealed class AzureEventHubLogWriter : LogWriterBase, IBatchLogWriter, IDisposable
	{

		#region Fields

		private ILogEventFormatter _Formatter;
		private EventHubSender _Sender;

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor. Sends log events as Json.
		/// </summary>
		/// <param name="connectionString"></param>
		public AzureEventHubLogWriter(string connectionString) : this(connectionString, null, false)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <remarks><para>Instances created using this constructor have compression enabled.</para></remarks>
		/// <param name="connectionString">A connection string granting write access to the Azure event hub events are to be forwarded to.</param>
		/// <param name="formatter">An <see cref="ILogEventFormatter"/> implementation used to format events before sending to Azure. If null then <see cref="Formatters.JsonLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> is empty or contains only whitespace.</exception>
		public AzureEventHubLogWriter(string connectionString, ILogEventFormatter formatter) : this(connectionString, formatter, false)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="connectionString">A connection string granting write access to the Azure event hub events are to be forwarded to.</param>
		/// <param name="formatter">An <see cref="ILogEventFormatter"/> implementation used to format events before sending to Azure. If null then <see cref="Formatters.JsonLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <param name="useCompression">If true the system will gzip compress data before sending it to Azure.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> is empty or contains only whitespace.</exception>
		public AzureEventHubLogWriter(string connectionString, ILogEventFormatter formatter, bool useCompression)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (String.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(connectionString)), nameof(connectionString));

			_Sender = new EventHubSender(connectionString, useCompression);
			_Formatter = formatter;
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
		/// Actually writes a single event to the Azure event hub.
		/// </summary>
		/// <param name="logEvent">The event to write.</param>
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			var task = _Sender.SendJsonMessage(LogEventToJson(logEvent));
			task.Wait();
		}

		#endregion

		#region IBatchLogWriter Members

		/// <summary>
		/// Writes a batch of events to the Azure event hub.
		/// </summary>
		/// <param name="logEvents">A enumerable of <see cref="LogEvent"/> instances to write.</param>
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			if (logEvents == null) return;

			try
			{
				try
				{
					var task = _Sender.SendJsonMessage(LogEventsToJson(logEvents));
					task.Wait();
				}
				catch (AggregateException agex)
				{
					var firstException = agex.InnerExceptions.FirstOrDefault() as HttpRequestException;
					if (firstException != null)
						throw firstException;
					else
						throw;
				}
			}
			catch (System.Net.Http.HttpRequestException hrex)
			{
				if ((HttpStatusCode)((hrex.Data["StatusCode"]) ?? HttpStatusCode.Unused) == HttpStatusCode.BadRequest)
					WriteOverSizedBatch(logEvents);
				else
					throw;
			}
		}

		/// <summary>
		/// Writes a batch of events to the Azure event hub.
		/// </summary>
		/// <param name="logEvents">An array of <see cref="LogEvent"/> instances to write.</param>
		/// <param name="length">The number of items from the array to write.</param>
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null) return;

			var batch = new List<LogEvent>(length);
			for (int cnt = 0; cnt < length; cnt++)
			{
				batch.Add(logEvents[cnt]);
			}

			WriteBatch(batch);
		}

		#endregion

		#region Private Members

		private string LogEventToJson(LogEvent logEvent)
		{
			if (_Formatter == null || _Formatter is Formatters.JsonLogEventFormatter)
				return (_Formatter ?? Formatters.JsonLogEventFormatter.DefaultInstance).FormatToString(logEvent);
			else
			{
				using (var writer = Globals.TextWriterPool.Take())
				{
					using (var jsonWriter = new JsonWriter(writer.Value, false))
					{
						jsonWriter.WriteJsonObject(_Formatter.FormatToString(logEvent));
						jsonWriter.Flush();
						return writer.Value.GetStringBuilder().ToString();
					}
				}
			}
		}

		private string LogEventsToJson(IEnumerable<LogEvent> logEvents)
		{
			using (var writer = Globals.TextWriterPool.Take())
			{
				using (var jsonWriter = new JsonWriter(writer.Value, false))
				{
					if (_Formatter == null || _Formatter is Formatters.JsonLogEventFormatter)
					{
						var formatter = (_Formatter ?? Formatters.JsonLogEventFormatter.DefaultInstance);
						jsonWriter.WriteArrayStart();
						var doneOne = false;
						foreach (var item in logEvents)
						{
							if (doneOne)
								jsonWriter.WriteDelimiter();

							formatter.FormatToTextWriter(item, writer.Value);

							doneOne = true;
						}
						jsonWriter.WriteArrayEnd();
					}
					else
						jsonWriter.WriteArray((from le in logEvents select _Formatter.FormatToString(le)));

					jsonWriter.Flush();
				}

				return writer.Value.GetStringBuilder().ToString();
			}
		}

		private void WriteOverSizedBatch(IEnumerable<LogEvent> logEvents)
		{
			System.Threading.Tasks.Parallel.ForEach<LogEvent>(logEvents,
				(item) =>
				{
					WriteEventInternal(item);
				});
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Disposes this instance and all internal resources.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Sender")]
		public void Dispose()
		{
			_Sender?.Dispose();
		}

		#endregion

		#region Private Classes

		private sealed class EventHubSender : IDisposable
		{

			#region Fields

			private ServiceBusConnectionStringBuilder _ConnectionString;
			private bool _UseCompression;
			private DateTime _AuthHeaderLastGenerated;

			private HttpClient _HttpClient;

			#endregion

			#region Constructors

			public EventHubSender(string eventHubConnectionString, bool useCompression)
			{
				if (eventHubConnectionString == null) throw new ArgumentNullException(nameof(eventHubConnectionString));
				_UseCompression = useCompression;

				var handler = new HttpClientHandler();
				if (handler.SupportsAutomaticDecompression)
					handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

				_ConnectionString = new ServiceBusConnectionStringBuilder(eventHubConnectionString);
				_HttpClient = new HttpClient(handler);
				SetAuthHeader();
			}

			#endregion

			#region Public Methods

			public async Task SendJsonMessage(string messageBody)
			{
				if (ShouldRenewAuthHeader())
					SetAuthHeader();

				using (var ms = new System.IO.MemoryStream())
				{
					var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(messageBody);

					if (_UseCompression)
					{
						using (var cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
						{
							cs.Write(bytes, 0, bytes.Length);
							cs.Flush();
						}
					}
					else
						ms.Write(bytes, 0, bytes.Length);

					int retries = 0, maxRetries = 5;
					while (retries < maxRetries)
					{
						ms.Seek(0, System.IO.SeekOrigin.Begin);

						using (var content = new StreamContent(ms))
						{
							if (_UseCompression)
								content.Headers.ContentEncoding.Add("gzip");

							var result = await _HttpClient.PostAsync(_ConnectionString.EventHubUrl + "/" + _ConnectionString.EventHubPath + "/messages", content).ConfigureAwait(false);
							if (((int)result.StatusCode >= 500 && (int)result.StatusCode < 600) || (int)result.StatusCode == 429)
							{
								retries++;
								if (retries < maxRetries)
								{
#if SUPPORTS_TASKEX
									await TaskEx.Delay(250 * (retries + 1)).ConfigureAwait(false);
#else
									await Task.Delay(250 * (retries + 1)).ConfigureAwait(false);
#endif
									continue;
								}
							}

							try
							{
								result.EnsureSuccessStatusCode();
								break;
							}
							catch (HttpRequestException hrex)
							{
								hrex.Data.Add("StatusCode", result.StatusCode);
								throw;
							}
						}
					}
				}
			}

#endregion

#region Private Methods

			private void SetAuthHeader()
			{
				_HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedAccessSignature", GenerateServiceBusAuthHeader());
			}

			private string GenerateServiceBusAuthHeader()
			{
				DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				_AuthHeaderLastGenerated = DateTime.UtcNow;
				TimeSpan diff = _AuthHeaderLastGenerated - origin;
				uint tokenExpirationTime = Convert.ToUInt32(diff.TotalSeconds) + 20 * 60;

				string url = _ConnectionString.EventHubUrl + "/" + _ConnectionString.EventHubPath + "/messages";
				string stringToSign = Uri.EscapeDataString(url) + "\n" + tokenExpirationTime;
				var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_ConnectionString.SharedAccessKey));

				string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
				string token = String.Format(CultureInfo.InvariantCulture, "sr={0}&sig={1}&se={2}&skn={3}",
						Uri.EscapeDataString(url), Uri.EscapeDataString(signature), tokenExpirationTime, _ConnectionString.SharedAccessKeyName);

				return token;
			}

			private bool ShouldRenewAuthHeader()
			{
				return DateTime.UtcNow.Subtract(_AuthHeaderLastGenerated).TotalMinutes > 15;
			}

#endregion

#region Public Methods

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_HttpClient")]
			public void Dispose()
			{
				_HttpClient?.Dispose();
			}

#endregion

		}

		private class ServiceBusConnectionStringBuilder
		{
			private static readonly char[] ConnectionStringPropertySeparator = new char[] { ';' };

			public ServiceBusConnectionStringBuilder() : this(null)
			{
			}

			public ServiceBusConnectionStringBuilder(string connectionString)
			{
				if (!String.IsNullOrEmpty(connectionString))
					ParseConnectionString(connectionString);
			}

			public string EventHubPath { get; set; }
			public string EventHubUrl { get; set; }
			public string SharedAccessKey { get; set; }
			public string SharedAccessKeyName { get; set; }

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])")]
			public override string ToString()
			{
				return $"Endpoint={ this.EventHubUrl.Replace("https://", "sb://") };SharedAccessKeyName={ this.SharedAccessKeyName };SharedAccessKey={ this.SharedAccessKey };EntityPath={ this.EventHubPath }";
			}

			private void ParseConnectionString(string connectionString)
			{
				var parts = connectionString.Split(ConnectionStringPropertySeparator);
				foreach (var part in parts)
				{
					var index = part.IndexOf('=');
					var key = part.Substring(0, index).ToUpperInvariant();
					var value = part.Substring(index + 1);
					switch (key.ToUpperInvariant())
					{
						case "ENDPOINT":
							this.EventHubUrl = ParseEndpoint(value);
							break;

						case "SHAREDACCESSKEYNAME":
							this.SharedAccessKeyName = value;
							break;

						case "SHAREDACCESSKEY":
							this.SharedAccessKey = value;
							break;

						case "ENTITYPATH":
							this.EventHubPath = value;
							break;
					}
				}
			}

			private static string ParseEndpoint(string value)
			{
				var uri = new Uri(value);
				var httpUri = "https://" + uri.Authority;
				if (!httpUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) && !uri.PathAndQuery.StartsWith("/", StringComparison.OrdinalIgnoreCase))
					httpUri += "/";
				else if (httpUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) && uri.PathAndQuery.StartsWith("/", StringComparison.OrdinalIgnoreCase))
					httpUri = httpUri.Trim('/');

				httpUri = httpUri.Trim('/');
				return httpUri;
			}
		}

#endregion

	}

}

#endif
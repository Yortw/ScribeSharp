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
	public sealed class AzureEventHubLogWriter : LogWriterBase, IBatchLogWriter
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
		public AzureEventHubLogWriter(string connectionString) : this(connectionString, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="connectionString">A connection string granting write access to the Azure event hub events are to be forwarded to.</param>
		/// <param name="formatter">An <see cref="ILogEventFormatter"/> implementation used to format events before sending to Azure. If null then <see cref="Formatters.JsonLogEventFormatter.DefaultInstance"/> is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="connectionString"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="connectionString"/> is empty or contains only whitespace.</exception>
		public AzureEventHubLogWriter(string connectionString, ILogEventFormatter formatter)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (String.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(connectionString)), nameof(connectionString));

			_Sender = new EventHubSender(connectionString);
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
				var task = _Sender.SendJsonMessage(LogEventsToJson(logEvents));
				task.Wait();
			}
			catch (AggregateException agex)
			{
				var firstException = agex.InnerExceptions.FirstOrDefault() as HttpRequestException;
				if (firstException != null && (HttpStatusCode)(firstException.Data["StatusCode"] ?? HttpStatusCode.Unused) == HttpStatusCode.BadRequest)
				{
					WriteOverSizedBatch(logEvents);
				}
				else
					throw;
			}
			catch (System.Net.Http.HttpRequestException hrex)
			{
				//TODO: Check exception here.
				WriteOverSizedBatch(logEvents);
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
						jsonWriter.Flush();
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

		#region Private Classes

		private class EventHubSender
		{

			private ServiceBusConnectionStringBuilder _ConnectionString;

			private HttpClient _HttpClient;

			public EventHubSender(string eventHubConnectionString)
			{
				if (eventHubConnectionString == null) throw new ArgumentNullException(nameof(eventHubConnectionString));

				var handler = new HttpClientHandler();
				if (handler.SupportsAutomaticDecompression)
					handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

				_ConnectionString = new ServiceBusConnectionStringBuilder(eventHubConnectionString);
				_HttpClient = new HttpClient(handler);
				_HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedAccessSignature", GenerateServiceBusAuthHeader());
			}

			private string GenerateServiceBusAuthHeader()
			{
				//TODO: Renew token?
				DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
				uint tokenExpirationTime = Convert.ToUInt32(diff.TotalSeconds) + 20 * 60;

				string url = _ConnectionString.EventHubUrl + "/" + _ConnectionString.EventHubPath + "/messages";
				string stringToSign = Uri.EscapeDataString(url) + "\n" + tokenExpirationTime;
				var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_ConnectionString.SharedAccessKey));

				string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
				string token = String.Format(CultureInfo.InvariantCulture, "sr={0}&sig={1}&se={2}&skn={3}",
						Uri.EscapeDataString(url), Uri.EscapeDataString(signature), tokenExpirationTime, _ConnectionString.SharedAccessKeyName);

				return token;
			}

			public async Task SendJsonMessage(string messageBody)
			{
				var result = await _HttpClient.PostAsync(_ConnectionString.EventHubUrl + "/" + _ConnectionString.EventHubPath + "/messages", new StringContent(messageBody, System.Text.UTF8Encoding.UTF8, "application/json")).ConfigureAwait(false);
				try
				{
					result.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException hrex)
				{
					hrex.Data.Add("StatusCode", result.StatusCode);
					throw;
				}
			}
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
					var key = part.Substring(0, index).ToLower();
					var value = part.Substring(index + 1);
					switch (key.ToLowerInvariant())
					{
						case "endpoint":
							this.EventHubUrl = ParseEndpoint(value);
							break;

						case "sharedaccesskeyname":
							this.SharedAccessKeyName = value;
							break;

						case "sharedaccesskey":
							this.SharedAccessKey = value;
							break;

						case "entitypath":
							this.EventHubPath = value;
							break;
					}
				}
			}

			private string ParseEndpoint(string value)
			{
				var uri = new Uri(value);
				var httpUri = "https://" + uri.Authority;
				if (!httpUri.EndsWith("/") && !uri.PathAndQuery.StartsWith("/"))
					httpUri += "/";
				else if (httpUri.EndsWith("/") && uri.PathAndQuery.StartsWith("/"))
					httpUri = httpUri.Trim('/');

				httpUri = httpUri.Trim('/');
				return httpUri;
			}
		}

		#endregion

	}

}

#endif
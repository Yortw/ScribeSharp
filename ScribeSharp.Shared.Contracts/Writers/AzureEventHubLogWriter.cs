#if SUPPORTS_AZURESERVICEBUS

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using System.Linq;
using ScribeSharp.Infrastructure;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// Sends events to an Azure event hub for processing in the cloud.
	/// </summary>
	public sealed class AzureEventHubLogWriter : LogWriterBase, IBatchLogWriter
	{

		#region Fields

		private string _ConnectionString;
		private ILogEventFormatter _Formatter;

		private EventHubClient _Client;

		#endregion

		#region Constructors

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

			_ConnectionString = connectionString;
			_Formatter = formatter ?? Formatters.JsonLogEventFormatter.DefaultInstance;
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
			using (var stream = LogEventToStream(logEvent))
			{
				Client.Send(new EventData(stream));
			}
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

			var batch = (
				from le
				in logEvents
				select new EventData(LogEventToStream(le))
			).ToArray();

			try
			{
				Client.SendBatch(batch);
			}
			catch (Microsoft.ServiceBus.Messaging.MessageSizeExceededException)
			{
				WriteOverSizedBatch(logEvents);
			}
			finally
			{
				DisposeBatch(batch);
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

			var batch = new EventData[length];
			try
			{
				for (int cnt = 0; cnt < length; cnt++)
				{
					batch[cnt] = new EventData(LogEventToStream(logEvents[cnt]));
				}
				Client.SendBatch(batch);
			}
			catch (Microsoft.ServiceBus.Messaging.MessageSizeExceededException)
			{
				WriteOverSizedBatch(logEvents);
			}
			finally
			{
				DisposeBatch(batch);
			}
		}

		#endregion

		#region Private Members

		private EventHubClient Client
		{
			get { return _Client ?? (_Client = EventHubClient.CreateFromConnectionString(_ConnectionString)); }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		private System.IO.Stream LogEventToStream(LogEvent logEvent)
		{
			var ms = new System.IO.MemoryStream();
			try
			{
				using (var writer = new System.IO.StreamWriter(new NonClosingWrapperStream(ms)))
				{
					_Formatter.FormatToTextWriter(logEvent, writer);
					writer.Flush();
				}
				ms.Seek(0, System.IO.SeekOrigin.Begin);

				return ms;
			}
			catch
			{
				ms?.Dispose();

				throw;
			}
		}

		private static void DisposeBatch(EventData[] batch)
		{
			for (int cnt = 0; cnt < batch.Length; cnt++)
			{
				batch[cnt].Dispose();
			}
		}

		private void WriteOverSizedBatch(IEnumerable<LogEvent> logEvents)
		{
			var batch = (from le in logEvents select new EventData(LogEventToStream(le))).ToArray();

			try
			{
				var client = _Client;

				System.Threading.Tasks.Parallel.ForEach<EventData>(batch,
					(item) =>
					{
						client.Send(item);
					});
			}
			finally
			{
				DisposeBatch(batch);
			}
		}

		#endregion

	}
}

#endif
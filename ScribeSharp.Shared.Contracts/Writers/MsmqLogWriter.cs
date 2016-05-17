#if SUPPORTS_SYSTEMMESSAGING

using System;
using System.Collections.Generic;
using System.Text;
using System.Messaging;
using ScribeSharp.Infrastructure;

namespace ScribeSharp.Writers
{
	/// <summary>
	/// A <see cref="ILogWriter"/> implementation that writes log events to MSMQ messages via objects from the <see cref="System.Messaging"/> namespace.
	/// </summary>
	/// <remarks>
	/// <para>If the system opens or creates the queue itself, then it uses a <see cref="MsmqJsonLogEventMessageFormatter"/> for the queue, so all message bodies are written in Json format.</para>
	/// <para>If the queue instance is provided then the queue formatter set on the queue is used, however the default <see cref="System.Messaging.XmlMessageFormatter"/> is incompatible with the <see cref="LogEvent"/> class and will error if it was chosen. <see cref="MsmqXmlLogEventMessageFormatter"/> has been provided for scenarios that require XML serialisation of log events.</para>
	/// </remarks>
	public sealed class MsmqLogWriter : LogWriterBase, IBatchLogWriter, IDisposable
	{

		#region Fields

		private System.Messaging.MessageQueue _Queue;
		private ILogEventFormatter _Formatter;

		private const int ThreeMegabytes = 3145728;

		private static readonly object _QueueCreationSynchroniser = new object();

		#endregion

		#region Constructors

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="queuePath">The path to the queue to send to.</param>
		/// <param name="createQueueIfNotFound">A boolean indicating if the queue should be created if it does not already exist.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="queuePath"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="queuePath"/> is empty or whitespace.</exception>
		/// <exception cref="System.Messaging.MessageQueueException">Thrown if the queue does not exist, cannot be created, MSMQ is not installed etc.</exception>
		public MsmqLogWriter(string queuePath, bool createQueueIfNotFound) : this(GetOrCreateQueue(queuePath, createQueueIfNotFound), null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="queuePath">The path to the queue to send to.</param>
		/// <param name="createQueueIfNotFound">A boolean indicating if the queue should be created if it does not already exist.</param>
		/// <param name="formatter">Either null, or a <see cref="ILogEventFormatter"/> implementation used to format log entries before placing them in the queue. If null, the the log event or collection of log events is set as the message body and the MSMQ formatter associated with the queue is used.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="queuePath"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="queuePath"/> is empty or whitespace.</exception>
		/// <exception cref="System.Messaging.MessageQueueException">Thrown if the queue does not exist, cannot be created, MSMQ is not installed etc.</exception>
		public MsmqLogWriter(string queuePath, bool createQueueIfNotFound, ILogEventFormatter formatter) : this(GetOrCreateQueue(queuePath, createQueueIfNotFound), formatter)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="queue">A pre-configured <see cref="System.Messaging.MessageQueue"/> instance that will be used to send log events to.</param>
		public MsmqLogWriter(System.Messaging.MessageQueue queue) : this(queue, null)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="queue">A pre-configured <see cref="System.Messaging.MessageQueue"/> instance that will be used to send log events to.</param>
		/// <param name="formatter">Either null, or a <see cref="ILogEventFormatter"/> implementation used to format log entries before placing them in the queue. If null, the the log event or collection of log events is set as the message body and the MSMQ formatter associated with the queue is used.</param>
		public MsmqLogWriter(System.Messaging.MessageQueue queue, ILogEventFormatter formatter)
		{
			if (queue == null) throw new ArgumentNullException(nameof(queue));

			_Queue = queue;
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
		/// Writes the event to MSMQ.
		/// </summary>
		/// <param name="logEvent"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
		protected override void WriteEventInternal(LogEvent logEvent)
		{
			if (logEvent == null) return;

			var label = (logEvent.EventName ?? $"{logEvent.EventSeverity.ToString()} {logEvent.EventType.ToString()} Log Event");
			label = label.Substring(Math.Min(124, label.Length));
			if (_Formatter == null)
				_Queue.Send(logEvent, label);
			else
				_Queue.Send(_Formatter.FormatToString(logEvent), label);
		}

		#endregion

		#region IBatchLogWriter Members

		/// <summary>
		/// Writes all log events from a <see cref="IEnumerable{T}"/> batch.
		/// </summary>
		/// <param name="logEvents">An enumerable set of <see cref="LogEvent"/> instances.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public void WriteBatch(IEnumerable<LogEvent> logEvents)
		{
			if (logEvents == null) return;

			var label = "Event Batch";
			if (_Formatter == null)
				_Queue.Send(logEvents, label);
			else
			{
				using (var pooledWriter = Globals.TextWriterPool.Take())
				{
					var writer = pooledWriter.Value;

					foreach (var logEvent in logEvents)
					{
						_Formatter.FormatToTextWriter(logEvent, writer);
						writer.Flush();
						if (writer.GetStringBuilder().Length > ThreeMegabytes)
						{
							_Queue.Send(writer.GetText(), label);
							writer.Close(); // Reset the writer to continue the batch.
						}
					}

					//Send the last batch if any.
					if (writer.GetStringBuilder().Length > 0)
						_Queue.Send(writer.GetText(), label);
				}
			}
		}

		/// <summary>
		/// Writes log events from a <see cref="IEnumerable{T}"/> batch up to the specified index.
		/// </summary>
		/// <remarks>
		/// <para>This method allows writing batches from an array that is reused, to reduce allocations. Writers implementing this method should only write events from the zero based index up to the index specified by <paramref name="length"/>.</para>
		/// </remarks>
		/// <param name="logEvents">An array of events to be written.</param>
		/// <param name="length">The last index within the array that should be written in this batch.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public void WriteBatch(LogEvent[] logEvents, int length)
		{
			if (logEvents == null) return;

			var label = "Event Batch";
			if (_Formatter == null)
				WriteArrayBatchToQueueWithNoFormatter(logEvents, length, label);
			else
			{
				using (var pooledWriter = Globals.TextWriterPool.Take())
				{
					var writer = pooledWriter.Value;

					for (int cnt = 0; cnt < length; cnt++)
					{
						var logEvent = logEvents[cnt];

						_Formatter.FormatToTextWriter(logEvent, writer);
						writer.Flush();
						if (writer.GetStringBuilder().Length > ThreeMegabytes)
						{
							_Queue.Send(writer.GetText(), label);
							writer.Close(); // Reset the writer to continue the batch.
						}
					}

					//Send the last batch if any.
					if (writer.GetStringBuilder().Length > 0)
						_Queue.Send(writer.GetText(), label);
				}
			}
		}

		#endregion

		#region Private Methods

		private static System.Messaging.MessageQueue GetOrCreateQueue(string queuePath, bool createQueueIfNotFound)
		{
			if (queuePath == null) throw new ArgumentNullException(nameof(queuePath));
			if (String.IsNullOrWhiteSpace(queuePath)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmptyOrWhitespace, nameof(queuePath)), nameof(queuePath));

			lock (_QueueCreationSynchroniser)
			{
				if (createQueueIfNotFound && !System.Messaging.MessageQueue.Exists(queuePath))
					System.Messaging.MessageQueue.Create(queuePath, false);
			}

			var retVal = new System.Messaging.MessageQueue(queuePath, false, true, QueueAccessMode.Send);
			retVal.Formatter = new MsmqJsonLogEventMessageFormatter();
			retVal.DefaultPropertiesToSend.Recoverable = true;
			return retVal;
		}

		private void WriteArrayBatchToQueueWithNoFormatter(LogEvent[] logEvents, int length, string label)
		{
			LogEvent[] eventsToWrite = null;
			if (length == logEvents.Length)
				eventsToWrite = logEvents;
			else
			{
				eventsToWrite = new LogEvent[length];
				for (int cnt = 0; cnt < length; cnt++)
				{
					eventsToWrite[cnt] = logEvents[cnt];
				}
			}

			_Queue.Send(eventsToWrite, label);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes this instance and the underlying <see cref="System.Messaging.MessageQueue"/> instance.
		/// </summary>
		public void Dispose()
		{
			_Queue?.Dispose();
		}

		#endregion
	}

	#region Formatter Classes

	#region MsmqXmlLogEventMessageFormatter

	/// <summary>
	/// Provides a <see cref="System.Messaging.IMessageFormatter"/> implementation, capable of formatting <see cref="LogEvent"/> instances to XML.
	/// </summary>
	/// <remarks>
	/// <para>This is required because the standard XML formatter cannot cope with dictionaries, which <see cref="LogEvent"/> includes.</para>
	/// <para>When using this formatter to read a message body, it will return a string containing the serialised XML, not a <see cref="LogEvent"/> instance.</para>
	/// </remarks>
	public sealed class MsmqXmlLogEventMessageFormatter : System.Messaging.IMessageFormatter
	{
		/// <summary>
		/// Returns true if the message body can be read.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CanRead(Message message)
		{
			return (message?.BodyStream?.Length ?? 0) > 0;
		}

		/// <summary>
		/// Returns a new instance of <see cref="MsmqXmlLogEventMessageFormatter"/>.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new MsmqXmlLogEventMessageFormatter();
		}

		/// <summary>
		/// Deserialises the message body and returns a <see cref="LogEvent"/> object.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		public object Read(Message message)
		{
			return new System.IO.StreamReader(message.BodyStream).ReadToEnd();
		}

		/// <summary>
		/// Serialises a <see cref="LogEvent"/> object into the message body.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="obj"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		public void Write(Message message, object obj)
		{
			if (obj == null) return;
			var logEvent = (LogEvent)obj;

			var ms = new System.IO.MemoryStream();
			using (var writer = System.Xml.XmlWriter.Create(
					ms,
					new System.Xml.XmlWriterSettings()
					{
						CloseOutput = false,
						Indent = false,
						OmitXmlDeclaration = true,
						NamespaceHandling = System.Xml.NamespaceHandling.OmitDuplicates
					}
				)
			)
			{
				Formatters.XmlLogEventFormatter.DefaultInstance.FormatToTextWriter(logEvent, writer);
				writer.Flush();
				message.BodyStream = ms;
			}
		}
	}

	#endregion

	#region MsmqJsonLogEventMessageFormatter

	/// <summary>
	/// Provides a <see cref="System.Messaging.IMessageFormatter"/> implementation, capable of formatting <see cref="LogEvent"/> instances to Json.
	/// </summary>
	/// <remarks>
	/// <para>This is required because the standard Json formatter cannot cope with dictionaries, which <see cref="LogEvent"/> includes.</para>
	/// <para>When using this formatter to read a message body, it will return a string containing the serialised Json, not a <see cref="LogEvent"/> instance.</para>
	/// </remarks>
	public sealed class MsmqJsonLogEventMessageFormatter : System.Messaging.IMessageFormatter
	{
		/// <summary>
		/// Returns true if the message body can be read.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CanRead(Message message)
		{
			return (message?.BodyStream?.Length ?? 0) > 0;
		}

		/// <summary>
		/// Returns a new instance of <see cref="MsmqJsonLogEventMessageFormatter"/>.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new MsmqJsonLogEventMessageFormatter();
		}

		/// <summary>
		/// Deserialises the message body and returns a <see cref="LogEvent"/> object.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		public object Read(Message message)
		{
			return new System.IO.StreamReader(message.BodyStream).ReadToEnd();
		}

		/// <summary>
		/// Serialises a <see cref="LogEvent"/> object into the message body.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="obj"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		public void Write(Message message, object obj)
		{
			if (obj == null) return;
			var logEvent = obj as LogEvent;
			IEnumerable<LogEvent> logEvents = null;

			if (logEvent == null)
				logEvents = obj as IEnumerable<LogEvent>;

			if (logEvents == null) throw new System.ArgumentException(nameof(obj));

			if (logEvent != null)
			{
				var ms = new System.IO.MemoryStream();
				using (var writer = new System.IO.StreamWriter(new NonClosingWrapperStream(ms)))
				{
					Formatters.JsonLogEventFormatter.DefaultInstance.FormatToTextWriter(logEvent, writer);
					writer.Flush();
					message.BodyStream = ms;
				}
			}
			else
			{
				message.BodyStream = WriteBatchToStream(logEvents);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		private static System.IO.Stream WriteBatchToStream(IEnumerable<LogEvent> logEvents)
		{
			var ms = new System.IO.MemoryStream();
			using (var textWriter = new System.IO.StreamWriter(new NonClosingWrapperStream(ms)))
			{
				using (var writer = new JsonWriter(textWriter, false))
				{
					writer.WriteArrayStart();
					foreach (var logEvent in logEvents)
					{
						writer.WriteRaw(Formatters.JsonLogEventFormatter.DefaultInstance.FormatToString(logEvent));
						writer.WriteDelimiter();
					}
					writer.WriteArrayEnd();
					writer.Flush();
				}
				return ms;
			}
		}
	}

	#endregion

	#endregion

}

#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using PoolSharp;

namespace ScribeSharp
{
	/// <summary>
	/// Represents an event to be logged.
	/// </summary>
	/// <remarks>
	/// <para>Used by the system to represent all log events. Can be derived from to create well known, consistent event records.</para>
	/// </remarks>
	public class LogEvent
	{

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogEvent()
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets or returns the human readable text or message associated with this event.
		/// </summary>
		public string EventName { get; set; }

		/// <summary>
		/// Sets or returns the date and time at which this event occurred.
		/// </summary>
		public DateTimeOffset DateTime { get; set; }

		/// <summary>
		/// Sets or returns a value from the <see cref="LogEventSeverity"/> determining how important this event is.
		/// </summary>
		public LogEventSeverity EventSeverity { get; set; } = LogEventSeverity.Information;

		/// <summary>
		/// Sets or returns a value from the <see cref="LogEventType"/> determining the generic type of this event.
		/// </summary>
		public LogEventType EventType { get; set; } = LogEventType.ApplicationEvent;

		/// <summary>
		/// Sets or returns a dictionay of key value pairs, where each key is a 'property name'. Used to add additional structured data to an event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IDictionary<string, object> Properties { get; set; }

		/// <summary>
		/// Sets or returns the 'source' that created this log entry.
		/// </summary>
		/// <remarks>
		/// <para>This is often set automatically to the name of the source file containing the code that initially created the log event, but can be overridden or changed.</para>
		/// </remarks>
		public string Source { get; set; }

		/// <summary>
		/// Sets or returns the name of the method that initially created the log event.
		/// </summary>
		public string SourceMethod { get; set; }

		/// <summary>
		/// Sets or returns the line number with the source code of the source method that initially created the log event (or called the WriteEvent method that created it).
		/// </summary>
		public int SourceLineNumber { get; set; } = -1;

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the value of the specified property from the <see cref="Properties"/> collection, cast to the type specified by <typeparamref name="T"/>. If the property does not exist or the <see cref="Properties"/> is null, then <paramref name="defaultValue"/> is returned.
		/// </summary>
		/// <typeparam name="T">The type to cast to property value to.</typeparam>
		/// <param name="propertyName">The name of the property to return.</param>
		/// <param name="defaultValue">The default value to return if the property does not exist.</param>
		/// <returns>The value of the specified property cast as <typeparamref name="T"/>.</returns>
		public T GetPropertyOrDefault<T>(string propertyName, T defaultValue)
		{
			if (this.Properties != null && this.Properties.ContainsKey(propertyName))
				return (T)this.Properties[propertyName];
			else
				return defaultValue;
		}

		/// <summary>
		/// Clears or returns all properties of this object to their defaults, so the instance can be re-used for a new event without worrying about stale data.
		/// </summary>
		public virtual void Clear()
		{
			DateTime = DateTimeOffset.MinValue;
			EventName = null;
			EventSeverity = LogEventSeverity.Information;
			EventType = LogEventType.ApplicationEvent;
			Properties?.Clear();
			Source = null;
			SourceMethod = null;
			SourceLineNumber = -1;
		}

		#region Clone Methods

		/// <summary>
		/// Creates a new <see cref="LogEvent"/> instance using values from this event.
		/// </summary>
		/// <remarks>
		/// <para>This is a shallow clone for any associated reference values.</para>
		/// <para>Derived types should override this method and return an instance of their own type.</para>
		/// </remarks>
		/// <returns>A new <see cref="LogEvent"/> with the same values as this instance.</returns>
		/// <seealso cref="ICloneable.Clone"/>
		public virtual LogEvent Clone()
		{
			var retVal = new LogEvent();
			Clone(retVal);
			return retVal;
		}

		/// <summary>
		/// Copies the properties of this instance to the specified <see cref="LogEvent"/> instance.
		/// </summary>
		/// <remarks>
		/// <para>Derived types with additional properties should override this method.</para>
		/// </remarks>
		/// <param name="destination">The <see cref="LogEvent"/> instance to copy this instances properties to.</param>
		public virtual void Clone(LogEvent destination)
		{
			if (destination == null) throw new ArgumentNullException(nameof(destination));

			destination.DateTime = this.DateTime;
			destination.EventName = this.EventName;
			destination.EventSeverity = this.EventSeverity;
			destination.EventType = this.EventType;
			destination.Source = this.Source;
			destination.SourceLineNumber = this.SourceLineNumber;
			destination.SourceMethod = this.SourceMethod;

			if (this.Properties != null)
				destination.Properties = new Dictionary<string, object>(this.Properties);
			else
				destination.Properties = null;
		}

		#endregion

		#endregion
	}
}
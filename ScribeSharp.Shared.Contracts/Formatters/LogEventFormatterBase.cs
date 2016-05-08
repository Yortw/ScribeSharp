using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScribeSharp.Formatters
{
	/// <summary>
	/// Base class for <see cref="ILogEventFormatter"/> implementations, providing common logic.
	/// </summary>
	public abstract class LogEventFormatterBase : ILogEventFormatter
	{

		private ITypeRendererMap _ExceptionRenderers;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected LogEventFormatterBase()
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="exceptionRenderers">A <see cref="ITypeRendererMap"/> instance containing the renderer(s) to use when formatting <see cref="LogEvent.Exception"/> properties.</param>
		protected LogEventFormatterBase(ITypeRendererMap exceptionRenderers)
		{
			_ExceptionRenderers = exceptionRenderers;
		}

		/// <summary>
		/// Returns the formatted version of the log event as a string.
		/// </summary>
		/// <remarks>
		/// <para>Prefer the <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> method where possible as it is usually more efficient.</para>
		/// <para>The base implementation of this method using a pooled, recycling text writer and the <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> to format the event, so only overriding <see cref="FormatToTextWriter(LogEvent, TextWriter)"/> is required and the method is still reasonable efficient.</para>
		/// </remarks>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <returns>A string containing the formatted event text.</returns>
		public virtual string FormatToString(LogEvent logEvent)
		{
			using (var pooledWriter = Globals.TextWriterPool.Take())
			{
				FormatToTextWriter(logEvent, pooledWriter.Value);
				return pooledWriter.Value.GetText();
			}
		}

		/// <summary>
		/// Overridden by derived classes to actually format the log event and write the formatted output to the <paramref name="writer"/> provided.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> instance to format.</param>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to output to.</param>
		public abstract void FormatToTextWriter(LogEvent logEvent, TextWriter writer);

		/// <summary>
		/// Returns a boolean indicating whether or not this instance was initialised with one or more exception renders (true).
		/// </summary>
		protected bool HasExceptionRenderers
		{
			get { return _ExceptionRenderers != null; }
		}

		/// <summary>
		/// Returns a string representation of an exception using the renderers provided in the constructor. If not appropriate renderer is found, the exceptions ToString method result is returned.
		/// </summary>
		/// <param name="exception">The exception to format.</param>
		/// <returns>A string containing the formatted reprsentation of <paramref name="exception"/>.</returns>
		protected virtual string RenderException(Exception exception)
		{
			if (exception == null) return null;
			if (_ExceptionRenderers == null) return exception.ToString();

			var renderer = _ExceptionRenderers.GetRenderer(exception.GetType());
			if (renderer == null)
				return exception.ToString();
			else
				return (renderer.RenderValue(exception) ?? String.Empty).ToString();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.AutoLoggers
{
	/// <summary>
	/// Base class containing common logic for class that automatically write log entries when events occur.
	/// </summary>
	public abstract class AutoLoggerBase : IDisposable
	{

		private readonly ILogger _Logger;
		private bool _IsDisposed;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="logger">A <see cref="ILogger"/> to write log events to.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
		protected AutoLoggerBase(ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			_Logger = logger;
		}

		/// <summary>
		/// Returns the <see cref="ILogger"/> instance provided via the constructor.
		/// </summary>
		protected ILogger Logger
		{
			get
			{
				return _Logger;
			}
		}

		/// <summary>
		/// Returns true if the <see cref="Dispose()"/> method has been called.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return _IsDisposed;
			}
		}

		/// <summary>
		/// Disposes this class and all internal resources.
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (!_IsDisposed)
				{
					Dispose(true);
					_IsDisposed = true;
				}
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Can be overriden by derived classes to perform custom dispose logic.
		/// </summary>
		/// <param name="isDisposing">True if the object is being explicitly disposed, false if it is being finalised.</param>
		protected virtual void Dispose(bool isDisposing)
		{
		}
	}
}
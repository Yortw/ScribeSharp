using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.AutoLoggers
{
	public abstract class AutoLoggerBase : IDisposable
	{

		private readonly ILogger _Logger;
		private bool _IsDisposed;

		protected AutoLoggerBase(ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			_Logger = logger;
		}

		protected ILogger Logger
		{
			get
			{
				return _Logger;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return _IsDisposed;
			}
		}

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

		protected virtual void Dispose(bool isDisposing)
		{
		}
	}
}
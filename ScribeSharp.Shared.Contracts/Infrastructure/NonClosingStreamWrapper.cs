using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScribeSharp.Infrastructure
{
	/// <summary>
	/// A wrapper around a <see cref="System.IO.Stream"/> that does not close or dispose the stream even when explicitly asked to do so.
	/// </summary>
	public class NonClosingWrapperStream : System.IO.Stream
	{

		#region Fields

		private System.IO.Stream _Stream;

		#endregion
		
		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		public NonClosingWrapperStream(System.IO.Stream stream)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			_Stream = stream;
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Returns <see cref="CanRead"/> from the underlying stream.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return _Stream.CanRead;
			}
		}

		/// <summary>
		/// Returns <see cref="CanSeek"/> from the underlying stream.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return _Stream.CanSeek;
			}
		}

		/// <summary>
		/// Returns <see cref="CanWrite"/> from the underlying stream.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return _Stream.CanWrite;
			}
		}

		/// <summary>
		/// Gets the <see cref="Length"/> of the underlying stream.
		/// </summary>
		public override long Length
		{
			get
			{
				return _Stream.Length;
			}
		}

		/// <summary>
		/// Gets or sets <see cref="Position"/> on the underlying stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return _Stream.Position;
			}

			set
			{
				_Stream.Position = value;
			}
		}

		/// <summary>
		/// Calls <see cref="Flush()"/> on the underlying stream.
		/// </summary>
		public override void Flush()
		{
			_Stream.Flush();
		}

		/// <summary>
		/// Calls <see cref="Read(byte[], int, int)"/> on the underlying stream.
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return _Stream.Read(buffer, offset, count);
		}

		/// <summary>
		/// Calls <see cref="Seek(long, SeekOrigin)"/> on the underlying stream.
		/// </summary>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _Stream.Seek(offset, origin);
		}

		/// <summary>
		/// Calls <see cref="SetLength(long)"/> on the underlying stream.
		/// </summary>
		public override void SetLength(long value)
		{
			_Stream.SetLength(value);
		}

		/// <summary>
		/// Calls <see cref="Write(byte[], int, int)"/> on the underlying stream.
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count)
		{
			_Stream.Write(buffer, offset, count);
		}

#if !NETFX_CORE && !CONTRACTS_ONLY
		/// <summary>
		/// Does nothing.
		/// </summary>
		public override void Close()
		{
		}
#endif

		/// <summary>
		/// Does nothing.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
		}

#endregion

	}
}
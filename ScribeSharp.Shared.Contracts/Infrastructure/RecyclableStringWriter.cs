using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// A <see cref="System.IO.StringWriter"/> implementation that can be reused, reducing the number of allocations for both the writers and underlying string builders.
	/// </summary>
	/// <remarks>
	/// <para>When the <see cref="Close()"/> or <see cref="Dispose(bool)"/> methods are called the underlying string builder is reset to a length of zero so it is ready to be used for a new string.</para>
	/// <para>If, when reset, the string builder capacity is over a specified maximum, it will be reset to a specified default size. This can help avoid keeping huge string builder buffers in memory.</para>
	/// </remarks>
	public class RecyclableStringWriter : System.IO.StringWriter
	{

		#region Fields

		private StringBuilder _StringBuilder;
		private int _MaxCapacity;

		/// <summary>
		/// Flushes the text writer and returns the written string.
		/// </summary>
		/// <returns>A string containing the contents written since the object was constructed or last reset (closed/disposed).</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public string GetText()
		{
			this.Flush();
			return (_StringBuilder ?? (_StringBuilder = this.GetStringBuilder())).ToString();
		}

		private int _DefaultCapacity;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RecyclableStringWriter"/> class, with no maximum or default capacity set.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.IO.StringWriter.#ctor")]
		public RecyclableStringWriter() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RecyclableStringWriter"/> class, with the specified format control and no maximum or default capacity set.
		/// </summary>
		/// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
		public RecyclableStringWriter(IFormatProvider formatProvider) : base(formatProvider)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RecyclableStringWriter"/> class that will write to the specified <see cref="StringBuilder"/>. The <see cref="StringBuilder.MaxCapacity"/> is used for he maximum capacity and the <see cref="StringBuilder.Capacity"/> values from the provided string builder are used when this instance is recycled.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> to write to.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="stringBuilder"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.IO.StringWriter.#ctor(System.Text.StringBuilder)")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public RecyclableStringWriter(StringBuilder stringBuilder) : base(stringBuilder)
		{
			if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));

#if CONTRACTS_ONLY
			BaitExceptionHelper.Throw();
#else
			_MaxCapacity = stringBuilder.MaxCapacity;
			_DefaultCapacity = stringBuilder.Capacity;
#endif
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RecyclableStringWriter"/> class that will write to the specified <see cref="StringBuilder"/>. The <see cref="StringBuilder.MaxCapacity"/> is used for he maximum capacity and the <see cref="StringBuilder.Capacity"/> values from the provided string builder are used when this instance is recycled.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> to write to.</param>
		/// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="stringBuilder"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public RecyclableStringWriter(StringBuilder stringBuilder, IFormatProvider formatProvider) : base(stringBuilder, formatProvider)
		{
			if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));

#if CONTRACTS_ONLY
			BaitExceptionHelper.Throw();
#else
			_MaxCapacity = stringBuilder.MaxCapacity;
			_DefaultCapacity = stringBuilder.Capacity;
#endif
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RecyclableStringWriter"/> class that will write to the specified <see cref="StringBuilder"/>. The <see cref="StringBuilder.MaxCapacity"/> is used for he maximum capacity and the <see cref="StringBuilder.Capacity"/> values from the provided string builder are used when this instance is recycled.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> to write to.</param>
		/// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
		/// <param name="maxCapacity">The maximum number of characters allows in the <see cref="StringBuilder"/> before it's capacity is reset to <paramref name="defaultCapacity"/> when recycled.</param>
		/// <param name="defaultCapacity">The number of characters to reset the <see cref="StringBuilder"/> capacity to when recycled (if it is larger than the <paramref name="maxCapacity" /> argument).</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="stringBuilder"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
		public RecyclableStringWriter(StringBuilder stringBuilder, IFormatProvider formatProvider, int maxCapacity, int defaultCapacity) : base(stringBuilder, formatProvider)
		{
			_MaxCapacity = maxCapacity;
			_DefaultCapacity = defaultCapacity;
		}

#endregion

#region Overrides

#if !NETFX_CORE && !CONTRACTS_ONLY
		/// <summary>
		/// Does NOT close the text writer or underlying <see cref="System.Text.StringBuilder"/>, but instead resets it so it can be reused.
		/// </summary>
		public override void Close()
		{
			ResetStringBuilder();
		}
#endif

		/// <summary>
		/// Really disposes this instance instead of just resetting it. Should only be called if you do not intend to use this instance again.
		/// </summary>
		public void ForceDispose()
		{
			base.Dispose(true);
		}

		/// <summary>
		/// If <paramref name="disposing"/> is true, resets the underlying string builder for use. If false, calls the base dispose method which will perform a full clean up.
		/// </summary>
		/// <param name="disposing">A boolean indicating whether this instance is being disposed explicitly (true), or from the finalizer (false).</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose")]
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				ResetStringBuilder();
			else
				base.Dispose();
		}

#endregion

#region Private Methods

		private void ResetStringBuilder()
		{
			if (_StringBuilder == null)
				_StringBuilder = this.GetStringBuilder();

			_StringBuilder.Length = 0;
			if (_StringBuilder.Capacity > _MaxCapacity && _MaxCapacity > 0 && _DefaultCapacity > 0)
				_StringBuilder.Capacity = _DefaultCapacity;
		}

#endregion

	}
}
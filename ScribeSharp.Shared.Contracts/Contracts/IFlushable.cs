using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface implemented by components that buffer data and may need to be explicitly flushed under certain conditions.
	/// </summary>
	public interface IFlushable
	{
		/// <summary>
		/// Forces the component to flush it's data immediately and waits for the flush operation to complete.
		/// </summary>
		void Flush();
	}
}
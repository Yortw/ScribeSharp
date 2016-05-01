using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Represents an 'immutable' stack. Each stack operation returns a new stack instance, with all previous and future instances remaining unchanged.
	/// </summary>
	/// <typeparam name="T">The type of value to store in the stack, accessed via the <see cref="Head"/> property.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class ImmutableStack<T> : IEnumerable<T>
	{

		#region Fields

		private readonly ImmutableStack<T> _Tail;
		private readonly T _Head;

		#endregion

		#region Constructors

		/// <summary>
		/// Returns a new <see cref="ImmutableStack{T}"/> with the specified value as the top item.
		/// </summary>
		/// <param name="value">The initial value to push onto the stack.</param>
		public ImmutableStack(T value) 
		{
			Empty.Push(value);
		}


		private ImmutableStack()
		{
		}

		private ImmutableStack(T head, ImmutableStack<T> tail)
		{
			_Head = head;
			_Tail = tail;
		}

		#endregion

		/// <summary>
		/// Returns a new stack with the specified item on top.
		/// </summary>
		/// <param name="value">The value to use as the top of the new stack.</param>
		/// <returns>A <see cref="ImmutableStack{T}"/> instance.</returns>
		public ImmutableStack<T> Push(T value)
		{
			return new ImmutableStack<T>(value, this);
		}

		/// <summary>
		/// Returns the <see cref="ImmutableStack{T}"/> instance prior to the current one, effectively popping the current item off the stack.
		/// </summary>
		/// <returns>A <see cref="ImmutableStack{T}"/> instance.</returns>
		public ImmutableStack<T> Pop()
		{
			return _Tail;
		}

		/// <summary>
		/// Returns a boolean indicating if this stack is empty or not.
		/// </summary>
		public bool IsEmpty
		{
			get { return _Tail == null; }
		}

		/// <summary>
		/// Returns an empty immutable stack. This is a singleton instance that is more efficient that creating new empty instances.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static ImmutableStack<T> Empty { get; } = new ImmutableStack<T>();

		/// <summary>
		/// Returns the item at the top of the stack.
		/// </summary>
		public T Head
		{
			get
			{
				return _Head;
			}
		}

		#region IEnumerable<T> Implementation

		/// <summary>
		/// Returns an enumerator for all items in this stack.
		/// </summary>
		/// <returns>An IEnumerator{T} that can be used to enumerate this stack.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			var next = this;
			while (!next.IsEmpty)
			{
				yield return next._Head;
				next = next._Tail;
			}
		}

		/// <summary>
		/// Returns an enumerator for all items in this stack.
		/// </summary>
		/// <returns>An IEnumerator{T} that can be used to enumerate this stack.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

	}
}
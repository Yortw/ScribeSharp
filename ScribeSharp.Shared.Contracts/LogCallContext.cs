using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Linq;

namespace ScribeSharp
{
	/// <summary>
	/// <para>Allows properties to be added to the 'logical call context', which will be added to <see cref="ILogger"/> implementations using the <see cref="ContextProviders.LogCallContextLogEventContextProvider"/>.
	/// This allows properties to be added to all log calls in the logical call stack without each call needing to explicitly pass them, and without including the properties in other parallel call stacks.
	/// </para>
	/// </summary>
	public static class LogCallContext
	{

#region Fields 

		private static readonly string SlotName = Guid.NewGuid().ToString("N");

#endregion

#region Public Methods

		/// <summary>
		/// Adds a property to the logical context and returns an <see cref="IDisposable"/> that can be used to remove the property when it no longer applies to the stack.
		/// </summary>
		/// <param name="name">The name of the property to add.</param>
		/// <param name="value">The value of the property</param>
		/// <returns>An <see cref="IDisposable"/> that can be used to remove the property (by calling <see cref="IDisposable.Dispose"/>.</returns>
		/// <remarks>
		/// <para>Note that if the property value is not serialisable, errors may occur when making calls across app domain/remoting boundaries etc. Ideally only primitive types should be used for call context properties.</para>
		/// </remarks>
		public static IDisposable PushProperty(string name, object value)
		{
			CurrentContext = CurrentContext.Push(new KeyValuePair<string, object>(name, value));
			return new LogCallContextProperty(); 
		}

		/// <summary>
		/// Adds multiple properties to the logical context and returns an <see cref="IDisposable"/> that can be used to remove them all when they no longer applies to the stack.
		/// </summary>
		/// <param name="properties">An array of <see cref="KeyValuePair{TKey, TValue}"/> instances describing the properties to add.</param>
		/// <returns>An <see cref="IDisposable"/> that can be used to remove the property (by calling <see cref="IDisposable.Dispose"/>.</returns>
		/// <remarks>
		/// <para>Note that if the property value is not serialisable, errors may occur when making calls across app domain/remoting boundaries etc. Ideally only primitive types should be used for call context properties.</para>
		/// </remarks>
		public static IDisposable PushProperties(params KeyValuePair<string, object>[] properties)
		{
			if (properties == null) throw new ArgumentNullException(nameof(properties));
			if (!Utils.Any(properties)) throw new ArgumentException(String.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.PropertyCannotBeEmpty, nameof(properties)), nameof(properties));
			LogCallContextProperty retVal = null;
			foreach (var property in properties)
			{
				CurrentContext = CurrentContext.Push(new KeyValuePair<string, object>(property.Key, property.Value));

				if (retVal == null)
					retVal = new LogCallContextProperty();
				else
					retVal = new LogCallContextProperty(retVal);
			}

			return retVal;
		}

		/// <summary>
		/// Clears the current context and returns an <see cref="IDisposable"/> that will resume the context when disposed.
		/// </summary>
		/// <returns>An <see cref="IDisposable"/> that will resume the context when disposed.</returns>
		/// <remarks>
		/// <para>Useful when the context contains non-serialisable values and you are about to make a (cross app domain/remoting etc) call that would attempt serialisation.</para>
		/// </remarks>
		public static IDisposable Suspend()
		{
			var retValue = new ContextBookmark(CurrentContext);

			CurrentContext = null;

			return retValue;
		}

#endregion

#region Public Properties

		/// <summary>
		/// Returns an enumerator instance that can be used to enumerate all properties in the current logical call context.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static IEnumerable<KeyValuePair<string, object>> CurrentProperties
		{
			get
			{
				var enumerator = CurrentContext.GetEnumerator();
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
		}

#endregion

#region Private Members

		private static void Pop()
		{
			var value = CurrentContext.Pop();
			if (value == null || value.IsEmpty)
				CurrentContext = null;
			else
				CurrentContext = value;
		}

		private static ImmutableStack<KeyValuePair<string, object>> CurrentContext
		{
			get
			{
				var ret = CallContext.LogicalGetData(SlotName) as Wrapper;
				return ret == null ? ImmutableStack<KeyValuePair<string, object>>.Empty : ret.Value;
			}

			set
			{
				if (value == null)
					CallContext.LogicalSetData(SlotName, null);
				else
					CallContext.LogicalSetData(SlotName, new Wrapper { Value = value });
			}
		}

#endregion

#region Private Classes

		private sealed class Wrapper : MarshalByRefObject
		{
			public ImmutableStack<KeyValuePair<string, object>> Value { get; set; }
		}

		private sealed class LogCallContextProperty : IDisposable
		{
			private LogCallContextProperty _ChildProperty;
			private bool _IsDisposed;

			public LogCallContextProperty()
			{
			}

			public LogCallContextProperty(LogCallContextProperty childProperty)
			{
				_ChildProperty = childProperty;
			}

			public void Dispose()
			{
				if (_IsDisposed)
					return;

				Pop();
				_IsDisposed = true;

				_ChildProperty?.Dispose();
			}
		}

		private sealed class ContextBookmark : IDisposable
		{
			private ImmutableStack<KeyValuePair<string, object>>  _ContextValue;

			public ContextBookmark(ImmutableStack<KeyValuePair<string, object>> contextValue)
			{
				_ContextValue = contextValue;
			}

			public void Dispose()
			{
				CurrentContext = _ContextValue;
			}
		}

#endregion

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// A base class for context providers containing common logic.
	/// </summary>
	public abstract class ContextProviderBase : ILogEventContextProvider
	{

		private readonly ILogEventFilter _Filter;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ContextProviderBase()
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		protected ContextProviderBase(ILogEventFilter filter)
		{
			_Filter = filter;
		}

		/// <summary>
		/// Returns the <see cref="ILogEventFilter"/> instance to use when deciding whether to apply context or not.
		/// </summary>
		public ILogEventFilter Filter
		{
			get
			{
				return _Filter;
			}
		}

		/// <summary>
		/// The name of the property to apply.
		/// </summary>
		public abstract string PropertyName
		{
			get;
		}

		/// <summary>
		/// The value of the property to apply.
		/// </summary>
		/// <returns>A loosley typed value.</returns>
		public abstract object GetValue();
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can locate a <see cref="IPropertyRenderer"/> instance to use for a given <see cref="System.Type"/>.
	/// </summary>
	public interface ITypeRendererMap
	{
		/// <summary>
		/// Gets a <see cref="IPropertyRenderer"/> for the provided <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type of value to get a renderer for.</param>
		/// <returns>A <see cref="IPropertyRenderer"/> reference, or null if no renderer could be found.</returns>
		IPropertyRenderer GetRenderer(Type type);
		/// <summary>
		/// Gets a <see cref="IPropertyRenderer"/> for the specified type.
		/// </summary>
		/// <typeparam name="T">The type of valu to retrieve a renderer for.</typeparam>
		/// <returns>A <see cref="IPropertyRenderer"/> reference, or null if no renderer could be found.</returns>
		IPropertyRenderer GetRenderer<T>();
	}
}
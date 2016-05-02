using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can locate a <see cref="IPropertyRenderer"/> instance to use for a given <see cref="System.Type"/>, and can be modified with new renderers after being constructed.
	/// </summary>
	public interface IMutableTypeRendererMap : ITypeRendererMap
	{
		/// <summary>
		/// Adds a renderer for the given type to the map.
		/// </summary>
		/// <param name="type">The type that can be rendered by this renderer.</param>
		/// <param name="renderer">The renderer to add.</param>
		/// <returns>A boolean indicating if the item was added (true), or false if it was not added because a renderer is already loaded for this type.</returns>
		bool AddRenderer(Type type, IPropertyRenderer renderer);
		/// <summary>
		/// Removes the renderer for the given type from the map.
		/// </summary>
		/// <param name="type">The type the renderer was added for.</param>
		/// <returns>A boolean indicating if the item was removed (true), or false if it was not removed because a renderer was not found for that type.</returns>
		bool RemoveRenderer(Type type);
	}
}
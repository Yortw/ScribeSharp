using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Provides an efficient, cached map of types to renders.
	/// </summary>
	public class TypeRendererMap : IMutableTypeRendererMap
	{

		private System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer> _KnownRenderers;
		private System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer> _CachedRenderers;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TypeRendererMap() : this(null)
		{
			_KnownRenderers = new System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer>();
			_CachedRenderers = new System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer>();
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		public TypeRendererMap(params KeyValuePair<Type, IPropertyRenderer>[] renderers) 
		{
			_KnownRenderers = new System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer>();
			_CachedRenderers = new System.Collections.Concurrent.ConcurrentDictionary<Type, IPropertyRenderer>();

			if (renderers != null)
			{
				foreach (var kvp in renderers)
				{
					_KnownRenderers.TryAdd(kvp.Key, kvp.Value);
					_CachedRenderers.TryAdd(kvp.Key, kvp.Value);
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="IPropertyRenderer"/> for the provided type.
		/// </summary>
		/// <typeparam name="T">The type of value the renderer operates on.</typeparam>
		/// <returns>A <see cref="IPropertyRenderer"/> reference, or null if no renderer could be found.</returns>
		public IPropertyRenderer GetRenderer<T>()
		{
			if (_KnownRenderers.Count == 0) return null;

			return GetRenderer(typeof(T));
		}

		/// <summary>
		/// Gets a <see cref="IPropertyRenderer"/> for the specified type.
		/// </summary>
		/// <param name="type">The type that can be rendered by this renderer.</param>
		/// <returns>A <see cref="IPropertyRenderer"/> reference, or null if no renderer could be found.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
		public IPropertyRenderer GetRenderer(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (_KnownRenderers.Count == 0) return null;

			IPropertyRenderer retVal = null;

			retVal = GetFromCache(type);
			if (retVal == null)
			{
				retVal = LookupByType(type);
				_CachedRenderers.TryAdd(type, retVal);
			}

			return retVal;
		}

		/// <summary>
		/// Adds a renderer for the given type to the map.
		/// </summary>
		/// <param name="type">The type that can be rendered by this renderer.</param>
		/// <param name="renderer">The renderer to add.</param>
		/// <returns>A boolean indicating if the item was added (true), or false if it was not added because a renderer is already loaded for this type.</returns>
		public bool AddRenderer(Type type, IPropertyRenderer renderer)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (renderer == null) throw new ArgumentNullException(nameof(renderer));

			var retVal = _KnownRenderers.TryAdd(type, renderer);
			_CachedRenderers.TryAdd(type, renderer);
			return retVal;			
		}
		/// <summary>
		/// Removes the renderer for the given type from the map.
		/// </summary>
		/// <param name="type">The type the renderer was added for.</param>
		/// <returns>A boolean indicating if the item was removed (true), or false if it was not removed because a renderer was not found for that type.</returns>
		public bool RemoveRenderer(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			IPropertyRenderer removedValue = null;
			var retVal = _KnownRenderers.TryRemove(type, out removedValue);
			_CachedRenderers.TryRemove(type, out removedValue);
			return retVal;
		}

		private IPropertyRenderer GetFromCache(Type type)
		{
			IPropertyRenderer retVal = null;
			_CachedRenderers.TryGetValue(type, out retVal);
			return retVal;
		}

		private IPropertyRenderer LookupByType(Type type)
		{
			var originalType = type;
			IPropertyRenderer retVal = null;
			while (type != null && !_KnownRenderers.TryGetValue(type, out retVal))
			{
				type = type.BaseType;
			}

			if (retVal == null)
			{
				var interfaces = originalType.GetInterfaces();
				for (int cnt = 0; cnt < interfaces.Length; cnt++)
				{
					if (_KnownRenderers.TryGetValue(interfaces[cnt], out retVal)) break;
				}
			}

			return retVal;
		}
	}
}
using System.Collections.Generic;

namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can automatically apply additional properties (context) to <see cref="LogEvent"/> instances.
	/// </summary>
	public interface ILogEventContextProvider
	{
		/// <summary>
		/// A <see cref="ILogEventFilter"/> used to determine if the property should be added.
		/// </summary>
		/// <remarks>
		/// <para>If you need multiple filters use an instance of <see cref="Filters.AndFilter"/> or <see cref="Filters.OrFilter"/>.</para>
		/// </remarks>
		ILogEventFilter Filter { get; }

		/// <summary>
		/// Called by the system to ask this context provider to add any properties it wants to apply.
		/// </summary>
		/// <param name="logEvent">The <see cref="LogEvent"/> to add properties to.</param>
		/// <param name="typeRendererMap">A typeRendererMap instance that can be used to to locate renderers for property values.</param>
		void AddProperties(LogEvent logEvent, ITypeRendererMap typeRendererMap);
	}
}
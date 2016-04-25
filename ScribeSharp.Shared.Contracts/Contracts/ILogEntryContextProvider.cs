namespace ScribeSharp
{
	/// <summary>
	/// Interface for components that can automatically apply additional properties (context) to <see cref="LogEvent"/> instances.
	/// </summary>
	public interface ILogEventContextProvider
	{
		/// <summary>
		/// Returns the name of the property to add.
		/// </summary>
		string PropertyName { get; }
		/// <summary>
		/// Returns the value of the property to add.
		/// </summary>
		/// <returns>The value of the property to add.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		object GetValue();

		/// <summary>
		/// A <see cref="ILogEventFilter"/> used to determine if the property should be added.
		/// </summary>
		/// <remarks>
		/// <para>If you need multiple filters use an instance of <see cref="Filters.AndFilter"/> or <see cref="Filters.OrFilter"/>.</para>
		/// </remarks>
		ILogEventFilter Filter { get; }

	}
}
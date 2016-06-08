using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp.ContextProviders
{
	/// <summary>
	/// Adds a property containing the version of an assembly. The simple name of the assembly is used for the property name if a more specific name is not explicitly provided.
	/// </summary>
	public sealed class AssemblyVersionLogEntryContextProvider : ContextProviderBase
	{

		#region Fields

		private string _PropertyName;
		private string _Version;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="assembly">A reference to a <see cref="System.Reflection.Assembly"/> whose version should be written as a property.</param>
		public AssemblyVersionLogEntryContextProvider(System.Reflection.Assembly assembly) : this(assembly, null) { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="assembly">A reference to a <see cref="System.Reflection.Assembly"/> whose version should be written as a property.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public AssemblyVersionLogEntryContextProvider(System.Reflection.Assembly assembly, ILogEventFilter filter) : this(assembly, null, filter) { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="assembly">A reference to a <see cref="System.Reflection.Assembly"/> whose version should be written as a property.</param>
		/// <param name="propertyName">A specific string to use for the property name. If null or empty string, the assembly 'simple' name will be used.</param>
		/// <param name="filter">A <see cref="ILogEventFilter"/> instance used to determine if the property should be added or not.</param>
		public AssemblyVersionLogEntryContextProvider(System.Reflection.Assembly assembly, string propertyName, ILogEventFilter filter) : base(filter)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));

#if !CONTRACTS_ONLY

			var name = assembly.GetName();
			_PropertyName = propertyName;

			if (String.IsNullOrEmpty(_PropertyName))
				_PropertyName = name.Name;

			_Version = name.Version.ToString();

#endif
		}

#endregion

#region Overrides

		/// <summary>
		/// Adds a property containing the version of the assembly provided via the constructor. Uses either the property name specified in the constructor, or the assembly's 'simple' name if the provided name was null or empty.
		/// </summary>
		/// <param name="logEvent">The log event to apply the property to.</param>
		/// <param name="rendererMap">A <see cref="ITypeRendererMap"/> that can be used to locate <see cref="IPropertyRenderer"/> instances to use when formatting properties. May be null if no renderers have been provided.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AddPropertiesCore(LogEvent logEvent, ITypeRendererMap rendererMap)
		{
			AddProperty(logEvent.Properties, _PropertyName, _Version, rendererMap);
		}

#endregion

	}
}
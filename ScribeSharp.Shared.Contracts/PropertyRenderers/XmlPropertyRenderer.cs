using System;
using System.Collections.Generic;
using System.Text;

namespace ScribeSharp.PropertyRenderers
{
	/// <summary>
	/// Renders a type as an XML string.
	/// </summary>
  public sealed class XmlPropertyRenderer : IPropertyRenderer
	{
#if !CONTRACTS_ONLY

		private System.Xml.Serialization.XmlSerializer _Serialiser;
		private System.Xml.Serialization.XmlSerializerNamespaces _EmptyNamespaces;

#endif

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="sourceType">The type to be rendered.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="sourceType"/> is null.</exception>
		public XmlPropertyRenderer(Type sourceType)
		{
#if CONTRACTS_ONLY
			BaitExceptionHelper.Throw();
#else
			if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));

			_Serialiser = new System.Xml.Serialization.XmlSerializer(sourceType, (string)null);
			_EmptyNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
			_EmptyNamespaces.Add(String.Empty, String.Empty);
#endif
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="serializer">A pre-configured <see cref="System.Xml.Serialization.XmlSerializer"/> used to serialised property values.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="serializer"/> is null.</exception>
		public XmlPropertyRenderer(System.Xml.Serialization.XmlSerializer serializer)
		{
#if CONTRACTS_ONLY
			BaitExceptionHelper.Throw();
#else
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			_Serialiser = serializer;
			_EmptyNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
			_EmptyNamespaces.Add(String.Empty, String.Empty);
#endif
		}

		/// <summary>
		/// Converts the provided object to an XML string.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A string containing an XML representation of <paramref name="value"/>.</returns>
		public object RenderValue(object value)
		{
#if CONTRACTS_ONLY
			BaitExceptionHelper.Throw();
			return null;
#else
			if (value == null) return null;

			using (var pooledWriter = Globals.TextWriterPool.Take())
			{
				_Serialiser.Serialize(pooledWriter.Value, value, _EmptyNamespaces);

				pooledWriter.Value.Flush();
				return pooledWriter.Value.GetStringBuilder().ToString();
			}
#endif
		}
	}
}
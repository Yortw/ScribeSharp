using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ScribeSharp.Infrastructure
{
	/// <summary>
	/// A simple, light weight json writer used to manually output Json data (unformatted/minified).
	/// </summary>
	/// <remarks>
	/// <para>This class does not guarantee correct Json. It will create correct Json when used properly, but using the fine grained methods and failing to manually write your own delimiters or closing tags the result can be invalid. This is by design, to allow generation of fragments, and for performance reasons.</para>
	/// <para>This class does not prettify the resulting json, the output is a relatively minified version good for storage/transport.</para>
	/// </remarks>
	public sealed class JsonWriter : IDisposable
	{

		#region Fields

		private TextWriter _Writer;
		private bool _DisposeOutput;

		private bool _NeedsDelimiter;

		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IList<System.Reflection.PropertyInfo>> TypeProperties = new System.Collections.Concurrent.ConcurrentDictionary<Type, IList<System.Reflection.PropertyInfo>>();

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="writer">The underlying <see cref="System.IO.TextWriter"/> to output to.</param>
		/// <param name="disposeOutput">True to have the <see cref="Dispose"/> dispose the <paramref name="writer"/> when called, otherwise false.</param>
		public JsonWriter(TextWriter writer, bool disposeOutput)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));
			_Writer = writer;
			_DisposeOutput = disposeOutput;
		}

		#endregion

		#region Public Methods

		#region Write Value Methods

		/// <summary>
		/// Writes a <see cref="bool"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteBoolean(bool value)
		{
			_Writer.Write(value ? "true" : "false");
		}

		/// <summary>
		/// Writes a <see cref="Single"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteSingle(float value)
		{
			_Writer.Write(value.ToString("R", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes a <see cref="double"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteDouble(double value)
		{
			_Writer.Write(value.ToString("R", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes a <see cref="DateTimeOffset"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTimeOffset.ToString(System.String)")]
		public void WriteDateTimeOffset(DateTimeOffset value)
		{
			_Writer.Write("\"");
			_Writer.Write(value.ToString("o"));
			_Writer.Write("\"");
		}

		/// <summary>
		/// Writes a <see cref="DateTime"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString(System.String)")]
		public void WriteDateTime(DateTime value)
		{
			_Writer.Write("\"");
			_Writer.Write(value.ToString("o"));
			_Writer.Write("\"");
		}

		/// <summary>
		/// Writes a <see cref="string"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteString(string value)
		{
			var content = Encode(value);
			_Writer.Write("\"");
			_Writer.Write(content);
			_Writer.Write("\"");
		}

		/// <summary>
		/// Writes a <see cref="short"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteShort(short value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="int"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteInt(int value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="long"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteLong(long value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="byte"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteByte(byte value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="decimal"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteDecimal(decimal value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="float"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteFloat(float value)
		{
			WriteFormattable(value);
		}

#if SUPPORTS_NONCLSCOMPLIANTPRIMITIVES

		/// <summary>
		/// Writes a <see cref="ushort"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ushort")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteUshort(ushort value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="uint"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteUInt(uint value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="ulong"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ulong")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteUlong(ulong value)
		{
			WriteFormattable(value);
		}

		/// <summary>
		/// Writes a <see cref="sbyte"/> value.
		/// </summary>
		/// <param name="value">The value to write.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "ScribeSharp.Infrastructure.JsonWriter.WriteFormattable(System.IFormattable)")]
		public void WriteSByte(sbyte value)
		{
			WriteFormattable(value);
		}
		
#endif

		/// <summary>
		/// Writes an array of values as a property with the specified name.
		/// </summary>
		/// <param name="values">The array to write.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
		public void WriteArray(IEnumerable values)
		{
			WriteArray(null, values);
		}

		/// <summary>
		/// Writes an array of values as a property with the specified name.
		/// </summary>
		/// <param name="name">The name of the property to use for the array.</param>
		/// <param name="values">The array to write.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
		public void WriteArray(string name, IEnumerable values)
		{
			if (!String.IsNullOrEmpty(name))
				WritePropertyName(name);

			if (values == null)
			{
				WriteNull();
				return;
			}

			WriteArrayStart();
			foreach (var value in values)
			{
				WriteValue(value);
				WriteDelimiter();
			}
			WriteArrayEnd();
		}

		/// <summary>
		/// Writes an <see cref="char"/> value (as a complete json string).
		/// </summary>
		/// <param name="value">The char to write.</param>
		public void WriteChar(char value)
		{
			WriteString(value.ToString());
		}

		#endregion

		#region WriteFormattable Overrides

		/// <summary>
		/// Writes out an <see cref="IFormattable"/> as  string using the <see cref="System.Globalization.CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <param name="formattable">The value to write as a string.</param>
		public void WriteFormattable(IFormattable formattable)
		{
			WriteFormattable(formattable, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Writes out an <see cref="IFormattable"/> as  string using the specified <see cref="IFormatProvider"/>.
		/// </summary>
		/// <param name="formattable">The value to write as a string.</param>
		/// <param name="formatProvider">An <see cref="IFormatProvider"/> implementation used to format the string.</param>
		public void WriteFormattable(IFormattable formattable, IFormatProvider formatProvider)
		{
			WriteFormattable(formattable, formatProvider, false);
		}

		/// <summary>
		/// Writes out an <see cref="IFormattable"/> as  string using the specified <see cref="IFormatProvider"/>.
		/// </summary>
		/// <param name="formattable">The value to write as a string.</param>
		/// <param name="formatProvider">An <see cref="IFormatProvider"/> implementation used to format the string.</param>
		/// <param name="quoteValue">True if the value should be treated as a json string value, otherwise false to have it written as a raw value (most numerics).</param>
		public void WriteFormattable(IFormattable formattable, IFormatProvider formatProvider, bool quoteValue)
		{
			if (formattable == null)
				WriteNull();
			else
			{
				if (quoteValue)
					WriteString(formattable.ToString(null, formatProvider));
				else
					_Writer.Write(formattable.ToString(null, formatProvider));
			}
		}

		#endregion

		/// <summary>
		/// Appends a strong containing raw formatting into the output, does not perform any escaping, quoting or other modifications to the string and does not validate the string is correct.
		/// </summary>
		/// <param name="json">A string containing the json to write.</param>
		///	<exception cref="System.ArgumentNullException">Thrown if <paramref name="json"/> is null.</exception>
		public void WriteRaw(string json)
		{
			if (json == null) throw new ArgumentNullException(nameof(json));

			_Writer.Write(json);
		}

		/// <summary>
		/// Writes the beginning of a json array.
		/// </summary>
		public void WriteArrayStart()
		{
			_Writer.Write("[");
		}

		/// <summary>
		/// Writes the end of a json array.
		/// </summary>
		public void WriteArrayEnd()
		{
			_Writer.Write("]");
			_NeedsDelimiter = true;
		}

		/// <summary>
		/// Writes the beginning of a json object.
		/// </summary>
		public void WriteObjectStart()
		{
			_Writer.Write("{");
		}

		/// <summary>
		/// Writes the end of a json object.
		/// </summary>
		public void WriteObjectEnd()
		{
			_Writer.Write("}");
			_NeedsDelimiter = true;
		}

		/// <summary>
		/// Attempts to write an <see cref="object"/> as it's most appropriate value to the output.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteValue(object value)
		{
			if (value == null)
			{
				WriteNull();
				return;
			}

			if (value is string)
				WriteString((string)value);
			else if (value is DateTime)
				WriteDateTime((DateTime)value);
			else if (value is DateTimeOffset)
				WriteDateTimeOffset((DateTimeOffset)value);
			else if (value is bool)
				WriteBoolean((bool)value);
			else if (value is double)
				WriteDouble((double)value);
			else if (value is Single)
				WriteSingle((Single)value);
			else if (value is byte)
				WriteByte((byte)value);
			else if (value is float)
				WriteFloat((float)value);
			else if (value is decimal)
				WriteDecimal((decimal)value);
			else if (value is char)
				WriteChar((char)value);
			else if (value is int)
				WriteInt((int)value);
			else if (value is long)
				WriteLong((long)value);
			else if (value is short)
				WriteShort((short)value);
#if SUPPORTS_NONCLSCOMPLIANTPRIMITIVES
			else if (value is uint)
				WriteUInt((uint)value);
			else if (value is ulong)
				WriteUlong((ulong)value);
			else if (value is ushort)
				WriteUshort((ushort)value);
			else if (value is sbyte)
				WriteSByte((sbyte)value);
#endif
			else if (value is char)
				WriteChar((char)value);
			else if (value is IFormattable)
				WriteFormattable((IFormattable)value, System.Globalization.CultureInfo.InvariantCulture, true);
			else if (value is IDictionary) // Must come before IEnumerable
				WriteDictionary((IDictionary)value);
			else if (value is IEnumerable)
				WriteArray((IEnumerable)value);
			else
				WriteJsonObject(value);
		}

		/// <summary>
		/// Writes the delimiter used to separates, arrays, objects and properties.
		/// </summary>
		public void WriteDelimiter()
		{
			_Writer.Write(",");
			_NeedsDelimiter = false;
		}

		/// <summary>
		/// Writes a json property name with no value.
		/// </summary>
		/// <param name="name">The name of the property to write.</param>
		public void WritePropertyName(string name)
		{
			if (_NeedsDelimiter) WriteDelimiter();

			name = Encode(name);
			_Writer.Write("\"");
			_Writer.Write(name);
			_Writer.Write("\":");
		}

		/// <summary>
		/// Writes a json property with the specified name and value.
		/// </summary>
		public void WriteJsonProperty(string name, object value)
		{
			if (_NeedsDelimiter) WriteDelimiter();

			_Writer.Write("\"");
			_Writer.Write(name);
			_Writer.Write("\":");
			WriteValue(value);

			_NeedsDelimiter = true;
		}

		/// <summary>
		/// Writes the json null token.
		/// </summary>
		public void WriteNull()
		{
			_Writer.Write("null");
		}

		/// <summary>
		/// Writes a json object using the properties of the <paramref name="value"/> argument.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public void WriteJsonObject(object value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			WriteJsonObject(null, value, value.GetType());
		}

		/// <summary>
		/// Writes a json object using the properties of the <paramref name="value"/> argument.
		/// </summary>
		/// <param name="value">The object to write.</param>
		public void WriteJsonObject<T>(T value)
		{
			WriteJsonObject(null, value, typeof(T));
		}

		/// <summary>
		/// Writes a json object with the specified name using the properties of the <paramref name="value"/> argument.
		/// </summary>
		/// <param name="name">The name of the object inside the json.</param>
		/// <param name="value">The object to write.</param>
		public void WriteJsonObject(string name, object value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			WriteJsonObject(name, value.GetType());
		}

		/// <summary>
		/// Writes a json object with the specified name using the properties of the <paramref name="value"/> argument.
		/// </summary>
		/// <param name="name">The name of the object inside the json.</param>
		/// <param name="value">The object to write.</param>
		public void WriteJsonObject<T>(string name, T value)
		{
			WriteJsonObject(name, value, typeof(T));
		}

		/// <summary>
		/// Writes a dictionary to the json output as a json array of json properties. The ToString result of the dictionary key is used as the property name for each array entry.
		/// </summary>
		/// <typeparam name="K">The type of value used for the key. The <see cref="object.ToString"/> method of this type will be used for the property name of each entry in the output.</typeparam>
		/// <typeparam name="V">The type of value stored in the dictionary.</typeparam>
		/// <param name="values">The dictionary containing the values to output.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "K")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
		public void WriteDictionary<K, V>(IDictionary<K, V> values)
		{
			if (values == null)
			{
				WriteNull();
				return;
			}

			WriteObjectStart();

			foreach (var kvp in values)
			{
				WriteJsonProperty(kvp.Key.ToString(), kvp.Value);
			}

			WriteObjectEnd();
		}

		/// <summary>
		/// Writes a dictionary to the json output as a json array of json properties. The ToString result of the dictionary key is used as the property name for each array entry.
		/// </summary>
		/// <typeparam name="K">The type of value used for the key. The <see cref="object.ToString"/> method of this type will be used for the property name of each entry in the output.</typeparam>
		/// <typeparam name="V">The type of value stored in the dictionary.</typeparam>
		/// <param name="name">The name of the property to use for the whole dictionary.</param>
		/// <param name="values">The dictionary containing the values to output.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "K")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
		public void WriteDictionary<K, V>(string name, IDictionary<K, V> values)
		{
			if (!String.IsNullOrEmpty(name))
				WritePropertyName(name);

			WriteDictionary(values);
		}

		/// <summary>
		/// Writes a dictionary to the json output as a json array of json properties. The ToString result of the dictionary key is used as the property name for each array entry.
		/// </summary>
		/// <param name="values">The dictionary containing the values to output.</param>
		public void WriteDictionary(IDictionary values)
		{
			if (values == null)
			{
				WriteNull();
				return;
			}

			WriteObjectStart();

			foreach (DictionaryEntry kvp in values)
			{
				WriteJsonProperty(kvp.Key.ToString(), kvp.Value);
			}

			WriteObjectEnd();
		}

		/// <summary>
		/// Writes a dictionary to the json output as a json array of json properties. The ToString result of the dictionary key is used as the property name for each array entry.
		/// </summary>
		/// <param name="name">The name of the property to use for the whole dictionary.</param>
		/// <param name="values">The dictionary containing the values to output.</param>
		public void WriteDictionary(string name, IDictionary values)
		{
			if (!String.IsNullOrEmpty(name))
				WritePropertyName(name);

			WriteDictionary(values);
		}

		/// <summary>
		/// Flushes this and the underlying writer to ensure output is written to the final destination.
		/// </summary>
		public void Flush()
		{
			_Writer.Flush();
		}

		#endregion

		#region Private Methods

		private void WriteJsonObject<T>(string name, T value, Type type)
		{
			var properties = GetTypeProperties(type);
			if (!Utils.Any(properties)) return;

			if (!String.IsNullOrEmpty(name))
				WritePropertyName(name);

			if (value == null)
			{
				WriteNull();
				return;
			}

			if (value is IDictionary)
			{
				WriteDictionary((IDictionary)value);
				return;
			}

			WriteObjectStart();

			for (int cnt = 0; cnt < properties.Count; cnt++)
			{
				var property = properties[cnt];
				WriteJsonProperty(property.Name, property.GetValue(value, null));
			}

			WriteObjectEnd();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes this instance and all internal properties.
		/// </summary>
		public void Dispose()
		{
			if (_DisposeOutput)
				_Writer?.Dispose();
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Perform simple encoding/escaping of a string (<paramref name="value"/>) to make it safe for embedding in Json.
		/// </summary>
		/// <param name="value">A string containing the data to be encoded.</param>
		/// <returns>A JSON-escaped version of <paramref name="value"/>.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString(System.String)")]
		public static string Encode(string value)
		{
			//Some "whitespace" needs encoding, so don't check that.
			if (String.IsNullOrEmpty(value)) return value;

			var retVal = value;
			StringBuilder escapedString = null;
			var startIndex = 0;
			for (var cnt = 0; cnt < value.Length; ++cnt)
			{
				var c = value[cnt];
				if (c < ' ' || c == '\\' || c == '"')
				{

					if (escapedString == null)
						escapedString = new StringBuilder();

					escapedString.Append(value.Substring(startIndex, cnt - startIndex));
					startIndex = cnt + 1;

					switch (c)
					{
						case '"':
							escapedString.Append("\\\"");
							break;
						case '\\':
							escapedString.Append("\\\\");
							break;
						case '\n':
							escapedString.Append("\\n");
							break;
						case '\r':
							escapedString.Append("\\r");
							break;
						case '\f':
							escapedString.Append("\\f");
							break;
						case '\t':
							escapedString.Append("\\t");
							break;
						default:
							escapedString.Append("\\u");
							escapedString.Append(((int)c).ToString("X4"));
							break;
					}
				}
			}

			if (escapedString != null)
			{
				if (startIndex != value.Length)
					escapedString.Append(value.Substring(startIndex));

				retVal = escapedString.ToString();
			}

			return retVal;
		}

		private static IList<System.Reflection.PropertyInfo> GetTypeProperties(Type type)
		{
			IList<System.Reflection.PropertyInfo> retVal = null;

			if (!TypeProperties.TryGetValue(type, out retVal))
			{
				retVal = new List<PropertyInfo>(
					(from pi 
					 in type.GetProperties()
					 where pi.GetIndexParameters().Length <= 0
					 select pi)
				);
				TypeProperties.TryAdd(type, retVal);
			}

			return retVal;
		}

		#endregion

	}
}
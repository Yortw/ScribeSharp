using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class JsonWriterTests
	{

		#region WriteJsonProperty Tests

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesValidJsonForProperties()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				jsonWriter.WriteObjectStart();

				jsonWriter.WriteJsonProperty("Test", "Test1");
				jsonWriter.WriteJsonProperty("IsTest", true);
				jsonWriter.WriteJsonProperty("TestCount", 10);

				jsonWriter.WriteObjectEnd();

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson1>(text);
				Assert.IsNotNull(result);
				Assert.AreEqual("Test1", result.Test);
				Assert.AreEqual(true, result.IsTest);
				Assert.AreEqual(10, result.TestCount);
				Assert.IsFalse(text.Contains("PrivateTestProperty"));
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesInt()
		{
			TestWriteJsonProperty<int>(1);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesLong()
		{
			TestWriteJsonProperty<long>(1);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesShort()
		{
			TestWriteJsonProperty<short>(1);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesBoolean()
		{
			TestWriteJsonProperty<bool>(true);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesDateTime()
		{
			TestWriteJsonProperty<DateTime>(DateTime.Now);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesDateTimeOffset()
		{
			TestWriteJsonProperty<DateTimeOffset>(DateTimeOffset.Now);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesString()
		{
			TestWriteJsonProperty<string>("Test value");
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesGuid()
		{
			TestWriteJsonProperty<Guid>(Guid.NewGuid());
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesChar()
		{
			TestWriteJsonProperty<char>('c');
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesFloat()
		{
			TestWriteJsonProperty<float>(1.3f);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesDecimal()
		{
			TestWriteJsonProperty<decimal>(29.99M);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesDouble()
		{
			TestWriteJsonProperty<double>((double)1.345);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesSingle()
		{
			TestWriteJsonProperty<Single>((Single)1.345);
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesByte()
		{
			TestWriteJsonProperty<Single>((byte)65);
		}


		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonProperty_WritesEncodedString()
		{
			TestWriteJsonProperty<string>("This string \twill require\r\nencoding for json due to some \\ funny \\ characters \f \u6771.");
		}

		#endregion

		#region WriteJsonObject Tests

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonObject_WritesValidComplexType()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				var test1 = new TestJson1()
				{
					Test = "Test1",
					IsTest = true,
					TestCount = 20
				};

				var sw = new System.Diagnostics.Stopwatch();
				sw.Start();
				jsonWriter.WriteJsonObject<TestJson1>(test1);
				sw.Stop();
				System.Diagnostics.Trace.WriteLine($"Complex type write took: {sw.Elapsed.ToString()}.");

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson1>(text);
				Assert.IsNotNull(result);
				Assert.AreEqual(test1.Test, result.Test);
				Assert.AreEqual(test1.IsTest, result.IsTest);
				Assert.AreEqual(test1.TestCount, result.TestCount);
				Assert.IsFalse(text.Contains("PrivateTestProperty"));
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		[Timeout(50)]
		public void JsonWriter_WriteJsonObject_ComplexTypePerfTest()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				int itemsToWrite = 10000;
				var list = new List<TestJson1>(itemsToWrite);
				for (int cnt = 0; cnt < itemsToWrite; cnt++)
				{
					list.Add(new TestJson1()
					{
						Test = "Test" + itemsToWrite.ToString(),
						IsTest = (cnt % 2) == 0,
						TestCount = cnt
					});
				}

				var sw = new System.Diagnostics.Stopwatch();
				for (int cnt = 0; cnt < itemsToWrite; cnt++)
				{
					var item = list[cnt];
					sw.Start();
					jsonWriter.WriteJsonObject<TestJson1>(item);
					sw.Stop();
				}
				System.Diagnostics.Trace.WriteLine($"Complex type write for {itemsToWrite.ToString()} items took: {sw.Elapsed.ToString()}.");
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonObject_WritesNestedComplexTypes()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				var test2 = new TestJson2()
				{
					Test = "Test2",
					SubValue = new TestJson1()
					{
						IsTest = true,
						Test = "Test1",
						TestCount = 15
					}
				};

				jsonWriter.WriteJsonObject<TestJson2>(test2);

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson2>(text);
				Assert.IsNotNull(result);
				Assert.AreEqual(test2.Test, result.Test);
				var test1 = result.SubValue;
				Assert.IsNotNull(test1);
				Assert.AreEqual(test2.SubValue.IsTest, test1.IsTest);
				Assert.AreEqual(test2.SubValue.TestCount, test1.TestCount);
				Assert.AreEqual(test2.SubValue.Test, test1.Test);
				Assert.IsFalse(text.Contains("PrivateTestProperty"));
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonObject_WritesNull()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				var test2 = new TestJson2()
				{
					Test = "Test2",
				};

				jsonWriter.WriteJsonObject<TestJson2>(test2);

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson2>(text);
				Assert.IsNotNull(result);
				Assert.AreEqual(test2.Test, result.Test);
				var test1 = result.SubValue;
				Assert.IsNull(test1);
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonObject_WritesDictionary()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				var dictionary = new Dictionary<string, object>();
				dictionary.Add("TestKey1", "TestValue1");
				dictionary.Add("TestKey2", "TestValue2");
				dictionary.Add("TestKey3", "TestValue3");

				jsonWriter.WriteJsonObject<Dictionary<string, object>>(dictionary);

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
				Assert.AreEqual(3, result.Count);
				Assert.AreEqual("TestValue1", result["TestKey1"]);
				Assert.AreEqual("TestValue2", result["TestKey2"]);
				Assert.AreEqual("TestValue3", result["TestKey3"]);
			}
		}

		[TestMethod]
		[TestCategory(nameof(JsonWriter))]
		[TestCategory("Infrastructure")]
		public void JsonWriter_WriteJsonObject_WritesDictionaryProperty()
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				var test = new TestJson3<Dictionary<string, object>>();
				test.Value = new Dictionary<string, object>();
				test.Value.Add("TestKey1", "TestValue1");
				test.Value.Add("TestKey2", "TestValue2");
				test.Value.Add("TestKey3", "TestValue3");

				jsonWriter.WriteJsonObject<TestJson3<Dictionary<string, object>>>(test);

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson3<Dictionary<string, object>>>(text);
				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Value);

				var dictionary = result.Value;
				Assert.AreEqual(3, dictionary.Count);
				Assert.AreEqual("TestValue1", dictionary["TestKey1"]);
				Assert.AreEqual("TestValue2", dictionary["TestKey2"]);
				Assert.AreEqual("TestValue3", dictionary["TestKey3"]);
			}
		}

		#endregion

		#region Utility Methods

		private void TestWriteJsonProperty<T>(T value)
		{
			using (var writer = new System.IO.StringWriter())
			using (var jsonWriter = new JsonWriter(writer, false))
			{
				jsonWriter.WriteObjectStart();

				jsonWriter.WriteJsonProperty("Value", value);

				jsonWriter.WriteObjectEnd();

				var text = writer.GetStringBuilder().ToString();

				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TestJson3<T>>(text);
				Assert.IsNotNull(result);
				Assert.AreEqual(value, result.Value);
			}
		}

		#endregion

		#region Internal Classes

		internal class TestJson1
		{
			public string Test { get; set; }
			public bool IsTest { get; set; }
			public int TestCount { get; set; }
			private string PrivateTestProperty { get; set; }
		}

		internal class TestJson2
		{
			public string Test { get; set; }
			public TestJson1 SubValue { get; set; }
		}

		public class TestJson3<T>
		{
			public T Value { get; set; }
		}

		#endregion

	}
}
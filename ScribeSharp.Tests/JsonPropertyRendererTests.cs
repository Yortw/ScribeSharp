using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.PropertyRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class JsonPropertyRendererTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("JsonPropertyRenderer")]
		[TestCategory("Renderers")]
		public void JsonPropertyRenderer_Constructor_ConstructsOk()
		{
			var renderer = new JsonPropertyRenderer();
		}

		#endregion

		#region RenderValue Tests

		[TestMethod]
		[TestCategory("JsonPropertyRenderer")]
		[TestCategory("Renderers")]
		public void JsonPropertyRenderer_RenderValue_RendersAllValuesToJson()
		{
			var renderer = new JsonPropertyRenderer();
			var testValue = new TestClass()
			{
				Count = 10,
				Name = "Test Name",
				TestDate = new DateTime(2016, 08, 05, 19, 53, 22)
			};

			var result = renderer.RenderValue(testValue).ToString();
			Assert.AreEqual("{\"Name\":\"Test Name\",\"Count\":10,\"TestDate\":\"2016-08-05T19:53:22\"}", result);
		}

		[TestMethod]
		[TestCategory("JsonPropertyRenderer")]
		[TestCategory("Renderers")]
		public void JsonPropertyRenderer_RenderValue_RendersNullAsNull()
		{
			var renderer = new JsonPropertyRenderer();
			var result = renderer.RenderValue(null);
			Assert.IsNull(result);
		}

		#endregion

	}
}
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
	public class XmlPropertyRendererTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("XmlPropertyRenderer")]
		[TestCategory("Renderers")]
		public void XmlPropertyRenderer_Constructor_ConstructsOk()
		{
			var renderer = new XmlPropertyRenderer(typeof(TestClass));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestCategory("XmlPropertyRenderer")]
		[TestCategory("Renderers")]
		public void XmlPropertyRenderer_Constructor_ThrowsOnNullType()
		{
			var renderer = new XmlPropertyRenderer((Type)null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestCategory("XmlPropertyRenderer")]
		[TestCategory("Renderers")]
		public void XmlPropertyRenderer_Constructor_ThrowsOnNullSerializer()
		{
			var renderer = new XmlPropertyRenderer((System.Xml.Serialization.XmlSerializer)null);
		}

		#endregion


		#region RenderValue Tests

		[TestMethod]
		[TestCategory("XmlPropertyRenderer")]
		[TestCategory("Renderers")]
		public void XmlPropertyRenderer_RenderValue_RendersAllValuesToXml()
		{
			var renderer = new XmlPropertyRenderer(typeof(TestClass));
			var testValue = new TestClass()
			{
				Count = 10,
				Name = "Test Name",
				TestDate = new DateTime(2016, 08, 05, 19, 53, 22)
			};

			var result = renderer.RenderValue(testValue).ToString();
			var xdoc = System.Xml.Linq.XDocument.Parse(result);
			var node = (from n in xdoc.Descendants("TestClass") select n).First();
			Assert.AreEqual("Test Name", node.Descendants("Name").First().Value);
			Assert.AreEqual("10", node.Descendants("Count").First().Value);
			Assert.AreEqual("2016-08-05T19:53:22", node.Descendants("TestDate").First().Value);
		}

		[TestMethod]
		[TestCategory("XmlPropertyRenderer")]
		[TestCategory("Renderers")]
		public void XmlPropertyRenderer_RenderValue_RendersNullAsNull()
		{
			var renderer = new XmlPropertyRenderer(typeof(TestClass));
			var result = renderer.RenderValue(null);
			Assert.AreEqual(null, result);
		}

		#endregion

	}
}
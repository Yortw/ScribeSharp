using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class ToStringRendererTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_Constructor_ConstructsOk()
		{
			var renderer = new PropertyRenderers.ToStringRenderer();
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_Constructor_AllowsNullFormatString()
		{
			var renderer = new PropertyRenderers.ToStringRenderer(null);
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_Constructor_AllowsEmptyFormatString()
		{
			var renderer = new PropertyRenderers.ToStringRenderer(String.Empty);
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_Constructor_AllowsNullFormatProvider()
		{
			var renderer = new PropertyRenderers.ToStringRenderer("G", null);
		}

		#endregion

		#region RenderValue Tests

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_RenderValue_RendersNullAsNull()
		{
			var renderer = new PropertyRenderers.ToStringRenderer("G", System.Globalization.CultureInfo.InvariantCulture);
			Assert.IsNull(renderer.RenderValue(null));
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_RenderValue_RendersValueUsingFormatStringAndCulture()
		{
			var renderer = new PropertyRenderers.ToStringRenderer("d", System.Globalization.CultureInfo.CurrentCulture);
			var now = DateTime.Now;
			Assert.AreEqual(now.ToString("d", System.Globalization.CultureInfo.CurrentCulture), renderer.RenderValue(now));
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_RenderValue_RendersValueWithoutFormatStringOrCulture()
		{
			var renderer = new PropertyRenderers.ToStringRenderer();
			var now = DateTime.Now;
			Assert.AreEqual(now.ToString(), renderer.RenderValue(now));
		}

		[TestMethod]
		[TestCategory("ToStringRenderer")]
		[TestCategory("Renderers")]
		public void ToStringRenderer_RenderValue_RendersValueWithOnlyFormatString()
		{
			var renderer = new PropertyRenderers.ToStringRenderer("d");
			var now = DateTime.Now;
			Assert.AreEqual(now.ToString("d"), renderer.RenderValue(now));
		}

		#endregion

	}
}
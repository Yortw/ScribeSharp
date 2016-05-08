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
	public class TypeRendererMapTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_Constructor_ConstructsOk()
		{
			var map = new TypeRendererMap();
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_Constructor_ConstructsOkWithNullMapping()
		{
			var map = new TypeRendererMap(null);
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_Constructor_ConstructsOkWithMapping()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
		}

		#endregion

		#region GetRenderer Tests

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_GetRendererWithGenericTypeReturnsRenderer()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			Assert.IsNotNull(map.GetRenderer<Exception>());
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_GetRendererReturnsRenderer()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			Assert.IsNotNull(map.GetRenderer(typeof(Exception)));
		}

		#endregion

		#region AddRenderer Tests

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void TypeRendererMap_AddRenderer_ThrowsOnNullType()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			map.AddRenderer(null, new ToStringRenderer());
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void TypeRendererMap_AddRenderer_ThrowsOnNullRenderer()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			map.AddRenderer(typeof(DateTime), null);
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_AddRenderer_ActuallyAddsRenderer()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			map.AddRenderer(typeof(DateTime), new ToStringRenderer());
			Assert.IsNotNull(map.GetRenderer<DateTime>());
		}

		#endregion

		#region RemoveRenderer Tests

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		public void TypeRendererMap_RemoveRenderer_ActuallyRemovesRenderer()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			map.RemoveRenderer(typeof(Exception));
			Assert.IsNull(map.GetRenderer<Exception>());
		}

		[TestMethod]
		[TestCategory("TypeRendererMap")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void TypeRendererMap_RemoveRenderer_ThrowsOnNullType()
		{
			var map = new TypeRendererMap(new KeyValuePair<Type, IPropertyRenderer>(typeof(Exception), new ExceptionAsXmlRenderer()));
			map.RemoveRenderer(null);
		}

		#endregion

	}
}
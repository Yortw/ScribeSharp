﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class RecyclingTextWriterTests
	{

		#region Constructors

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Constructor_ConstructsOk()
		{
			var writer = new RecyclableStringWriter();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Constructor_ThrowsOnNullStringBuilder()
		{
			var writer = new RecyclableStringWriter((StringBuilder)null);
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Constructor_AllowsNullFormatProvider()
		{
			var writer = new RecyclableStringWriter((IFormatProvider)null);
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Constructor_ConstructsOkWithStringBuilderAndFormatProvider()
		{
			var sb = new StringBuilder();
			var writer = new RecyclableStringWriter(sb, System.Globalization.CultureInfo.CurrentCulture);
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Constructor_FullConstructorConstructsOk()
		{
			var sb = new StringBuilder();
			var writer = new RecyclableStringWriter(sb, System.Globalization.CultureInfo.CurrentCulture, 1024, 512);
		}

		#endregion

		#region Dispose Tests

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Dispose_DoesNotDisposeInstance()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);
			writer.Dispose();

			writer.Write("Test");
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Dispose_ResetsStringBuilder()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Write("Test");
			Assert.AreEqual("Test", sb.ToString());

			writer.Dispose();

			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Dispose_SetsCapacityWhenOverSize()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb, System.Globalization.CultureInfo.CurrentCulture, 1024, 512);

			writer.Write(new string('A', 2048));
			writer.Dispose();

			Assert.AreEqual(512, sb.Capacity);
			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Dispose_DoesNotSetCapacityWhenUnderMaxSize()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Write(new string('A', 600));
			writer.Dispose();

			Assert.AreEqual(600, sb.Capacity);
			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		#endregion

		#region Close Tests

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Close_DoesNotCloseInstance()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Close();

			writer.Write("Test");
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Close_ResetsStringBuilder()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Write("Test");
			Assert.AreEqual("Test", sb.ToString());

			writer.Close();

			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Close_SetsCapacityWhenOverSize()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb, System.Globalization.CultureInfo.CurrentCulture, 1024, 512);

			writer.Write(new string('A', 2048));
			writer.Close();

			Assert.AreEqual(512, sb.Capacity);
			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_Close_DoesNotSetCapacityWhenUnderMaxSize()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Write(new string('A', 600));
			writer.Close();

			Assert.AreEqual(600, sb.Capacity);
			Assert.AreEqual(0, sb.Length);
			Assert.AreEqual(String.Empty, sb.ToString());
		}

		#endregion

		#region GetText Tests

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		public void RecyclingTextWriter_GetText_ReturnsWrittenText()
		{
			var sb = new System.Text.StringBuilder();
			var writer = new RecyclableStringWriter(sb);

			writer.Write("A test string. ");
			writer.Write("Some more test text.");

			var actual = writer.GetText();
			Assert.AreEqual("A test string. Some more test text.", actual);
		}

		#endregion

		#region Force Dispose Tests

		[TestMethod]
		[TestCategory("RecyclingTextWriter")]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void RecyclingTextWriter_ForceDispose_ReallyDisposes()
		{
			var writer = new RecyclableStringWriter();
			writer.ForceDispose();
			writer.Write("Test");
		}

		#endregion

	}
}
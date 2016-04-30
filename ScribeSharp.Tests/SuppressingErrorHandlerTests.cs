using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class SuppressingErrorHandlerTests
	{
		[TestMethod]
		[TestCategory("SuppressingErrorHandler")]
		public void SuppressingErrorHandler_ReturnsSuppressForLogExceptions()
		{
			var errorHandler = new SuppressingErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Suppress, errorHandler.ReportError(new LogException("Test")));
		}

		[TestMethod]
		[TestCategory("SuppressingErrorHandler")]
		public void SuppressingErrorHandler_ReturnsSuppressForNonLogExceptions()
		{
			var errorHandler = new SuppressingErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Suppress, errorHandler.ReportError(new NullReferenceException("Test")));
		}

		[TestMethod]
		[TestCategory("SuppressingErrorHandler")]
		public void SuppressingErrorHandler_ReturnsSuppressForNonLogWriterExceptions()
		{
			var errorHandler = new SuppressingErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Suppress, errorHandler.ReportError(new LogWriterException("Test")));
		}

		[TestMethod]
		[TestCategory("SuppressingErrorHandler")]
		public void SuppressingErrorHandler_RaisesEvent()
		{
			bool wasRaised = false;
			var errorHandler = new SuppressingErrorHandler();
			errorHandler.Error += (sender, args) => wasRaised = true;
			errorHandler.ReportError(new LogWriterException("Test"));
			Assert.AreEqual(true, wasRaised);
		}

		[TestMethod]
		[TestCategory("SuppressingErrorHandler")]
		public void SuppressingErrorHandler_IgnoresRecommendations()
		{
			var errorHandler = new SuppressingErrorHandler();
			errorHandler.Error += (sender, args) => args.HandleErrorRecommendation = LoggingErrorPolicy.Rethrow;
			errorHandler.ReportError(new LogWriterException("Test"));
			Assert.AreEqual(LoggingErrorPolicy.Suppress, errorHandler.ReportError(new LogWriterException("Test")));
		}

	}
}
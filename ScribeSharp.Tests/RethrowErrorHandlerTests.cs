using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class RethrowErrorHandlerTests
	{
		[TestMethod]
		[TestCategory("RethrowErrorHandler")]
		public void RethrowErrorHandler_ReturnsSuppressForLogExceptions()
		{
			var errorHandler = new RethrowErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Rethrow, errorHandler.ReportError(new LogException("Test")));
		}

		[TestMethod]
		[TestCategory("RethrowErrorHandler")]
		public void RethrowErrorHandler_ReturnsSuppressForNonLogExceptions()
		{
			var errorHandler = new RethrowErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Rethrow, errorHandler.ReportError(new NullReferenceException("Test")));
		}

		[TestMethod]
		[TestCategory("RethrowErrorHandler")]
		public void RethrowErrorHandler_ReturnsSuppressForNonLogWriterExceptions()
		{
			var errorHandler = new RethrowErrorHandler();
			Assert.AreEqual(LoggingErrorPolicy.Rethrow, errorHandler.ReportError(new LogWriterException("Test")));
		}

		[TestMethod]
		[TestCategory("RethrowErrorHandler")]
		public void RethrowErrorHandler_RaisesEvent()
		{
			bool wasRaised = false;
			var errorHandler = new RethrowErrorHandler();
			errorHandler.Error += (sender, args) => wasRaised = true;
			errorHandler.ReportError(new LogWriterException("Test"));
			Assert.AreEqual(true, wasRaised);
		}

		[TestMethod]
		[TestCategory("RethrowErrorHandler")]
		public void RethrowErrorHandler_IgnoresRecommendations()
		{
			var errorHandler = new RethrowErrorHandler();
			errorHandler.Error += (sender, args) => args.HandleErrorRecommendation = LoggingErrorPolicy.Suppress;
			Assert.AreEqual(LoggingErrorPolicy.Rethrow, errorHandler.ReportError(new LogWriterException("Test")));
		}

	}
}
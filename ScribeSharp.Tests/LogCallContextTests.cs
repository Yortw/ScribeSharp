using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class LogCallContextTests
	{

		[TestMethod]
		[TestCategory("LogCallContext")]
		public void LogCallContext_CanPushProperty()
		{
			using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
			{
				var firstProp = LogCallContext.CurrentProperties.First();
				Assert.AreEqual("Test Prop 1", firstProp.Key);
			}
		}

		[TestMethod]
		[TestCategory("LogCallContext")]
		public void LogCallContext_CanPushMultipleProperties()
		{
			using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
			{
				using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
				{
					var firstProp = LogCallContext.CurrentProperties.First();
					Assert.AreEqual("Test Prop 2", firstProp.Key);
					var lastProp = LogCallContext.CurrentProperties.Last();
					Assert.AreEqual("Test Prop 1", lastProp.Key);
				}
			}
		}

		[TestMethod]
		[TestCategory("LogCallContext")]
		public void LogCallContext_ContextMovesToOtherThread()
		{
			using (var signal = new System.Threading.ManualResetEvent(false))
			{
				using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
				{
					using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
					{
						bool assertsPassedOnOtherThread = false;
						System.Threading.ThreadPool.QueueUserWorkItem((reserved) =>
						{
							var firstProp = LogCallContext.CurrentProperties.First();
							Assert.AreEqual("Test Prop 2", firstProp.Key);
							var lastProp = LogCallContext.CurrentProperties.Last();
							Assert.AreEqual("Test Prop 1", lastProp.Key);
							assertsPassedOnOtherThread = true;
							signal.Set();
						});

						signal.WaitOne();
						Assert.IsTrue(assertsPassedOnOtherThread);
					}
				}
			}
		}

		[TestMethod]
		[TestCategory("LogCallContext")]
		public async Task LogCallContext_ContextMovesToOtherTask()
		{
			using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
			{
				using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
				{
					bool assertsPassedOnOtherThread = false;
					await Task.Factory.StartNew(() => 
					{
						var firstProp = LogCallContext.CurrentProperties.First();
						Assert.AreEqual("Test Prop 2", firstProp.Key);
						var lastProp = LogCallContext.CurrentProperties.Last();
						Assert.AreEqual("Test Prop 1", lastProp.Key);
						assertsPassedOnOtherThread = true;
					});

					Assert.IsTrue(assertsPassedOnOtherThread);
				}
			}
		}

		[TestMethod]
		[TestCategory("LogCallContext")]
		public async Task LogCallContext_ContextMovesToOtherTaskWithConfigureAwait()
		{
			using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
			{
				using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
				{
					bool assertsPassedOnOtherThread = false;
					await Task.Factory.StartNew(() =>
					{
						var firstProp = LogCallContext.CurrentProperties.First();
						Assert.AreEqual("Test Prop 2", firstProp.Key);
						var lastProp = LogCallContext.CurrentProperties.Last();
						Assert.AreEqual("Test Prop 1", lastProp.Key);
						assertsPassedOnOtherThread = true;
					}).ConfigureAwait(false);

					Assert.IsTrue(assertsPassedOnOtherThread);
				}
			}
		}

		[TestMethod]
		[TestCategory("LogCallContext")]
		public void LogCallContext_ContextClearedAfterPops()
		{
			using (var signal = new System.Threading.ManualResetEvent(false))
			{
				using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
				{
					using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
					{
						bool assertsPassedOnOtherThread = false;
						System.Threading.ThreadPool.QueueUserWorkItem((reserved) =>
						{
							var firstProp = LogCallContext.CurrentProperties.First();
							Assert.AreEqual("Test Prop 2", firstProp.Key);
							var lastProp = LogCallContext.CurrentProperties.Last();
							Assert.AreEqual("Test Prop 1", lastProp.Key);
							assertsPassedOnOtherThread = true;
							signal.Set();
						});

						signal.WaitOne();
						Assert.IsTrue(assertsPassedOnOtherThread);
					}
				}

				signal.Reset();
				var contextWasEmptyOnOtherThread = false;
				System.Threading.ThreadPool.QueueUserWorkItem((reserved) =>
				{
					contextWasEmptyOnOtherThread = !LogCallContext.CurrentProperties.Any();
					signal.Set();
				});

				signal.WaitOne();
				Assert.IsTrue(contextWasEmptyOnOtherThread);
			}
		}

	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
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

		[TestMethod]
		[TestCategory("LogCallContext")]
		public void LogCallContext_ContextProviderAddsFromCallContext()
		{
			var eventList = new List<LogEvent>();
			var policy = new LogPolicy()
			{
				ContextProviders = new ILogEventContextProvider[] { new ContextProviders.LogCallContextLogEventContextProvider() },
				LogWriter = new ListLogWriter(eventList, 10)
			};
			var logger = new Logger(policy);

			using (var signal = new System.Threading.ManualResetEvent(false))
			{
				using (var prop = LogCallContext.PushProperty("Test Prop 1", Guid.NewGuid()))
				{
					using (var prop2 = LogCallContext.PushProperty("Test Prop 2", Guid.NewGuid()))
					{
						System.Threading.ThreadPool.QueueUserWorkItem((reserved) =>
						{
							logger.WriteEvent("Test event");
							signal.Set();
						});

						signal.WaitOne();
						Assert.AreEqual(eventList.Last().Properties.Count, 2);
						Assert.AreEqual(eventList.Last().Properties.First().Key, "Test Prop 2");
						Assert.AreEqual(eventList.Last().Properties.Last().Key, "Test Prop 1");
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
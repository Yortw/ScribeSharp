using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.ContextProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	[TestClass]
	public class ContextProviderTests
	{

		#region AppDomainIdLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainIdLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void AppDomainIdLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new AppDomainIdLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainIdLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void AppDomainIdLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new AppDomainIdLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainIdLogEventContextProvider")]
		public void AppDomainIdLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new AppDomainIdLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(AppDomain.CurrentDomain.Id, logEvent.Properties["AppDomain Id"]);
		}

		#endregion

		#endregion

		#region AppDomainNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainNameLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void AppDomainNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new AppDomainNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainNameLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void AppDomainNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new AppDomainNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("AppDomainNameLogEventContextProvider")]
		public void AppDomainNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new AppDomainNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(AppDomain.CurrentDomain.FriendlyName, logEvent.Properties["AppDomain Name"]);
		}

		#endregion

		#endregion

		#region ApplicationNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ApplicationNameLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void ApplicationNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ApplicationNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ApplicationNameLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void ApplicationNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ApplicationNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ApplicationNameLogEventContextProvider")]
		public void ApplicationNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ApplicationNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(AppDomain.CurrentDomain?.ApplicationIdentity?.FullName ?? System.Diagnostics.Process.GetCurrentProcess().ProcessName, 
				logEvent.Properties["Application Name"]);
		}

		#endregion

		#endregion

		#region AssemblyVersionLogEntryContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(AssemblyVersionLogEntryContextProvider))]
		[TestCategory("ApiQualityTests")]
		public void AssemblyVersionLogEntryContextProvider_Constructor_ConstructsOk()
		{
			var provider = new AssemblyVersionLogEntryContextProvider(typeof(AssemblyVersionLogEntryContextProvider).Assembly);
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(AssemblyVersionLogEntryContextProvider))]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void AssemblyVersionLogEntryContextProvider_Constructor_ThrowsOnNullAssembly()
		{
			var provider = new AssemblyVersionLogEntryContextProvider(null);
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(AssemblyVersionLogEntryContextProvider))]
		[TestCategory("ApiQualityTests")]
		public void AssemblyVersionLogEntryContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new AssemblyVersionLogEntryContextProvider(typeof(AssemblyVersionLogEntryContextProvider).Assembly, null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(AssemblyVersionLogEntryContextProvider))]
		public void AssemblyVersionLogEntryContextProvider_AddProperties_AddsPropertyWithSpecifiedName()
		{
			var assembly = typeof(AssemblyVersionLogEntryContextProvider).Assembly;
			var provider = new AssemblyVersionLogEntryContextProvider(assembly, "ScribeSharp Version", null);
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(assembly.GetName().Version.ToString(), logEvent.Properties["ScribeSharp Version"]);
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(AssemblyVersionLogEntryContextProvider))]
		public void AssemblyVersionLogEntryContextProvider_AddProperties_AddsPropertyWithAssemblyName()
		{
			var assembly = typeof(AssemblyVersionLogEntryContextProvider).Assembly;
			var provider = new AssemblyVersionLogEntryContextProvider(assembly, null);
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(assembly.GetName().Version.ToString(), logEvent.Properties[assembly.GetName().Name]);
		}

		#endregion

		#endregion

		#region ClrVersionLogEntryContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ClrVersionLogEventContextProvider")]
		public void ClrVersionLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ClrVersionLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ClrVersionLogEventContextProvider")]
		public void ClrVersionLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ClrVersionLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ClrVersionLogEventContextProvider")]
		public void ClrVersionLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ClrVersionLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(Environment.Version.ToString(), logEvent.Properties["CLR Version"]);
		}

		#endregion

		#endregion

		#region DelegateLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("DelegateLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void DelegateLogEventContextProvider_Constructor_ThrowsOnNullPropertyName()
		{
			var provider = new DelegateLogEventContextProvider(null, () => Guid.NewGuid().ToString());
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("DelegateLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void DelegateLogEventContextProvider_Constructor_ThrowsOnNullFunction()
		{
			var provider = new DelegateLogEventContextProvider("Test Property", null);
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("DelegateLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void DelegateLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new DelegateLogEventContextProvider("Test Property", () => Guid.NewGuid().ToString());
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("DelegateLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void DelegateLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new DelegateLogEventContextProvider("Test Property", () => Guid.NewGuid().ToString(), null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("DelegateLogEventContextProvider")]
		public void DelegateLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new DelegateLogEventContextProvider("Test Property", () => Guid.NewGuid().ToString());
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Guid.Parse((string)logEvent.Properties["Test Property"]);
		}

		#endregion

		#endregion

		#region EntryAssemblyLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("EntryAssemblyLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void EntryAssemblyLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new EntryAssemblyLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("EntryAssemblyLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void EntryAssemblyLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new EntryAssemblyLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("EntryAssemblyLogEventContextProvider")]
		public void EntryAssemblyLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new EntryAssemblyLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Reflection.Assembly.GetEntryAssembly()?.FullName ?? String.Empty, logEvent.Properties["Entry Assembly Name"]);
		}

		#endregion

		#endregion

		#region FixedValueLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestCategory("ContextProviders")]
		[TestCategory("FixedValueLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void FixedValueLogEventContextProvider_Constructor_ThrowsOnNullPropertyName()
		{
			var provider = new FixedValueLogEventContextProvider(null, "Test Value");
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("FixedValueLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void FixedValueLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new FixedValueLogEventContextProvider("Test Property", "Test Value");
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("FixedValueLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void FixedValueLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new FixedValueLogEventContextProvider("Test Property", "Test Value", null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("FixedValueLogEventContextProvider")]
		public void FixedValueLogEventContextProvider_AddProperties_AddsProperty()
		{
			var propValue = System.Guid.NewGuid().ToString();

			var provider = new FixedValueLogEventContextProvider("Test Property", propValue);
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(propValue, logEvent.Properties["Test Property"]);
		}

		#endregion

		#endregion

		#region GuidEventInstanceIdLogEntryContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(GuidEventInstanceIdLogEntryContextProvider))]
		public void GuidEventInstanceIdLogEntryContextProvider_Constructor_ConstructsOk()
		{
			var provider = new GuidEventInstanceIdLogEntryContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(GuidEventInstanceIdLogEntryContextProvider))]
		public void GuidEventInstanceIdLogEntryContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new GuidEventInstanceIdLogEntryContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory(nameof(GuidEventInstanceIdLogEntryContextProvider))]
		public void GuidEventInstanceIdLogEntryContextProvider_AddProperties_AddsProperty()
		{
			var provider = new GuidEventInstanceIdLogEntryContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.IsTrue(logEvent.Properties.ContainsKey("Event Instance ID"));
			System.Guid.Parse(logEvent.Properties["Event Instance ID"].ToString());
		}

		#endregion

		#endregion

		#region LogCallContextLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("LogCallContextLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void LogCallContextLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new LogCallContextLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("LogCallContextLogEventContextProvider")]
		[TestCategory("ApiQualityTests")]
		public void LogCallContextLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new LogCallContextLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("LogCallContextLogEventContextProvider")]
		public void LogCallContextLogEventContextProvider_AddProperties_AddsProperty()
		{
			using (var prop = LogCallContext.PushProperty("Test Property", "Test Value"))
			{
				var provider = new LogCallContextLogEventContextProvider();
				var logEvent = new LogEvent()
				{
					EventName = "Test event",
					Properties = new Dictionary<string, object>()
				};
				provider.AddProperties(logEvent, null);
				Assert.AreEqual("Test Value", logEvent.Properties["Test Property"]);
			}
		}

		#endregion

		#endregion

		#region MachineNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("MachineNameLogEventContextProvider")]
		public void MachineNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new MachineNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("MachineNameLogEventContextProvider")]
		public void MachineNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new MachineNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("MachineNameLogEventContextProvider")]
		public void MachineNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new MachineNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(Environment.MachineName, logEvent.Properties["Machine Name"]);
		}

		#endregion

		#endregion

		#region OSUserNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSUserNameLogEventContextProvider")]
		public void OSUserNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new OSUserNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSUserNameLogEventContextProvider")]
		public void OSUserNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new OSUserNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSUserNameLogEventContextProvider")]
		public void OSUserNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new OSUserNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual((Environment.UserDomainName ?? String.Empty) + "\\" + Environment.UserName, logEvent.Properties["OS User"]);
		}

		#endregion

		#endregion

		#region OSVersionDescriptionLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionDescriptionLogEventContextProvider")]
		public void OSVersionDescriptionLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new OSVersionDescriptionLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionDescriptionLogEventContextProvider")]
		public void OSVersionDescriptionLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new OSVersionDescriptionLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionDescriptionLogEventContextProvider")]
		public void OSVersionDescriptionLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new OSVersionDescriptionLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(Environment.OSVersion.VersionString, logEvent.Properties["OS Version Description"]);
		}

		#endregion

		#endregion

		#region OSVersionLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionLogEventContextProvider")]
		public void OSVersionLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new OSVersionLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionLogEventContextProvider")]
		public void OSVersionLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new OSVersionLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("OSVersionLogEventContextProvider")]
		public void OSVersionLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new OSVersionLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(Environment.OSVersion.Version.ToString(), logEvent.Properties["OS Version"]);
		}

		#endregion

		#endregion

		#region ProcessIdLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessIdLogEventContextProvider")]
		public void ProcessIdLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ProcessIdLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessIdLogEventContextProvider")]
		public void ProcessIdLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ProcessIdLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessIdLogEventContextProvider")]
		public void ProcessIdLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ProcessIdLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Diagnostics.Process.GetCurrentProcess().Id, logEvent.Properties["Process ID"]);
		}

		#endregion

		#endregion

		#region ProcessNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessNameLogEventContextProvider")]
		public void ProcessNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ProcessNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessNameLogEventContextProvider")]
		public void ProcessNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ProcessNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ProcessNameLogEventContextProvider")]
		public void ProcessNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ProcessNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Diagnostics.Process.GetCurrentProcess().ProcessName, logEvent.Properties["Process Name"]);
		}

		#endregion

		#endregion

		#region StackTraceLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("StackTraceLogEventContextProvider")]
		public void StackTraceLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new StackTraceLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("StackTraceLogEventContextProvider")]
		public void StackTraceLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new StackTraceLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("StackTraceLogEventContextProvider")]
		public void StackTraceLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new StackTraceLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.IsFalse(String.IsNullOrWhiteSpace((string)logEvent.Properties["Stacktrace"]));
		}

		#endregion

		#endregion

		#region TerminalServerSessionIdLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("TerminalServerSessionIdLogEventContextProvider")]
		public void TerminalServerSessionIdLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new TerminalServerSessionIdLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("TerminalServerSessionIdLogEventContextProvider")]
		public void TerminalServerSessionIdLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new TerminalServerSessionIdLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("TerminalServerSessionIdLogEventContextProvider")]
		public void TerminalServerSessionIdLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new TerminalServerSessionIdLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Diagnostics.Process.GetCurrentProcess().SessionId, logEvent.Properties["Terminal Server Session Id"]);
		}

		#endregion

		#endregion

		#region ThreadIdLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadIdLogEventContextProvider")]
		public void ThreadIdLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ThreadIdLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadIdLogEventContextProvider")]
		public void ThreadIdLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ThreadIdLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadIdLogEventContextProvider")]
		public void ThreadIdLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ThreadIdLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Threading.Thread.CurrentThread.ManagedThreadId, logEvent.Properties["Thread ID"]);
		}

		#endregion

		#endregion

		#region ThreadNameLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadNameLogEventContextProvider")]
		public void ThreadNameLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ThreadNameLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadNameLogEventContextProvider")]
		public void ThreadNameLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ThreadNameLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadNameLogEventContextProvider")]
		public void ThreadNameLogEventContextProvider_AddProperties_AddsProperty()
		{
			System.Threading.Thread.CurrentThread.Name = "Test Thread";

			var provider = new ThreadNameLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Threading.Thread.CurrentThread.Name, logEvent.Properties["Thread Name"]);
		}

		#endregion

		#endregion

		#region ThreadPrincipalLogEventContextProvider Tests

		#region Constructor Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadPrincipalLogEventContextProvider")]
		public void ThreadPrincipalLogEventContextProvider_Constructor_ConstructsOk()
		{
			var provider = new ThreadPrincipalLogEventContextProvider();
		}

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadPrincipalLogEventContextProvider")]
		public void ThreadPrincipalLogEventContextProvider_Constructor_ConstructsOkWithNullFilter()
		{
			var provider = new ThreadPrincipalLogEventContextProvider(null);
		}

		#endregion

		#region AddProperties Tests

		[TestMethod]
		[TestCategory("ContextProviders")]
		[TestCategory("ThreadPrincipalLogEventContextProvider")]
		public void ThreadPrincipalLogEventContextProvider_AddProperties_AddsProperty()
		{
			var provider = new ThreadPrincipalLogEventContextProvider();
			var logEvent = new LogEvent()
			{
				EventName = "Test event",
				Properties = new Dictionary<string, object>()
			};
			provider.AddProperties(logEvent, null);
			Assert.AreEqual(System.Threading.Thread.CurrentPrincipal.Identity.Name, logEvent.Properties["Thread Principal"]);
		}

		#endregion

		#endregion

	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScribeSharp.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribeSharp.Tests
{
	//Some of these tests are not unit tests but integration tests.
	//They are fragile because they require a local sql express install with Windows Integration enabled and a database with appropriate schema
	//to be available at the time they are run. Therefore, they are marked with the ignore attribute, and must be run manually by those who
	//have done the work to support them in their local environment.

	[TestClass]
	public class SqlServerLogWriterTests
	{

		#region Constructor Tests

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnNullConnectionString()
		{
			var writer = new SqlServerLogWriter(null, "Test");
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnEmptyConnectionString()
		{
			var writer = new SqlServerLogWriter(String.Empty, "Test");
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnWhitespaceConnectionString()
		{
			var writer = new SqlServerLogWriter("  ", "Test");
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnNullTableName()
		{
			var writer = new SqlServerLogWriter(ConnectionString, null);
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnEmptyTableName()
		{
			var writer = new SqlServerLogWriter(ConnectionString, String.Empty);
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		[ExpectedException(typeof(ArgumentException))]
		public void SqlServerLogWriter_Constructor_ThrowsOnWhitespaceTableName()
		{
			var writer = new SqlServerLogWriter(ConnectionString, "  ");
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		public void SqlServerLogWriter_Constructor_ConstructsOkWithNullColumnMapping()
		{
			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", null);
		}

		[TestMethod]
		[TestCategory(nameof(SqlServerLogWriter))]
		[TestCategory("ApiQualityTests")]
		[TestCategory("Writers")]
		public void SqlServerLogWriter_Constructor_ConstructsOkWithNullFormatter()
		{
			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", null, null);
		}

		#endregion

		#region Write Tests

#if !RUN_SQLWRITERINTEGRATIONTESTS
		[Ignore]
#endif
		[TestMethod]
		public void SqlServerLogWriter_Write_WritesEvent()
		{
			TruncateTable();

			var mapping = new Dictionary<string, string>();
			mapping.Add("DateTime", "LogDate");
			mapping.Add("EventName", "Event");
			mapping.Add("EventType", "EventType");
			mapping.Add("Source", "Source");
			mapping.Add("SeverityLevel", "Severity");
			mapping.Add("Test Prop", "Data");
			mapping.Add("FullDetails", "StructuredData");

			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", mapping);

			writer.Write(new LogEvent()
			{
				EventName = "Test event",
				DateTime = DateTime.Now,
				Source = "Test Source",
				Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
			});

			var data = GetTableRows();
			Assert.AreEqual(1, data.Tables[0].Rows.Count);
			var logEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<LogEvent>((string)data.Tables[0].Rows[0]["StructuredData"]);
			Assert.AreEqual("Test event", logEvent.EventName);
			Assert.AreEqual("Test Source", logEvent.Source);
		}

#if !RUN_SQLWRITERINTEGRATIONTESTS
		[Ignore]
#endif
		[TestMethod]
		public void SqlServerLogWriter_Write_UsesEventFormatter()
		{
			TruncateTable();

			var mapping = new Dictionary<string, string>();
			mapping.Add("DateTime", "LogDate");
			mapping.Add("EventName", "Event");
			mapping.Add("EventType", "EventType");
			mapping.Add("Source", "Source");
			mapping.Add("SeverityLevel", "Severity");
			mapping.Add("Test Prop", "Data");
			mapping.Add("FullDetails", "StructuredData");

			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", mapping, Formatters.XmlLogEventFormatter.DefaultInstance);

			writer.Write(new LogEvent()
			{
				EventName = "Test event",
				DateTime = DateTime.Now,
				Source = "Test Source",
				Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
			});

			var data = GetTableRows();
			var eventDoc = System.Xml.Linq.XDocument.Parse((string)data.Tables[0].Rows[0]["StructuredData"]);
			Assert.IsNotNull(eventDoc);
			Assert.IsNotNull((from n in eventDoc.Descendants() where n.Name == "LogEvent" select n).FirstOrDefault());
		}

		#endregion

		#region WriteBatch Tests

#if !RUN_SQLWRITERINTEGRATIONTESTS
		[Ignore]
#endif
		[TestMethod]
		public void SqlServerLogWriter_WriteBatch_WritesLargeBatch()
		{
			TruncateTable();

			var mapping = new Dictionary<string, string>();
			mapping.Add("DateTime", "LogDate");
			mapping.Add("EventName", "Event");
			mapping.Add("EventType", "EventType");
			mapping.Add("Source", "Source");
			mapping.Add("SeverityLevel", "Severity");
			mapping.Add("Test Prop", "Data");

			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", mapping);

			var entryCount = 1000;
			var events = new LogEvent[entryCount];
			for (int cnt = 0; cnt < entryCount; cnt++)
			{
				events[cnt] = new LogEvent()
				{
					EventName = "Test event " + cnt.ToString(),
					DateTime = DateTime.Now,
					Source = "Test Source",
					Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
				};
			}

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			writer.WriteBatch(events);
			sw.Stop();
			System.Diagnostics.Trace.WriteLine($"SqlServerLogWriter: Write of {entryCount} entries took {sw.Elapsed.ToString()}");

			var data = GetTableRows();
			Assert.AreEqual(entryCount, data.Tables[0].Rows.Count);
		}

#if !RUN_SQLWRITERINTEGRATIONTESTS
		[Ignore]
#endif
		[TestMethod]
		public void SqlServerLogWriter_WriteBatch_WritesPartialArrayBatch()
		{
			TruncateTable();

			var mapping = new Dictionary<string, string>();
			mapping.Add("DateTime", "LogDate");
			mapping.Add("EventName", "Event");
			mapping.Add("EventType", "EventType");
			mapping.Add("Source", "Source");
			mapping.Add("SeverityLevel", "Severity");
			mapping.Add("Test Prop", "Data");

			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", mapping);

			var entryCount = 1000;
			var events = new LogEvent[entryCount];
			for (int cnt = 0; cnt < entryCount; cnt++)
			{
				events[cnt] = new LogEvent()
				{
					EventName = "Test event " + cnt.ToString(),
					DateTime = DateTime.Now,
					Source = "Test Source",
					Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
				};
			}

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			writer.WriteBatch(events, 10);
			sw.Stop();
			System.Diagnostics.Trace.WriteLine($"SqlServerLogWriter: Write of {10} entries took {sw.Elapsed.ToString()}");

			var data = GetTableRows();
			Assert.AreEqual(10, data.Tables[0].Rows.Count);
		}

#if !RUN_SQLWRITERINTEGRATIONTESTS
		[Ignore]
#endif
		[TestMethod]
		public void SqlServerLogWriter_WriteBatch_UsesEventFormatter()
		{
			TruncateTable();

			var mapping = new Dictionary<string, string>();
			mapping.Add("DateTime", "LogDate");
			mapping.Add("EventName", "Event");
			mapping.Add("EventType", "EventType");
			mapping.Add("Source", "Source");
			mapping.Add("SeverityLevel", "Severity");
			mapping.Add("Test Prop", "Data");
			mapping.Add("FullDetails", "StructuredData");

			var writer = new SqlServerLogWriter(ConnectionString, "AuditTrail", mapping, Formatters.XmlLogEventFormatter.DefaultInstance);

			var entryCount = 1000;
			var events = new LogEvent[entryCount];
			for (int cnt = 0; cnt < entryCount; cnt++)
			{
				events[cnt] = new LogEvent()
				{
					EventName = "Test event " + cnt.ToString(),
					DateTime = DateTime.Now,
					Source = "Test Source",
					Properties = new Dictionary<string, object>() { { "Test Prop", "Test Value" } }
				};
			}

			writer.WriteBatch(events, 10);

			var data = GetTableRows();
			var eventDoc = System.Xml.Linq.XDocument.Parse((string)data.Tables[0].Rows[0]["StructuredData"]);
			Assert.IsNotNull(eventDoc);
			Assert.IsNotNull((from n in eventDoc.Descendants() where n.Name == "LogEvent" select n).FirstOrDefault());
		}

		#endregion

		#region Utility Methods

		private const string ConnectionString = "Data Source=.\\SqlExpress;Initial Catalog=ScribeSharpTests;Integrated Security=true";

		private System.Data.DataSet GetTableRows()
		{
			return GetCommandResults("SELECT * FROM [AuditTrail]");
		}

		private void TruncateTable()
		{
			ExecuteCommand("TRUNCATE TABLE [AuditTrail]");
		}

		private void ExecuteCommand(string commandText)
		{
			using (var connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new System.Data.SqlClient.SqlCommand(commandText, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}

		private System.Data.DataSet GetCommandResults(string commandText)
		{
			using (var connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new System.Data.SqlClient.SqlCommand(commandText, connection))
				{
					using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
					{
						var ds = new System.Data.DataSet();
						adapter.Fill(ds);
						return ds;
					}
				}
			}
		}

		#endregion

	}
}
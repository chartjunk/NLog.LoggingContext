using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Targets;
using NLogContext.IntegrationTest.SQLite;
using NLogContext.Targets;
using NLogContext.TestUtils;
using static NLogContext.TestUtils.ContextUtils;

namespace NLogContext.IntegrationTest
{
    [TestClass]
    public class CustomDbTargetTests
    {
        public TestContext TestContext { get; set; }
        private Access _access;
        private static string UsernameIdentifier = "Username";
        private string _schemaTableName;
        private string _connectionString;

        [TestInitialize]
        public void Initialize()
        {
            _connectionString = "FullUri=file::memory:?cache=shared;";
            _schemaTableName = "LogForTest_" + TestContext.TestName;

            //NLog.Common.InternalLogger.LogToConsole = true;
            //NLog.Common.InternalLogger.LogLevel = LogLevel.Trace;
            //NLog.Common.InternalLogger.LogFile = "c:\\temp\\nlog-internal.txt";

            _access = new Access(_connectionString, _schemaTableName);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!_access.IsDisposed)
            {
                _access.ClearLogRows();
                _access.Dispose();
            }
        }

        [TestMethod]
        public void TestCustomUsernameColumnExtensionWithColumnName()
        {
            var columnName = "StringUsername";
            TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    target.WithColumn("${gdc:item=" + UsernameIdentifier + "}", columnName);
                    return columnName;
                },
                assertActualUsernameAction: (expectedUsername, logRow) => Assert.AreEqual(expectedUsername, logRow.StringUsername));
        }

        [TestMethod]
        public void TestCustomUsernameColumnExtensionWithSchemaPropertyExpression()
        {
            Expression<Func<DefaultLogSchemaWithUsername, string>> schemaExpression = r => r.SchemaUsername;
            TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    target.WithColumn("${gdc:item=" + UsernameIdentifier + "}", schemaExpression);
                    return "SchemaUsername";
                },
                assertActualUsernameAction: (expectedUsername, logRow) => Assert.AreEqual(expectedUsername,
                    logRow.SchemaUsername));
        }

        public void TestCustomUsernameColumnExtension(
            Func<NLogContextDbTarget<DefaultLogSchemaWithUsername>, string> withColumnNameFunc,
            Action<string, LogRow> assertActualUsernameAction)
        {
            // Assign
            var testUsername = "TestDummy";
            var targetName = "MyTarget";
            var testMsg = "Hallo world!";

            // Create custom target with an extra StringUsername column
            var target = new NLogContextDbTarget<DefaultLogSchemaWithUsername>(targetName, _schemaTableName);

            // Initialize DefaultSchema fields
            DefaultNLogContextDbTarget.DoDefaultInitialization(target, targetName, _schemaTableName);

            // Tell the custom target to get the Username-value from a gdc-entry for each log row
            var columnName = withColumnNameFunc(target);

            // Add an additional installation command for creating the StringUsername column
            target.InstallDdlCommands.Add(new DatabaseCommandInfo
            {
                Text = $"ALTER TABLE {_schemaTableName} ADD COLUMN {columnName} VARCHAR(100)",
                CommandType = CommandType.Text,
                IgnoreFailures = false
            });

            NLogContextDbTargetSetter.SetTarget(target, targetName, _connectionString, _schemaTableName);
            GlobalDiagnosticsContext.Set(UsernameIdentifier, testUsername);

            // Act
            DoWithContext(logger => logger.Info(testMsg));

            // Assert
            var logRow = _access.GetLogRows().Single();
            Assert.AreEqual(testMsg, logRow.Message);
            assertActualUsernameAction(testUsername, logRow);
        }
    }
}
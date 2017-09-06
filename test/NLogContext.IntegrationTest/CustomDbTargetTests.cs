using System;
using System.Data;
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

        /// <summary>
        /// TODO: clean this up and refactor initialization
        /// </summary>
        [TestMethod]
        public void TestCustomUsernameColumnExtension()
        {
            var testUsername = "TestDummy";
            var targetName = "MyTarget";
            var usernameColumnName = "TheUsername";
            var target = new NLogContextDbTarget<DefaultLogSchemaWithUsername>(targetName, _schemaTableName)
            {
                CommandType = System.Data.CommandType.Text,
                ConnectionString = _connectionString,
                DBProvider = typeof(System.Data.SQLite.SQLiteConnection).AssemblyQualifiedName
            };
            DefaultNLogContextDbTarget.DoDefaultInitialization(target, targetName, _schemaTableName);
            target.InstallDdlCommands.Add(new DatabaseCommandInfo
            {
                Text = $"ALTER TABLE {_schemaTableName} ADD COLUMN {usernameColumnName} VARCHAR(100)",
                CommandType = CommandType.Text,
                IgnoreFailures = false
            });

            // Map username from the Property to the gdc-item
            target.WithColumn(r => r.Username, "${gdc:item=" + UsernameIdentifier + "}", usernameColumnName);

            NLogContextDbTargetSetter.SetTarget(target, targetName, _connectionString, _schemaTableName);
            GlobalDiagnosticsContext.Set(UsernameIdentifier, testUsername);

            var testMsg = "Hallo world!";

            DoWithContext(logger => logger.Info(testMsg));

            var logRows = _access.GetLogRows();
        }
    }
}
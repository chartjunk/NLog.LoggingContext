﻿using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingScope.IntegrationTest.SQLite;
using NLog.LoggingScope.Targets;
using NLog.LoggingScope.TestUtils;
using NLog.Targets;

namespace NLog.LoggingScope.IntegrationTest
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
            => TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    var columnName = "StringUsername";
                    target.SetColumn("${gdc:item=" + UsernameIdentifier + "}", columnName);
                    return columnName;
                },
                act: (testUsername, testMsg) =>
                {
                    GlobalDiagnosticsContext.Set(UsernameIdentifier, testUsername);
                    ScopeUtils.DoWithContext(logger => logger.Info(testMsg));
                },
                getActualUsernameFunc: logRow => logRow.StringUsername);
        

        [TestMethod]
        public void TestCustomUsernameColumnExtensionWithSchemaPropertyExpressionWithDirectGdcSetting()
            => TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    Expression<Func<DefaultLogSchemaWithUsername, string>> schemaExpression = r => r.SchemaUsername;
                    target.SetColumn("${gdc:item=" + UsernameIdentifier + "}", schemaExpression);
                    return ReflectionUtils.GetPropertyInfo(schemaExpression).Name;
                },
                act: (testUsername, testMsg) =>
                {
                    GlobalDiagnosticsContext.Set(UsernameIdentifier, testUsername);
                    ScopeUtils.DoWithContext(logger => logger.Info(testMsg));
                },
                getActualUsernameFunc: logRow => logRow.SchemaUsername);


        [TestMethod]
        public void TestCustomUsernameColumnExtensionWithSchemaPropertyExpressionWithSchemaSetter()
            => TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    Expression<Func<DefaultLogSchemaWithUsername, string>> schemaExpression = r => r.SchemaUsername;
                    target.SetGdcColumn(schemaExpression);
                    return ReflectionUtils.GetPropertyInfo(schemaExpression).Name;
                },
                act: (testUsername, testMsg) => ScopeUtils.DoWithContext(logger => logger.Info(testMsg), 
                    getNewContext: cn => new LoggingScope(cn).WithSchema<DefaultLogSchemaWithUsername>(s => s.Set(f => f.SchemaUsername, testUsername))),
                getActualUsernameFunc: logRow => logRow.SchemaUsername);


        [TestMethod]
        public void TestCustomUsernameColumnExtensionWithSchemaPropertyExpressionWithSet()
            => TestCustomUsernameColumnExtension(
                withColumnNameFunc: target =>
                {
                    Expression<Func<DefaultLogSchemaWithUsername, string>> schemaExpression = r => r.SchemaUsername;
                    target.SetGdcColumn(schemaExpression);
                    return ReflectionUtils.GetPropertyInfo(schemaExpression).Name;
                },
                act: (testUsername, testMsg) => ScopeUtils.DoWithContext(logger => logger.Info(testMsg),
                    getNewContext: cn => new LoggingScope(cn).Set<DefaultLogSchemaWithUsername, string>(s => s.SchemaUsername, testUsername)),
                getActualUsernameFunc: logRow => logRow.SchemaUsername);


        public void TestCustomUsernameColumnExtension(
            Func<LoggingScopeDbTarget<DefaultLogSchemaWithUsername>, string> withColumnNameFunc,
            Action<string, string> act,
            Func<LogRow, string> getActualUsernameFunc)
        {
            // Assign
            var testUsername = "TestDummy";
            var targetName = "MyTarget";
            var testMsg = "Hallo world!";

            // Create custom target with an extra StringUsername column
            var target = new LoggingScopeDbTarget<DefaultLogSchemaWithUsername>
            {
                Name = targetName,
                SchemaTableName = _schemaTableName
            };

            // Initialize DefaultSchema fields
            DefaultLoggingScopeDbTarget.DoDefaultInitialization(target, _schemaTableName);

            // Tell the custom target to get the Username-value from a gdc-entry for each log row
            var columnName = withColumnNameFunc(target);

            // Add an additional installation command for creating the StringUsername column
            target.InstallDdlCommands.Add(new DatabaseCommandInfo
            {
                Text = $"ALTER TABLE {_schemaTableName} ADD COLUMN {columnName} VARCHAR(100)",
                CommandType = CommandType.Text,
                IgnoreFailures = false
            });

            LoggingScopeDbTargetSetter.SetTarget<DefaultLogSchemaWithUsername>(target, _connectionString);

            // Act
            act(testUsername, testMsg);

            // Assert
            var logRow = _access.GetLogRows().Single();
            Assert.AreEqual(testMsg, logRow.Message);
            Assert.AreEqual(testUsername, getActualUsernameFunc(logRow));
        }
    }
}
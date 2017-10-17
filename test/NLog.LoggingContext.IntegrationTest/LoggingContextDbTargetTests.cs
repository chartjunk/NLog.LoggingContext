using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingContext.TestUtils;

namespace NLog.LoggingContext.IntegrationTest
{
    [TestClass]
    public class LoggingContextDbTargetTests
    {
        public TestContext TestContext { get; set; }
        private SQLite.Access _access;

        [TestInitialize]
        public void Initialize()
        {
            var connectionString = "FullUri=file::memory:?cache=shared;";
            var schemaTableName = "LogForTest_" + TestContext.TestName;
            var targetName = "MyTarget";

            //NLog.Common.InternalLogger.LogToConsole = true;
            //NLog.Common.InternalLogger.LogLevel = LogLevel.Trace;
            //NLog.Common.InternalLogger.LogFile = "c:\\temp\\nlog-internal.txt";

            _access = new SQLite.Access(connectionString, schemaTableName);
            SQLite.LoggingContextDbTargetSetter.SetTarget(targetName, connectionString, schemaTableName);
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
        public void AddSimpleLogRow()
        {
            // Assign
            var testMessage = "Hello World";

            // Act
            ContextUtils.DoWithContext(logger => logger.Info(testMessage));

            // Assert
            var logRow = _access.GetLogRows().Single();
            Assert.AreEqual(testMessage, logRow.Message);
        }

        [TestMethod]
        public void AddMultipleLogRowsInSameContext()
        {
            // Assign
            var testMessage1 = "Foo";
            var testMessage2 = "Bar";

            // Act
            ContextUtils.DoWithContext(logger =>
            {
                logger.Info(testMessage1);
                logger.Info(testMessage2);
            });

            // Assert
            var logRows = _access.GetLogRows();
            Assert.AreEqual(2, logRows.Count);
             CollectionAssert.AreEquivalent(new[] { testMessage1, testMessage2 }, logRows.Select(r => r.Message).ToArray());
            Assert.AreEqual(1, logRows.Select(r => r.ContextId).Distinct().Count());
            Assert.IsTrue(logRows.All(r => r.ContextId == r.TopmostParentContextId));
        }

        [TestMethod]
        public void AddLogRowsInSeparateContexts()
        {
            // Assign
            var testMessage1 = "Foo";
            var testMessage2 = "Bar";

            // Act
            ContextUtils.DoWithContext(logger => logger.Info(testMessage1));
            ContextUtils.DoWithContext(logger => logger.Info(testMessage2));

            // Assert
            var logRows = _access.GetLogRows();
            Assert.AreEqual(2, logRows.Count);
            CollectionAssert.AreEquivalent(new[] { testMessage1, testMessage2 }, logRows.Select(r => r.Message).ToArray());
            Assert.AreEqual(2, logRows.Select(r => r.ContextId).Distinct().Count());
            Assert.IsTrue(logRows.All(r => r.ContextId == r.TopmostParentContextId));
        }

        [TestMethod]
        public void AddLogRowsInNestedContexts()
        {
            //Assign
            var testMessage1 = "Foo";
            var testMessage2 = "Bar";
            var testMessage3 = "Fizz";
            var context1Name = "Context1";
            var context2Name = "Context2";

            // Act
            ContextUtils.DoWithContext(context1Name, logger1 => 
            {
                logger1.Info(testMessage1);
                ContextUtils.DoWithContext(context2Name, logger2 =>
                {
                    logger1.Info(testMessage2);
                    logger2.Info(testMessage3);
                });
            });

            // Assert
            var logRows = _access.GetLogRows();
            Assert.AreEqual(1, logRows.Select(r => r.TopmostParentContextId).Distinct().Count());
            var context1Rows = logRows.Where(r => r.ContextName == context1Name).ToList();
            var context2Rows = logRows.Where(r => r.ContextName == context2Name).ToList();
            Assert.AreEqual(1, context1Rows.Count);
            Assert.AreEqual(2, context2Rows.Count);
            Assert.AreEqual(1, context1Rows.Where(r => r.ContextId == r.TopmostParentContextId).Count());
            Assert.AreEqual(1, context2Rows.Select(r => r.ContextId).Distinct().Count());
            Assert.AreEqual(context1Rows.Single().ContextId, context2Rows.Select(r => r.ParentContextId).Distinct().Single());
        }
    }
}

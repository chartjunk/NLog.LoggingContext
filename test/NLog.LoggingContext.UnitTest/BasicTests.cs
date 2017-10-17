using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingContext.TestUtils;
using NLog.LoggingContext.UnitTest.MethodCallTargeting;

namespace NLog.LoggingContext.UnitTest
{
    [TestClass]
    public class BasicTests
    {
        [TestInitialize]
        public void Initialize()
        {
            if (!MockTargetSingleton.IsSingletonInitialized)
            {
                //NLog.Common.InternalLogger.LogToConsole = true;
                //NLog.Common.InternalLogger.LogLevel = LogLevel.Trace;
                //NLog.Common.InternalLogger.LogFile = "c:\\temp\\nlog-internal.txt";

                MockTargetSingleton.InitializeSingleton();
                LoggingContextMethodCallTargetSetter.SetTarget("MockTarget", () => MockTargetSingleton.Log(null, null, null, null, null, null, null, null));
            }
        }

        [TestCleanup]
        public void Cleanup() => MockTargetSingleton.ClearLogRows();

        [TestMethod]
        public void TestLogRowEntry()
        { 
            // Assign
            var testMessage = "Hello World!";

            // Act
            var contextName = ContextUtils.DoWithContext(logger => logger.Info(testMessage));

            // Assert
            var row = MockTargetSingleton.GetLogRows().Single();
            Assert.AreEqual(contextName, row.ContextName);
            Assert.AreEqual(testMessage, row.Message);
            Assert.AreEqual(row.ContextId, row.TopmostParentContextId);
            Assert.AreEqual("", row.ParentContextId);
        }

        [TestMethod]
        public void TestSequentialLogRowEntryInSameContext()
        {
            // Assign
            var testMsg1 = "Hello";
            var testMsg2 = "World!";

            // Act
            var contextName = ContextUtils.DoWithContext(logger => 
            {
                logger.Info(testMsg1);
                logger.Info(testMsg2);
            });

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var row1 = rows.Single(r => r.Message == testMsg1);
            var row2 = rows.Single(r => r.Message != testMsg2);
            var contextId = row1.ContextId;
            new[] { row1, row2 }.ToList().ForEach(row =>
            {
                Assert.AreEqual(contextName, row.ContextName);
                Assert.AreEqual(contextId, row.ContextId);
                Assert.AreEqual(contextId, row.TopmostParentContextId);
                Assert.AreEqual("", row.ParentContextId);
            });
        }

        private void TestSequentialLogRowEntry(Action<string, string, string, string> actWithOuterMsgInnerMsgOuterCtxNameInnerCtxName)
        {
            // Assign
            var outerMsg = "Hello";
            var innerMsg = "World!";
            var outerCtxName = "Context1";
            var innerCtxName = "Context2";

            // Act
            actWithOuterMsgInnerMsgOuterCtxNameInnerCtxName(outerMsg, innerMsg, outerCtxName, innerCtxName);

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var innerRow = rows.Single(r => r.Message == innerMsg);
            var outerRow = rows.Single(r => r.Message == outerMsg);
            var innerCtxId = innerRow.ContextId;
            var outerCtxId = outerRow.ContextId;
            Assert.AreEqual(innerMsg, innerRow.Message);
            Assert.AreEqual(outerMsg, outerRow.Message);
            Assert.AreEqual(innerCtxName, innerRow.ContextName);
            Assert.AreEqual(outerCtxName, outerRow.ContextName);
            Assert.AreEqual("", outerRow.ParentContextId);
            Assert.AreEqual(outerCtxId, innerRow.ParentContextId);
            Assert.AreEqual(outerCtxId, outerRow.TopmostParentContextId);
            Assert.AreEqual(outerCtxId, innerRow.TopmostParentContextId);
        }

        [TestMethod]
        public void TestSequentialLogRowEntryFirstOuterThenInnerContext() => TestSequentialLogRowEntry((outerMsg, innerMsg, outerCtxName, innerCtxName) =>
        {
            ContextUtils.DoWithContext(outerCtxName, logger =>
            {
                logger.Info(outerMsg);
                ContextUtils.DoWithContext(innerCtxName, () => logger.Info(innerMsg));
            });
        });

        [TestMethod]
        public void TestSequentialLogRowEntryFirstInnerThenOuterContext() => TestSequentialLogRowEntry((outerMsg, innerMsg, outerCtxName, innerCtxName) =>
        {
            ContextUtils.DoWithContext(outerCtxName, logger =>
            {
                ContextUtils.DoWithContext(innerCtxName, () => logger.Info(innerMsg));
                logger.Info(outerMsg);
            });
        });

        [TestMethod]
        public void TestSubsequentContextsLogRowEntry()
        {
            // Assign
            var testMsg1 = "Hello";
            var testMsg2 = "World!";
            var ctxName1 = "Context1";
            var ctxName2 = "Context2";

            // Act
            ContextUtils.DoWithContext(ctxName1, logger => logger.Info(testMsg1));
            ContextUtils.DoWithContext(ctxName2, logger => logger.Info(testMsg2));

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var row1 = rows.Single(r => r.Message == testMsg1);
            var row2 = rows.Single(r => r.Message == testMsg2);
            Assert.IsTrue(row1.ContextId != row2.ContextId);
            new[] { row1, row2 }.ToList().ForEach(row =>
            {
                Assert.IsTrue(row.ContextId == row.TopmostParentContextId);
                Assert.AreEqual("", row.ParentContextId);
            });
        }
    }
}

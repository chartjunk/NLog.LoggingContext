using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingScope.TestUtils;
using NLog.LoggingScope.UnitTest.MethodCallTargeting;

namespace NLog.LoggingScope.UnitTest
{
    [TestClass]
    public class BasicTests : MockTargetSingletonTests
    {
        [TestMethod]
        public void TestLogRowEntry()
        { 
            // Assign
            var testMessage = "Hello World!";

            // Act
            var scopeName = ScopeUtils.DoWithContext(logger => logger.Info(testMessage));

            // Assert
            var row = MockTargetSingleton.GetLogRows().Single();
            Assert.AreEqual(scopeName, row.ScopeName);
            Assert.AreEqual(testMessage, row.Message);
            Assert.AreEqual(row.ScopeId, row.TopmostParentScopeId);
            Assert.AreEqual("", row.ParentScopeId);
        }

        [TestMethod]
        public void TestSequentialLogRowEntryInSameContext()
        {
            // Assign
            var testMsg1 = "Hello";
            var testMsg2 = "World!";

            // Act
            var scopeName = ScopeUtils.DoWithContext(logger => 
            {
                logger.Info(testMsg1);
                logger.Info(testMsg2);
            });

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var row1 = rows.Single(r => r.Message == testMsg1);
            var row2 = rows.Single(r => r.Message != testMsg2);
            var scopeId = row1.ScopeId;
            new[] { row1, row2 }.ToList().ForEach(row =>
            {
                Assert.AreEqual(scopeName, row.ScopeName);
                Assert.AreEqual(scopeId, row.ScopeId);
                Assert.AreEqual(scopeId, row.TopmostParentScopeId);
                Assert.AreEqual("", row.ParentScopeId);
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
            var innerCtxId = innerRow.ScopeId;
            var outerCtxId = outerRow.ScopeId;
            Assert.AreEqual(innerMsg, innerRow.Message);
            Assert.AreEqual(outerMsg, outerRow.Message);
            Assert.AreEqual(innerCtxName, innerRow.ScopeName);
            Assert.AreEqual(outerCtxName, outerRow.ScopeName);
            Assert.AreEqual("", outerRow.ParentScopeId);
            Assert.AreEqual(outerCtxId, innerRow.ParentScopeId);
            Assert.AreEqual(outerCtxId, outerRow.TopmostParentScopeId);
            Assert.AreEqual(outerCtxId, innerRow.TopmostParentScopeId);
        }

        [TestMethod]
        public void TestSequentialLogRowEntryFirstOuterThenInnerContext() => TestSequentialLogRowEntry((outerMsg, innerMsg, outerCtxName, innerCtxName) =>
        {
            ScopeUtils.DoWithContext(outerCtxName, logger =>
            {
                logger.Info(outerMsg);
                ScopeUtils.DoWithContext(innerCtxName, () => logger.Info(innerMsg));
            });
        });

        [TestMethod]
        public void TestSequentialLogRowEntryFirstInnerThenOuterContext() => TestSequentialLogRowEntry((outerMsg, innerMsg, outerCtxName, innerCtxName) =>
        {
            ScopeUtils.DoWithContext(outerCtxName, logger =>
            {
                ScopeUtils.DoWithContext(innerCtxName, () => logger.Info(innerMsg));
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
            ScopeUtils.DoWithContext(ctxName1, logger => logger.Info(testMsg1));
            ScopeUtils.DoWithContext(ctxName2, logger => logger.Info(testMsg2));

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var row1 = rows.Single(r => r.Message == testMsg1);
            var row2 = rows.Single(r => r.Message == testMsg2);
            Assert.IsTrue(row1.ScopeId != row2.ScopeId);
            new[] { row1, row2 }.ToList().ForEach(row =>
            {
                Assert.IsTrue(row.ScopeId == row.TopmostParentScopeId);
                Assert.AreEqual("", row.ParentScopeId);
            });
        }
    }
}

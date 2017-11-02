using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingScope.TestUtils;
using NLog.LoggingScope.UnitTest.MethodCallTargeting;

namespace NLog.LoggingScope.UnitTest
{
    [TestClass]
    public class ConcurrencyTests
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
                LoggingScopeMethodCallTargetSetter.SetTarget("MockTarget", () => MockTargetSingleton.Log(null, null, null, null, null, null, null, null));
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            MockTargetSingleton.ClearLogRows();
        }

        [TestMethod]
        public void TestLoggingInSimultaneousTasks()
        {
            // Assign
            var testMsg1 = "Hello";
            var testMsg2 = "World!";
            var ctxName1 = "Context1";
            var ctxName2 = "Context2";

            // Act
            // HACK: Replace Task.Delays with something more robust
            Task.WaitAll(
                Task.Run(() =>
                {
                    Task.Delay(100).Wait();
                    ScopeUtils.DoWithContext(ctxName1, logger => logger.Info(testMsg1));
                }),
                Task.Run(() =>
                {
                    ScopeUtils.DoWithContext(ctxName2, logger => logger.Info(testMsg2));
                    Task.Delay(100).Wait();
                }));

            // Assert
            var rows = MockTargetSingleton.GetLogRows().ToList();
            var row1 = rows.Single(r => r.Message == testMsg1);
            var row2 = rows.Single(r => r.Message == testMsg2);
            var ctxId1 = row1.ContextId;
            var ctxId2 = row2.ContextId;
            Assert.AreNotEqual(row1.ContextId, row2.ContextId);
            new[] { row1, row2 }.ToList().ForEach(row =>
            {
                Assert.AreEqual(row.ContextId, row.TopmostParentContextId);
                Assert.AreEqual("", row.ParentContextId);
            });
        }
    }
}

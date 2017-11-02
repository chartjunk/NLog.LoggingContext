using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingScope.UnitTest.MethodCallTargeting;

namespace NLog.LoggingScope.UnitTest
{
    [TestClass]
    public class MockTargetSingletonTests
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
                LoggingScopeMethodCallTargetSetter.SetTarget("MockTarget",
                    () => MockTargetSingleton.Log(null, null, null, null, null, null, null, null));
            }
        }

        [TestCleanup]
        public void Cleanup() => MockTargetSingleton.ClearLogRows();
    }
}
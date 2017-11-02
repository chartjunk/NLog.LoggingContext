using System.Collections.Generic;
using NLog.LoggingScope.TestUtils;

namespace NLog.LoggingScope.UnitTest.MethodCallTargeting
{
    public class MockTargetSingleton
    {
        private static MockTarget Instance { get; set; }

        public static void InitializeSingleton() => Instance = new MockTarget();
        public static bool IsSingletonInitialized => Instance != null;
        public static void ClearLogRows() => Instance.ClearLogRows();
        public static IEnumerable<LogRow> GetLogRows() => Instance.GetLogRows();
        public static void Log(
            string level,
            string message,
            string scopeName,
            string scopeId,
            string topmostParentScopeId,
            string dateTime,
            string parentScopeId,
            string exception)
            => Instance.Log(
                level,
                message,
                scopeName,
                scopeId,
                topmostParentScopeId,
                dateTime,
                parentScopeId,
                exception);
    }
}

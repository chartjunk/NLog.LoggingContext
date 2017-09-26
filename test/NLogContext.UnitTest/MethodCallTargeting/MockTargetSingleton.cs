﻿using System.Collections.Generic;
using Joona.NLogContext.TestUtils;

namespace Joona.NLogContext.UnitTest.MethodCallTargeting
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
            string contextName,
            string contextId,
            string topmostParentContextId,
            string dateTime,
            string parentContextId,
            string exception)
            => Instance.Log(
                level,
                message,
                contextName,
                contextId,
                topmostParentContextId,
                dateTime,
                parentContextId,
                exception);
    }
}

using System.Collections.Generic;
using NLog.LoggingContext.TestUtils;

namespace NLog.LoggingContext.UnitTest.MethodCallTargeting
{
    internal class MockTarget
    {
        private List<LogRow> _logRows = new List<LogRow>();

        public void Log(
            string level,
            string message,
            string contextName,
            string contextId,
            string topmostParentContextId,
            string dateTime,
            string parentContextId = null,
            string exception = null)
            => _logRows.Add(new LogRow
            {
                ContextId = contextId,
                ContextName = contextName,
                Exception = exception,
                Level = level,
                Message = message,
                ParentContextId = parentContextId,
                TopmostParentContextId = topmostParentContextId
            });

        public IEnumerable<LogRow> GetLogRows() => _logRows;
        public void ClearLogRows() => _logRows.Clear();
    }
}

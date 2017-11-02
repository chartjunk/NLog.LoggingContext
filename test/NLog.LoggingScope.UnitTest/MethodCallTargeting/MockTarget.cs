using System.Collections.Generic;
using NLog.LoggingScope.TestUtils;

namespace NLog.LoggingScope.UnitTest.MethodCallTargeting
{
    internal class MockTarget
    {
        private List<LogRow> _logRows = new List<LogRow>();

        public void Log(
            string level,
            string message,
            string scopeName,
            string scopeId,
            string topmostParentScopeId,
            string dateTime,
            string parentScopeId = null,
            string exception = null)
            => _logRows.Add(new LogRow
            {
                ScopeId = scopeId,
                ScopeName = scopeName,
                Exception = exception,
                Level = level,
                Message = message,
                ParentScopeId = parentScopeId,
                TopmostParentScopeId = topmostParentScopeId
            });

        public IEnumerable<LogRow> GetLogRows() => _logRows;
        public void ClearLogRows() => _logRows.Clear();
    }
}

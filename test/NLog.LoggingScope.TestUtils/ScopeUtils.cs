using System;

namespace NLog.LoggingScope.TestUtils
{
    public static class ScopeUtils
    {
        public static string DoWithContext(string scopeName, Action<Logger> action) => DoWithContext(action, scopeName);
        public static string DoWithContext(string scopeName, Action action) => DoWithContext(logger => action(), scopeName);
        public static string DoWithContext(Action<Logger> action, string scopeName = "MyContext", Func<string, LoggingScope> getNewContext = null)
        {
            getNewContext = getNewContext ?? (n => new LoggingScope(n));
            var logger = LogManager.GetCurrentClassLogger();
            using (getNewContext(scopeName))
                action(logger);

            return scopeName;
        }
    }
}

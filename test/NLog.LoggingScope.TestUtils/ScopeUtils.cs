using System;

namespace NLog.LoggingScope.TestUtils
{
    public static class ScopeUtils
    {
        public static string DoWithContext(string contextName, Action<Logger> action) => DoWithContext(action, contextName);
        public static string DoWithContext(string contextName, Action action) => DoWithContext(logger => action(), contextName);
        public static string DoWithContext(Action<Logger> action, string contextName = "MyContext", Func<string, LoggingScope> getNewContext = null)
        {
            getNewContext = getNewContext ?? (n => new LoggingScope(n));
            var logger = LogManager.GetCurrentClassLogger();
            using (getNewContext(contextName))
                action(logger);

            return contextName;
        }
    }
}

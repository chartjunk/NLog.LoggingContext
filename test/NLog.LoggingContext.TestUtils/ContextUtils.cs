using System;

namespace NLog.LoggingContext.TestUtils
{
    public static class ContextUtils
    {
        public static string DoWithContext(string contextName, Action<Logger> action) => DoWithContext(action, contextName);
        public static string DoWithContext(string contextName, Action action) => DoWithContext(logger => action(), contextName);
        public static string DoWithContext(Action<Logger> action, string contextName = "MyContext", Func<string, LoggingContext> getNewContext = null)
        {
            getNewContext = getNewContext ?? (n => new LoggingContext(n));
            var logger = LogManager.GetCurrentClassLogger();
            using (getNewContext(contextName))
                action(logger);

            return contextName;
        }
    }
}

using System;

namespace NLog.LoggingContext.TestUtils
{
    public static class ContextUtils
    {
        public static string DoWithContext(string contextName, Action<Logger> action) => DoWithContext(action, contextName);
        public static string DoWithContext(string contextName, Action action) => DoWithContext(logger => action(), contextName);
        public static string DoWithContext(Action<Logger> action, string contextName = "MyContext")
        {
            var logger = LogManager.GetCurrentClassLogger();
            using (new LoggingContext(contextName))
                action(logger);

            return contextName;
        }
    }
}

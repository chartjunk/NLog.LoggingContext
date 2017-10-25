using System;
using System.Collections.Generic;
using System.Text;
using NLog.Layouts;

namespace NLog.LoggingContext
{
    internal static class SharedLoggingContextConfig
    {
        private static readonly string Prefix = "LoggingContext:";

        internal static string TryGet(string key)
        {
            SimpleLayout value = null;
            //var appSettings = new NLog.Internal.ConfigurationManager().AppSettings;            
            LogManager.Configuration?.Variables?.TryGetValue(Prefix + key, out value);
            return value?.Text;
        }
    }
}

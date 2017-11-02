using NLog.Layouts;

namespace NLog.LoggingScope
{
    public static class Layouts
    {
        public static Layout ScopeIdLayout => GetMdlcLayout("ScopeId");
        public static Layout LevelLayout => "${level}";
        public static Layout MessageLayout => "${message}";
        public static Layout ExceptionLayout => "${exception}";
        public static Layout DateTimeLayout => "${date:format=yyyy-MM-dd HH:mm.ss}";
        
        private static string GetMdlcLayout(string shortKey) => $"${{mdlc:{DiagnosticContextUtils.Mdlc.GetMdlcLongKey(shortKey)}}}";
        public static string GetGdcLayout(string shortKey) => $"${{logging-scope-gdc:{DiagnosticContextUtils.Mdlc.GetMdlcLongKey(shortKey)}}}";
    }
}

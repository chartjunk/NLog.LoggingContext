using NLog.Layouts;

namespace NLog.LoggingContext
{
    public static class Layouts
    {
        public static Layout ContextIdLayout => GetMdlcLayout(Identifiers.ContextIdIdentifier);
        public static Layout ContextNameLayout => GetMdlcLayout(Identifiers.ContextNameIdentifier);
        public static Layout LevelLayout => "${level}";
        public static Layout MessageLayout => "${message}";
        public static Layout ExceptionLayout => "${exception}";
        public static Layout DateTimeLayout => "${date:format=yyyy-MM-dd HH:mm.ss}";
        public static Layout ParentContextIdLayout => GetMdlcLayout(Identifiers.ParentContextIdIdentifier);
        public static Layout TopmostParentContextIdLayout => GetMdlcLayout(Identifiers.TopmostParentContextIdIdentifier);

        private static string GetMdlcLayout(string parameterName) => $"${{mdlc:{parameterName}}}";
    }
}

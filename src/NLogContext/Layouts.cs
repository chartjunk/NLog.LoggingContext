using NLog.Layouts;
using static NLogContext.Identifiers;

namespace NLogContext
{
    public static class Layouts
    {
        public static Layout ContextIdLayout => GetMdlcLayout(ContextIdIdentifier);
        public static Layout ContextNameLayout => GetMdlcLayout(ContextNameIdentifier);
        public static Layout LevelLayout => "${level}";
        public static Layout MessageLayout => "${message}";
        public static Layout ExceptionLayout => "${exception}";
        public static Layout ParentContextIdLayout => GetMdlcLayout(ParentContextIdIdentifier);
        public static Layout TopmostParentContextIdLayout => GetMdlcLayout(TopmostParentContextIdIdentifier);

        private static string GetMdlcLayout(string parameterName) => $"${{mdlc:{parameterName}}}";
    }
}

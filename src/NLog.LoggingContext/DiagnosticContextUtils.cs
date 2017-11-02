using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace NLog.LoggingScope
{
    public static class DiagnosticContextUtils
    {
        private static readonly string LoggingScopeNamespacePrefix = "NLog.LoggingScope.";

        public static class Mdlc
        {
            public static string GetMdlcLongKey(string shortKey)
                => LoggingScopeNamespacePrefix + shortKey;

            public static string GetMdlcByShortKey(string shortKey) => GetMdlcByLongLey(GetMdlcLongKey(shortKey));

            public static string GetMdlcByLongLey(string longKey)
            {
                if (MappedDiagnosticsLogicalContext.Contains(longKey))
                    return MappedDiagnosticsLogicalContext.Get(longKey);
                return null;
            }

            public static void SetMdlcByShortKey(string shortKey, string value) =>
                SetMdlcByLongKey(GetMdlcLongKey(shortKey), value);

            public static void SetMdlcByLongKey(string longKey, string value) =>
                MappedDiagnosticsLogicalContext.Set(longKey, value);

            public static void RemoveMdlcByShortKey(string shortKey) => RemoveMdlcByLongKey(GetMdlcLongKey(shortKey));

            public static void RemoveMdlcByLongKey(string longKey)
            {
                if(MappedDiagnosticsLogicalContext.Contains(longKey))
                    MappedDiagnosticsLogicalContext.Remove(longKey);
            }

            internal static Dictionary<string, string> GetMdlcs() =>
                MappedDiagnosticsLogicalContext.GetNames()
                    .Where(n => n.StartsWith(LoggingScopeNamespacePrefix))
                    .ToDictionary(n => n, MappedDiagnosticsLogicalContext.Get);
        }

        public static class Gdc
        {
            public static string GetGdcLongKey(string shortKey, string contextId = "")
                => Mdlc.GetMdlcLongKey(shortKey) + ":" + contextId;

            public static string GetGdcByShortKey(string shortKey, string contextId) => GetGdcByLongKey(
                GetGdcLongKey(shortKey, contextId));

            public static string GetGdcByLongKey(string longKey)
            {
                if (GlobalDiagnosticsContext.Contains(longKey))
                    return GlobalDiagnosticsContext.Get(longKey);
                return null;
            }

            public static void SetGdcByShortKey(string shortKey, string contextId, string value) =>
                SetGdcByLongKey(GetGdcLongKey(shortKey, contextId), value);

            public static void SetGdcByLongKey(string longKey, string value) =>
                GlobalDiagnosticsContext.Set(longKey, value);

            public static void RemoveGdcByShortKey(string shortKey, string contextId) => RemoveGdcByLongKey(GetGdcLongKey(shortKey, contextId));

            public static void RemoveGdcByLongKey(string longKey)
            {
                if (GlobalDiagnosticsContext.Contains(longKey))
                    GlobalDiagnosticsContext.Remove(longKey);
            }

            public static void RemoveGdcByContextId(string contextId)
            {
                // TODO: Track context names within the context object and remove them straight by their names on disposal
                GlobalDiagnosticsContext.GetNames().Where(n =>
                
                    n.StartsWith(LoggingScopeNamespacePrefix)
                    && n.EndsWith(":" + contextId)

                ).ToList().ForEach(GlobalDiagnosticsContext.Remove);
            }

            public static void CopyGdcValues(string fromContextId, string toContextId)
            {
                var fromContextIdLength = fromContextId.Length;
                GlobalDiagnosticsContext.GetNames().Where(n =>
                
                    n.StartsWith(LoggingScopeNamespacePrefix)
                    && n.EndsWith(":" + fromContextId)

                ).ToList().ForEach(n =>
                {
                    var gdcLongKey = string.Join("", n.Take(n.Length - fromContextIdLength)) + toContextId;
                    SetGdcByLongKey(gdcLongKey, GetGdcByLongKey(n));
                });
            }

            internal static Dictionary<string, string> GetGdcs() =>
                GlobalDiagnosticsContext.GetNames()
                    .Where(n => n.StartsWith(LoggingScopeNamespacePrefix))
                    .ToDictionary(n => n, GlobalDiagnosticsContext.Get);
        }
    }
}

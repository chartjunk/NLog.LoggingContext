using System;

namespace NLog.LoggingContext
{
    public class LoggingContext : IDisposable
    {
        public static string ContextId { get => GetMdlcValue(Identifiers.ContextIdIdentifier); internal set => SetMdlcValue(Identifiers.ContextIdIdentifier, value); }
        public static string ContextName { get => GetMdlcValue(Identifiers.ContextNameIdentifier); internal set => SetMdlcValue(Identifiers.ContextNameIdentifier, value); }
        public static string ParentContextId { get => GetMdlcValue(Identifiers.ParentContextIdIdentifier); internal set => SetMdlcValue(Identifiers.ParentContextIdIdentifier, value); }
        public string ParentContextName { get; internal set; }
        public string ParentParentContextId { get; internal set; }
        public string ParentParentContextName { get; internal set; }
        public string TopmostParentContextId { get => GetMdlcValue(Identifiers.TopmostParentContextIdIdentifier); internal set => SetMdlcValue(Identifiers.TopmostParentContextIdIdentifier, value); }

        public LoggingContext(string contextName)
        {
            var contextId = GenerateContextId();
            PushContext(contextName, contextId);
        }

        public void Dispose() => PopContext();

        internal void PushContext(string contextName, string contextId)
        {
            if (ContextId != null)
            {
                if (ParentContextId != null)
                {
                    ParentParentContextId = ParentContextId;
                    ParentParentContextName = ParentContextName;
                }
                ParentContextId = ContextId;
                ParentContextName = ContextName;
            }

            if (TopmostParentContextId == null)
                TopmostParentContextId = contextId;

            ContextId = contextId;
            ContextName = contextName;
        }

        internal void PopContext()
        {
            if (TopmostParentContextId == ContextId)
                TopmostParentContextId = null;

            ContextId = ParentContextId;
            ContextName = ParentContextName;
            ParentContextId = ParentParentContextId;
            ParentContextName = ParentParentContextName;
            ParentParentContextId = null;
            ParentParentContextName = null;
        }

        internal static string GenerateContextId() => Guid.NewGuid().ToString();
        internal static string GetMdlcValue(string key)
        {
            return MappedDiagnosticsLogicalContext.Contains(key) ? MappedDiagnosticsLogicalContext.Get(key) : null;
        }
        internal static void SetMdlcValue(string key, string value)
        {
            if (value == null)
                MappedDiagnosticsLogicalContext.Remove(key);
            else
                MappedDiagnosticsLogicalContext.Set(key, value);
        }
    }
}

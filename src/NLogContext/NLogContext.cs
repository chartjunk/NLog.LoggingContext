using System;
using NLog;

namespace Joona.NLogContext
{
    public class NLogContext : IDisposable
    {
        internal static string ContextId { get => GetMdlcValue(Identifiers.ContextIdIdentifier); set => SetMdlcValue(Identifiers.ContextIdIdentifier, value); }
        internal static string ContextName { get => GetMdlcValue(Identifiers.ContextNameIdentifier); set => SetMdlcValue(Identifiers.ContextNameIdentifier, value); }
        internal static string ParentContextId { get => GetMdlcValue(Identifiers.ParentContextIdIdentifier); set => SetMdlcValue(Identifiers.ParentContextIdIdentifier, value); }
        internal string ParentContextName { get; set; }
        internal string ParentParentContextId { get; set; }
        internal string ParentParentContextName { get; set; }
        internal string TopmostParentContextId { get => GetMdlcValue(Identifiers.TopmostParentContextIdIdentifier); set => SetMdlcValue(Identifiers.TopmostParentContextIdIdentifier, value); }

        public NLogContext(string contextName)
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

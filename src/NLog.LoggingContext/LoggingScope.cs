using System;
using System.Linq.Expressions;

namespace NLog.LoggingScope
{
    public class LoggingScope : IDisposable
    {
        private string _parentContextId;
        private string _parentParentContextId;
        public string ContextId { get; }

        public LoggingScope(string contextName)
        {
            ContextId = GenerateContextId();
            PushContext(contextName);
        }        

        public void Dispose() => PopContext();
    
        internal void PushContext(string contextName)
        {
            // Get parentContextId from parent context if one exists
            _parentContextId = DiagnosticContextUtils.Mdlc.GetMdlcByShortKey("ContextId");
            _parentParentContextId = DiagnosticContextUtils.Gdc.GetGdcByShortKey("ParentContextId", _parentContextId);

            DiagnosticContextUtils.Mdlc.SetMdlcByShortKey("ContextId", ContextId);

            // Roll parent context values to the current context as needed
            if (_parentContextId != null)
            {
                // Copy parent context values to the current context. This has to be done before setting the overriding values of the current context.
                DiagnosticContextUtils.Gdc.CopyGdcValues(_parentContextId, ContextId);
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("ParentContextId", ContextId, _parentContextId);
            }
            else
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("TopmostParentContextId", ContextId, ContextId);

            if (_parentParentContextId != null)
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("ParentParentContextId", ContextId, _parentParentContextId);
            
            // Store the rest of the static context values
            DiagnosticContextUtils.Gdc.SetGdcByShortKey("ContextName", ContextId, contextName);
        }

        internal void PopContext()
        {
            DiagnosticContextUtils.Gdc.RemoveGdcByContextId(ContextId);

            // Restore context ids
            if (_parentContextId != null)
                DiagnosticContextUtils.Mdlc.SetMdlcByShortKey("ContextId", _parentContextId);
            else            
                DiagnosticContextUtils.Mdlc.RemoveMdlcByShortKey("ContextId");

            // All parent context values should still reside at Gdc
        }
        
        internal static string GenerateContextId() => Guid.NewGuid().ToString();
    }
}

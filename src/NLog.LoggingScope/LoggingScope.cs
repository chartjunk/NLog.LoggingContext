using System;
using System.Linq.Expressions;

namespace NLog.LoggingScope
{
    public class LoggingScope : IDisposable
    {
        private string _parentScopeId;
        private string _parentParentScopeId;
        public string ScopeId { get; }

        public LoggingScope(string scopeName)
        {
            ScopeId = GenerateScopeId();
            PushContext(scopeName);
        }        

        public void Dispose() => PopContext();
    
        internal void PushContext(string scopeName)
        {
            // Get parentScopeId from parent context if one exists
            _parentScopeId = DiagnosticContextUtils.Mdlc.GetMdlcByShortKey("ScopeId");
            _parentParentScopeId = DiagnosticContextUtils.Gdc.GetGdcByShortKey("ParentScopeId", _parentScopeId);

            DiagnosticContextUtils.Mdlc.SetMdlcByShortKey("ScopeId", ScopeId);

            // Roll parent context values to the current context as needed
            if (_parentScopeId != null)
            {
                // Copy parent context values to the current context. This has to be done before setting the overriding values of the current context.
                DiagnosticContextUtils.Gdc.CopyGdcValues(_parentScopeId, ScopeId);
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("ParentScopeId", ScopeId, _parentScopeId);
            }
            else
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("TopmostParentScopeId", ScopeId, ScopeId);

            if (_parentParentScopeId != null)
                DiagnosticContextUtils.Gdc.SetGdcByShortKey("ParentParentScopeId", ScopeId, _parentParentScopeId);
            
            // Store the rest of the static context values
            DiagnosticContextUtils.Gdc.SetGdcByShortKey("ScopeName", ScopeId, scopeName);
        }

        internal void PopContext()
        {
            DiagnosticContextUtils.Gdc.RemoveGdcByScopeId(ScopeId);

            // Restore context ids
            if (_parentScopeId != null)
                DiagnosticContextUtils.Mdlc.SetMdlcByShortKey("ScopeId", _parentScopeId);
            else            
                DiagnosticContextUtils.Mdlc.RemoveMdlcByShortKey("ScopeId");

            // All parent context values should still reside at Gdc
        }
        
        internal static string GenerateScopeId() => Guid.NewGuid().ToString();
    }
}

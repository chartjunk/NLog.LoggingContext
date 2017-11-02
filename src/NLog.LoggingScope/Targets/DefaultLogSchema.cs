namespace NLog.LoggingScope.Targets
{
    public class DefaultLogSchema
    {
        public long Id { get; set; }
        public string ScopeId { get; set; }
        public string ScopeName { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string ParentScopeId { get; set; }
        public string TopmostParentScopeId { get; set; }
    }
}
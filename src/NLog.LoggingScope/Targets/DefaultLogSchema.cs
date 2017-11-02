namespace NLog.LoggingScope.Targets
{
    public class DefaultLogSchema
    {
        public long Id { get; set; }
        public string ContextId { get; set; }
        public string ContextName { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string ParentContextId { get; set; }
        public string TopmostParentContextId { get; set; }
    }
}
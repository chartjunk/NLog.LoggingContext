﻿namespace NLog.LoggingScope.TestUtils
{
    public class LogRow
    {
        public long? Id { get; set; }
        public string dateTime { get; set; }
        public string ScopeId { get; set; }
        public string ScopeName { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string ParentScopeId { get; set; }
        public string TopmostParentScopeId { get; set; }

        /// <summary>
        /// TODO: refactor these to another class that inherits LogRow
        /// </summary>
        public string StringUsername { get; set; }
        public string SchemaUsername { get; set; }
    }
}

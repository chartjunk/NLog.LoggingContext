using NLog.LoggingScope.Targets;

namespace NLog.LoggingScope.TestUtils
{
    public class DefaultLogSchemaWithUsername : DefaultLogSchema
    {
        public string SchemaUsername { get; set; }
    }
}
using NLog.LoggingContext.Targets;

namespace NLog.LoggingContext.TestUtils
{
    public class DefaultLogSchemaWithUsername : DefaultLogSchema
    {
        public string SchemaUsername { get; set; }
    }
}
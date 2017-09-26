using Joona.NLogContext.Targets;

namespace Joona.NLogContext.TestUtils
{
    public class DefaultLogSchemaWithUsername : DefaultLogSchema
    {
        public string SchemaUsername { get; set; }
    }
}
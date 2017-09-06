using NLogContext.Targets;

namespace NLogContext.TestUtils
{
    public class DefaultLogSchemaWithUsername : DefaultLogSchema
    {
        public string Username { get; set; }
    }
}
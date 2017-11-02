using System.Linq;
using NLog.Config;
using NLog.LoggingScope.Targets;
using NLog.Targets;

namespace NLog.LoggingScope.IntegrationTest.SQLite
{
    internal static class LoggingScopeDbTargetSetter
    {
        public static void SetTarget<TLogSchema>(
            LoggingScopeDbTarget<TLogSchema> target, string connectionString) where TLogSchema : class
        {
            target.CommandType = System.Data.CommandType.Text;
            target.ConnectionString = connectionString;
            target.DBProvider = typeof(System.Data.SQLite.SQLiteConnection).AssemblyQualifiedName;

            // HACK: SQLite crashes if type is *CHAR(MAX) instead of *CHAR(123)
            var command = Enumerable.First<DatabaseCommandInfo>(target.InstallDdlCommands).Text.ToString().Replace("CHAR(MAX)", "CHAR(123)").Replace("'", "");
            var newCommand = new DatabaseCommandInfo
            {
                Text = command,
                CommandType = System.Data.CommandType.Text,
                IgnoreFailures = false
            };
            target.InstallDdlCommands[0] = newCommand;
            target.Install(new InstallationContext { IgnoreFailures = false });
            var rule = new LoggingRule("*", LogLevel.Trace, target);
            LogManager.Configuration = new LoggingConfiguration();
            LogManager.Configuration.AddTarget(target.Name, target);
            LogManager.Configuration.LoggingRules.Add(rule);
            LogManager.ReconfigExistingLoggers();
        }

        public static void SetTarget(string targetName, string connectionString, string schemaTableName)
        {
            var target = new DefaultLoggingScopeDbTarget
            {
                Name = targetName,
                SchemaTableName = schemaTableName
            };
            SetTarget(target, connectionString);
        }

        public static void InstallTarget<TLogSchema>(LoggingScopeDbTarget<TLogSchema> target) where TLogSchema : class
        {
            // HACK: SQLite crashes if type is *CHAR(MAX) instead of *CHAR(123)
            var command = Enumerable.First<DatabaseCommandInfo>(target.InstallDdlCommands).Text.ToString().Replace("CHAR(MAX)", "CHAR(123)").Replace("'", "");
            var newCommand = new DatabaseCommandInfo
            {
                Text = command,
                CommandType = System.Data.CommandType.Text,
                IgnoreFailures = false
            };
            target.InstallDdlCommands[0] = newCommand;
            target.Install(new InstallationContext { IgnoreFailures = false });
        }
    }
}

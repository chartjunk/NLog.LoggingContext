using NLog;
using NLog.Config;
using NLog.Targets;
using NLogContext.Targets;
using System.Linq;

namespace NLogContext.IntegrationTest.SQLite
{
    internal static class NLogContextDbTargetSetter
    {
        public static void SetTarget<TLogSchema>(
            NLogContextDbTarget<TLogSchema> target, string connectionString)
        {
            target.CommandType = System.Data.CommandType.Text;
            target.ConnectionString = connectionString;
            target.DBProvider = typeof(System.Data.SQLite.SQLiteConnection).AssemblyQualifiedName;

            // HACK: SQLite crashes if type is *CHAR(MAX) instead of *CHAR(123)
            var command = target.InstallDdlCommands.First().Text.ToString().Replace("CHAR(MAX)", "CHAR(123)").Replace("'", "");
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
            var target = new DefaultNLogContextDbTarget(targetName, schemaTableName);
            SetTarget(target, connectionString);
        }
    }
}

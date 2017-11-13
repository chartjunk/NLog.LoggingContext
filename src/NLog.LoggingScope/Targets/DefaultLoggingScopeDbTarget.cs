using System.Linq;
using NLog.Targets;

namespace NLog.LoggingScope.Targets
{
    [Target("DefaultLoggingScopeDbTarget")]
    public class DefaultLoggingScopeDbTarget : LoggingScopeDbTarget<DefaultLogSchema>
    {
        internal static string GetInstallCommandSql(string schemaTableName) => 
            $"CREATE TABLE {schemaTableName} ( " +
            "  Id IDENTITY BIGINT PRIMARY KEY, " +
            "  [ScopeId] CHAR(36) NOT NULL, " +
            "  [ScopeName] VARCHAR(128), " +
            "  [Level] VARCHAR(16), " +
            "  [Message] NVARCHAR(MAX), " +
            "  [Exception] NVARCHAR(MAX), " +
            "  [InnerException] NVARCHAR(MAX), " +
            "  [ParentScopeId] CHAR(36), " +
            "  [TopmostParentScopeId] CHAR(36) NOT NULL " +
            ") ";

        public override string SchemaTableName
        {
            get => base.SchemaTableName;
            set
            {
                base.SchemaTableName = value;
                DoDefaultInitialization(this, value);
            }
        }

        public DefaultLoggingScopeDbTarget() : base()
        {
            
        }

        public static void DoDefaultInitialization<TDefaultLogSchema>(
            LoggingScopeDbTarget<TDefaultLogSchema> target, string schemaTableName = null)
            where TDefaultLogSchema : DefaultLogSchema
        {
            if (schemaTableName != null)
            {
                // Set table creation and dropping commands
                // NOTE: These are necessary only if you use target.Install(...) and target.Uninstall(...) explicitly
                target.InstallDdlCommands.Add(new DatabaseCommandInfo
                {
                    Text = GetInstallCommandSql(schemaTableName),
                    CommandType = System.Data.CommandType.Text,
                    IgnoreFailures = false
                });
                target.UninstallDdlCommands.Add(new DatabaseCommandInfo
                {
                    Text = $"DROP TABLE {schemaTableName}",
                    CommandType = System.Data.CommandType.Text,
                    IgnoreFailures = false
                });
            }

            // Set parameters
            target.AddColumn(Layouts.ScopeIdLayout, d => d.ScopeId);
            target.AddGdcColumn(d => d.ScopeName);            ;
            target.AddColumn(Layouts.LevelLayout, d => d.Level);
            target.AddColumn(Layouts.MessageLayout, d => d.Message);
            target.AddColumn(Layouts.ExceptionLayout, d => d.Exception);
            target.AddGdcColumn(d => d.ParentScopeId);
            target.AddGdcColumn(d => d.TopmostParentScopeId);
            target.RefreshInsertCommandText();
        }
    }
}
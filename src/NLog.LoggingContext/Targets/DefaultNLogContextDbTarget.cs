using NLog.Targets;

namespace NLog.LoggingContext.Targets
{
    [Target("DefaultLoggingContextDbTarget")]
    public sealed class DefaultLoggingContextDbTarget : LoggingContextDbTarget<DefaultLogSchema>
    {
        internal static string GetInstallCommandSql(string schemaTableName) => 
            $"CREATE TABLE {schemaTableName} ( " +
            "  Id IDENTITY BIGINT PRIMARY KEY, " +
            "  [ContextId] CHAR(36) NOT NULL, " +
            "  [ContextName] VARCHAR(128), " +
            "  [Level] VARCHAR(16), " +
            "  [Message] NVARCHAR(MAX), " +
            "  [Exception] NVARCHAR(MAX), " +
            "  [InnerException] NVARCHAR(MAX), " +
            "  [ParentContextId] CHAR(36), " +
            "  [TopmostParentContextId] CHAR(36) NOT NULL " +
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

        public static void DoDefaultInitialization<TDefaultLogSchema>(
            LoggingContextDbTarget<TDefaultLogSchema> target, string schemaTableName)
            where TDefaultLogSchema : DefaultLogSchema
        {
            // Set table creation and dropping commands
            // NOTE: These are necessary only if you use target.Install(...) and target.Uninstall(...) explicitly
            target.InstallDdlCommands.Add(new DatabaseCommandInfo
            {
                Text = GetInstallCommandSql(schemaTableName),
                CommandType = System.Data.CommandType.Text,
                IgnoreFailures = false
            });
            target.UninstallDdlCommands.Add(new DatabaseCommandInfo { Text = $"DROP TABLE {schemaTableName}", CommandType = System.Data.CommandType.Text, IgnoreFailures = false });

            // Set paremeters
            target.AddColumn(Layouts.ContextIdLayout, d => d.ContextId);
            target.AddColumn(Layouts.ContextNameLayout, d => d.ContextName);
            target.AddColumn(Layouts.LevelLayout, d => d.Level);
            target.AddColumn(Layouts.MessageLayout, d => d.Message);
            target.AddColumn(Layouts.ExceptionLayout, d => d.Exception);
            target.AddColumn(Layouts.ParentContextIdLayout, d => d.ParentContextId);
            target.AddColumn(Layouts.TopmostParentContextIdLayout, d => d.TopmostParentContextId);
            target.RefreshInsertCommandText();
        }
    }
}
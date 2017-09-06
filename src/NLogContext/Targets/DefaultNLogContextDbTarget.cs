using System;
using NLog.Targets;

namespace NLogContext.Targets
{
    public class DefaultNLogContextDbTarget : NLogContextDbTarget<DefaultLogSchema>
    {
        public DefaultNLogContextDbTarget(string name, string schemaTableName) : base(name, schemaTableName)
            => DoDefaultInitialization(this, name, schemaTableName);

        public static void DoDefaultInitialization<TDefaultLogSchema>(
            NLogContextDbTarget<TDefaultLogSchema> target, 
            string name, string schemaTableName)
            where TDefaultLogSchema : DefaultLogSchema
        {
            // Set table creation and dropping commands
            // NOTE: These are necessary only if you use target.Install(...) and target.Uninstall(...) explicitly
            target.InstallDdlCommands.Add(new DatabaseCommandInfo
            {
                Text =
                    $"CREATE TABLE {schemaTableName} ( " +
                    $"  Id IDENTITY BIGINT PRIMARY KEY, " +
                    $"  [ContextId] CHAR(36) NOT NULL, " +
                    $"  [ContextName] VARCHAR(128), " +
                    $"  [Level] VARCHAR(16), " +
                    $"  [Message] NVARCHAR(MAX), " +
                    $"  [Exception] NVARCHAR(MAX), " +
                    $"  [InnerException] NVARCHAR(MAX), " +
                    $"  [ParentContextId] CHAR(36), " +
                    $"  [TopmostParentContextId] CHAR(36) NOT NULL " +
                    $") ",
                CommandType = System.Data.CommandType.Text,
                IgnoreFailures = false
            });
            target.UninstallDdlCommands.Add(new DatabaseCommandInfo { Text = $"DROP TABLE {schemaTableName}", CommandType = System.Data.CommandType.Text, IgnoreFailures = false });

            // Set paremeters
            target.AddColumn(d => d.ContextId, Layouts.ContextIdLayout);
            target.AddColumn(d => d.ContextName, Layouts.ContextNameLayout);
            target.AddColumn(d => d.Level, Layouts.LevelLayout);
            target.AddColumn(d => d.Message, Layouts.MessageLayout);
            target.AddColumn(d => d.Exception, Layouts.ExceptionLayout);
            target.AddColumn(d => d.ParentContextId, Layouts.ParentContextIdLayout);
            target.AddColumn(d => d.TopmostParentContextId, Layouts.TopmostParentContextIdLayout);
            target.RefreshInsertCommandText();
        }
    }
}
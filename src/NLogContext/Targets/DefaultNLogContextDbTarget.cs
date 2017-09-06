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
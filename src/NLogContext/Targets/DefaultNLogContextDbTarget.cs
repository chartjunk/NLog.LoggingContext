using NLog.Targets;

namespace NLogContext.Targets
{
    public class DefaultNLogContextDbTarget : NLogContextDbTarget<DefaultLogSchema>
    {
        public DefaultNLogContextDbTarget(string name, string schemaTableName) : base(name, schemaTableName)
        {

            // Set table creation and dropping commands
            InstallDdlCommands.Add(new DatabaseCommandInfo
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
            UninstallDdlCommands.Add(new DatabaseCommandInfo { Text = $"DROP TABLE {schemaTableName}", CommandType = System.Data.CommandType.Text, IgnoreFailures = false });

            // Set paremeters
            AddColumn(d => d.ContextId, Layouts.ContextIdLayout);
            AddColumn(d => d.ContextName, Layouts.ContextNameLayout);
            AddColumn(d => d.Level, Layouts.LevelLayout);
            AddColumn(d => d.Message, Layouts.MessageLayout);
            AddColumn(d => d.Exception, Layouts.ExceptionLayout);
            AddColumn(d => d.ParentContextId, Layouts.ParentContextIdLayout);
            AddColumn(d => d.TopmostParentContextId, Layouts.TopmostParentContextIdLayout);
            RefreshInsertCommandText();

            //CommandText = 
            //    $"INSERT INTO {schemaTableName} ([ContextId], [ContextName], [Level], [Message], [Exception], [ParentContextId], [TopmostParentContextId]) " +
            //    $"VALUES (@p_contextid, " +
            //    $"NULLIF(@p_contextname,''), " +
            //    $"NULLIF(@p_level,''), " +
            //    $"NULLIF(@p_message,''), " +
            //    $"NULLIF(@p_exception,''), " +
            //    $"NULLIF(@p_parentcontextid,''), " +
            //    $"NULLIF(@p_topmostparentcontextid,''))";
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_contextid", Layout = Layouts.ContextIdLayout });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_contextname", Layout = Layouts.ContextNameLayout });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_level", Layout = Layouts.LevelLayout });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_message", Layout = Layouts.MessageLayout });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_exception", Layout = Layouts.ExceptionLayout, });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_parentcontextid", Layout = Layouts.ParentContextIdLayout });
            //Parameters.Add(new DatabaseParameterInfo { Name = "p_topmostparentcontextid", Layout = Layouts.TopmostParentContextIdLayout });

        }
    }
}
using NLog.Targets;
using System.Collections.Generic;
using static NLogContext.Identifiers;

namespace NLogContext.Targets
{
    public class NLogContextDbTarget : DatabaseTarget
    {
        public NLogContextDbTarget(string name) : base(name) { }
        
        public NLogContextDbTarget(string name, string schemaTableName) : base(name)
        {
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
            CommandText = 
                $"INSERT INTO {schemaTableName} ([ContextId], [ContextName], [Level], [Message], [Exception], [ParentContextId], [TopmostParentContextId]) " +
                $"VALUES (@p_contextid, " +
                $"NULLIF(@p_contextname,''), " +
                $"NULLIF(@p_level,''), " +
                $"NULLIF(@p_message,''), " +
                $"NULLIF(@p_exception,''), " +
                $"NULLIF(@p_parentcontextid,''), " +
                $"NULLIF(@p_topmostparentcontextid,''))";
            Parameters.Add(new DatabaseParameterInfo { Name = "p_contextid", Layout = GetMdlcLayout(ContextIdIdentifier) });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_contextname", Layout = GetMdlcLayout(ContextNameIdentifier) });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_level", Layout = "${level}" });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_message", Layout = "${message}" });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_exception", Layout = "${exception}" });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_parentcontextid", Layout = GetMdlcLayout(ParentContextIdIdentifier) });
            Parameters.Add(new DatabaseParameterInfo { Name = "p_topmostparentcontextid", Layout = GetMdlcLayout(TopmostParentContextIdIdentifier) });
            CommandType = System.Data.CommandType.Text;
        }

        private static string GetMdlcLayout(string parameterName) => $"${{mdlc:{parameterName}}}";
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;

namespace NLogContext.IntegrationTest.SQLite
{
    class Access : IDisposable
    {
        public bool IsDisposed { get; private set; } = true;

        private readonly SQLiteConnection _connection;
        private readonly string _logSchemaTable;

        public Access(string connectionString, string logSchemaTable)
        {
            _connection = new SQLiteConnection(connectionString);
            _logSchemaTable = logSchemaTable;
            _connection.Open();
            IsDisposed = false;
            _connection.Disposed += (s, e) => IsDisposed = true;
        }

        public List<LogRow> GetLogRows() => _connection.Query<LogRow>($"SELECT * FROM {_logSchemaTable}").ToList();
        public void ClearLogRows()
        {
            using (var comm = _connection.CreateCommand())
            {
                comm.CommandType = System.Data.CommandType.Text;
                comm.CommandText = $"DELETE FROM {_logSchemaTable}";
                comm.ExecuteNonQuery();
            }
        }
        public void Dispose() => _connection.Close();
    }
}

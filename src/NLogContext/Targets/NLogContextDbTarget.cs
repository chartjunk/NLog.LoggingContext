using System;
using NLog.Targets;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NLog.Layouts;
using static NLogContext.Identifiers;

namespace NLogContext.Targets
{
    public class NLogContextDbTarget<TLogSchema> : DatabaseTarget
    {
        private class InsertParameterPair
        {
            public string InsertParamenterName { get; set; }
            public string TableColumnName { get; set; }
        }

        private readonly string _schemaTableName;
        private List<InsertParameterPair> InsertParameterPairs { get; } = new List<InsertParameterPair>();

        public NLogContextDbTarget(string name, string schemaTableName) : base(name)
        {
            _schemaTableName = schemaTableName;
            CommandType = System.Data.CommandType.Text;
        }

        public NLogContextDbTarget<TLogSchema> WithColumn<TColumn>(
            Expression<Func<TLogSchema, TColumn>> columnExpression,
            Layout layout,
            string tableColumnName = null) 
        {
            AddColumn(columnExpression, layout, tableColumnName);
            RefreshInsertCommandText();
            return this;
        }

        protected void AddColumn<TColumn>(
            Expression<Func<TLogSchema, TColumn>> columnExpression,
            Layout layout,
            string tableColumnName = null)
        {
            var propertyInfo = (columnExpression?.Body as MemberExpression)?.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Parameter not PropertyExpression", nameof(columnExpression));
            tableColumnName = tableColumnName ?? propertyInfo.Name;

            // Add a new parameter
            var insertParameterName = "p_nlogctx_" + propertyInfo.Name.ToLower();
            Parameters.Add(new DatabaseParameterInfo { Name = insertParameterName, Layout = layout });
            InsertParameterPairs.Add(new InsertParameterPair { InsertParamenterName = insertParameterName, TableColumnName = tableColumnName });
        }

        protected void RefreshInsertCommandText()
        {
            var columns = string.Join(",", InsertParameterPairs.Select(p => "[" + p.TableColumnName + "]"));
            var parameters = string.Join(",", InsertParameterPairs.Select(p => "NULLIF(@" + p.InsertParamenterName + ",'')"));
            var commandText = $"INSERT INTO {_schemaTableName} (" + columns + ") VALUES (" + parameters + ")";
            CommandText = commandText;
        }
    }
}

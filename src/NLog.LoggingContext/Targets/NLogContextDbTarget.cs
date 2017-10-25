﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.LoggingContext.Targets
{
    public class LoggingContextDbTarget<TLogSchema> : DatabaseTarget
    {
        public virtual string SchemaTableName { get; set; }

        private class InsertParameterPair
        {
            public string InsertParamenterName { get; set; }
            public string TableColumnName { get; set; }
        }

        private List<InsertParameterPair> InsertParameterPairs { get; } = new List<InsertParameterPair>();

        public LoggingContextDbTarget()
        {
            CommandType = System.Data.CommandType.Text;
        }

        public LoggingContextDbTarget<TLogSchema> SetColumn(
            Layout sourceLayout, string targetTableColumnName) 
        {
            AddColumn(sourceLayout, targetTableColumnName);
            RefreshInsertCommandText();
            return this;
        }

        public LoggingContextDbTarget<TLogSchema> SetColumn<TColumn>(
            Layout sourceLayout,
            Expression<Func<TLogSchema, TColumn>> targetTableColumnExpression)
        {
            AddColumn(sourceLayout, targetTableColumnExpression);
            RefreshInsertCommandText();
            return this;
        }

        internal void AddColumn(Layout sourceLayout, string targetTableColumnName)
        {
            var insertParameterName = "p_nlogctx_" + targetTableColumnName;
            Parameters.Add(new DatabaseParameterInfo { Name = insertParameterName, Layout = sourceLayout });
            InsertParameterPairs.Add(new InsertParameterPair { InsertParamenterName = insertParameterName, TableColumnName = targetTableColumnName });
        }

        internal void AddColumn<TColumn>(
            Layout sourceLayout,
            Expression<Func<TLogSchema, TColumn>> targetTableColumnExpression)
        {
            var propertyInfo = (targetTableColumnExpression?.Body as MemberExpression)?.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Parameter not PropertyExpression", nameof(targetTableColumnExpression));
            AddColumn(sourceLayout, propertyInfo.Name);
        }

        internal void RefreshInsertCommandText()
        {
            var columns = string.Join(",", InsertParameterPairs.Select(p => "[" + p.TableColumnName + "]"));
            var parameters = string.Join(",", InsertParameterPairs.Select(p => "NULLIF(@" + p.InsertParamenterName + ",'')"));
            var commandText = $"INSERT INTO {SchemaTableName} (" + columns + ") VALUES (" + parameters + ")";
            CommandText = commandText;
        }
    }
}

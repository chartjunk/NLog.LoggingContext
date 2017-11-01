using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.LoggingContext.Targets
{
    public class LoggingContextDbTarget<TLogSchema> : DatabaseTarget where TLogSchema : class
    {        
        public virtual string SchemaTableName { get; set; }

        public string SharedConfigSource { get; set; } = "ConfigurationManager";

        private class InsertParameterPair
        {
            public string InsertParamenterName { get; set; }
            public string TableColumnName { get; set; }
        }

        private List<InsertParameterPair> InsertParameterPairs { get; } = new List<InsertParameterPair>();

        public LoggingContextDbTarget()
        {
            CommandType = System.Data.CommandType.Text;
            if(SharedConfigSource == "ConfigurationManager")
                ConfigureSharedConfigurationUsingConfigurationManager();
        }

        public virtual void ConfigureSharedConfigurationUsingConfigurationManager()
        {
            var appSettings = new Internal.ConfigurationManager().AppSettings;
            var connectionStringKey = "NLog.LoggingContext:ConnectionString";
            var connectionStringNameKey = "NLog.LoggingContext:ConnectionStringName";
            var dbProviderKey = "NLog.LoggingContext:DbProvider";
            if (appSettings.AllKeys.Contains(connectionStringKey))
                ConnectionString = appSettings.Get(connectionStringKey);
            if (appSettings.AllKeys.Contains(connectionStringNameKey))
                ConnectionStringName = appSettings.Get(connectionStringNameKey);
            if (appSettings.AllKeys.Contains(dbProviderKey))
                DBProvider = appSettings.Get(dbProviderKey);
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
            var propertyInfo = ReflectionUtils.GetPropertyInfo(targetTableColumnExpression);
            AddColumnWithPropertyInfo<TColumn>(sourceLayout, propertyInfo);
        }

        internal void AddGdcColumn<TColumn>(Expression<Func<TLogSchema, TColumn>> targetTableColumnExpression)
        {
            var propertyInfo = ReflectionUtils.GetPropertyInfo(targetTableColumnExpression);
            var layout = Layouts.GetGdcLayout(propertyInfo.Name);
            AddColumnWithPropertyInfo<TColumn>(layout, propertyInfo);
        }

        internal void AddGdcColumn(string targetTableColumnName)
        {
            var layout = Layouts.GetGdcLayout(targetTableColumnName);
            AddColumn(layout, targetTableColumnName);
        }

        internal void AddColumnWithPropertyInfo<TColumn>(Layout sourceLayout, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentException("Parameter not PropertyExpression");
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

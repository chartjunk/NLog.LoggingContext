using System;
using System.Linq.Expressions;
using NLog.LoggingScope.WithSchemaExtension;

namespace NLog.LoggingScope
{
    public static class Extensions
    {
        public static LoggingScope WithSchema<TSchema>(
            this LoggingScope loggingScope,
            Action<WithSchemaSetter<TSchema>> setterAction) where TSchema : class
        {
            setterAction(new WithSchemaSetter<TSchema>(loggingScope));
            return loggingScope;
        }

        public static LoggingScope Set<TSchema, TValue>(
            this LoggingScope loggingScope,
            Expression<Func<TSchema, TValue>> propertyExpression, TValue value) where TSchema : class
        {
            new WithSchemaSetter<TSchema>(loggingScope).Set(propertyExpression, value);
            return loggingScope;
        }

        public static LoggingScope Set<TValue>(
            this LoggingScope loggingScope,
            string columnName, TValue value)
        {
            DiagnosticContextUtils.Gdc.SetGdcByShortKey(columnName, loggingScope.ScopeId, Convert.ToString(value));
            return loggingScope;
        }
    }
}
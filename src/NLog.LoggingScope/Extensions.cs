using System;
using System.Linq.Expressions;
using NLog.LoggingScope.WithSchemaExtension;

namespace NLog.LoggingScope
{
    public static class Extensions
    {
        public static LoggingScope WithSchema<TSchema>(
            this LoggingScope LoggingScope,
            Action<WithSchemaSetter<TSchema>> setterAction) where TSchema : class
        {
            setterAction(new WithSchemaSetter<TSchema>(LoggingScope));
            return LoggingScope;
        }

        public static LoggingScope Set<TSchema, TValue>(
            this LoggingScope LoggingScope,
            Expression<Func<TSchema, TValue>> propertyExpression, TValue value) where TSchema : class
        {
            new WithSchemaSetter<TSchema>(LoggingScope).Set(propertyExpression, value);
            return LoggingScope;
        }
    }
}
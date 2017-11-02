using System;
using System.Linq.Expressions;
using NLog.LoggingContext.WithSchemaExtension;

namespace NLog.LoggingContext
{
    public static class Extensions
    {
        public static LoggingContext WithSchema<TSchema>(
            this LoggingContext loggingContext,
            Action<WithSchemaSetter<TSchema>> setterAction) where TSchema : class
        {
            setterAction(new WithSchemaSetter<TSchema>(loggingContext));
            return loggingContext;
        }

        public static LoggingContext Set<TSchema, TValue>(
            this LoggingContext loggingContext,
            Expression<Func<TSchema, TValue>> propertyExpression, TValue value) where TSchema : class
        {
            new WithSchemaSetter<TSchema>(loggingContext).Set(propertyExpression, value);
            return loggingContext;
        }
    }
}
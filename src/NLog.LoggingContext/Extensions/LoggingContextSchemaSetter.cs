using System;
using System.Linq.Expressions;

namespace NLog.LoggingContext.Extensions
{
    public class LoggingContextSchemaSetter<TSchema> where TSchema: class
    {
        public LoggingContextSchemaSetter<TSchema> Set<TValue>(Expression<Func<TSchema, TValue>> propertyExpr,
            TValue value) => Set(propertyExpr, () => value);

        public LoggingContextSchemaSetter<TSchema> Set<TValue>(Expression<Func<TSchema, TValue>> propertyExpr,
            Func<TValue> valueFunc)
        {
            var propertyName = ReflectionUtils.GetPropertyInfo(propertyExpr).Name;

            return this;
        }

        internal LoggingContext ApplyOn(LoggingContext source)
        {
            throw new NotImplementedException();
            //return source;
        }
    }
}
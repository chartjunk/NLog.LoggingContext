using System;
using System.Linq.Expressions;

namespace NLog.LoggingScope.WithSchemaExtension
{
    public class WithSchemaSetter<TSchema> where TSchema : class
    {
        private readonly LoggingScope _loggingScope;

        public WithSchemaSetter(LoggingScope loggingScope)
        {
            _loggingScope = loggingScope;
        }

        public WithSchemaSetter<TSchema> Set<TValue>(Expression<Func<TSchema, TValue>> propertyExpression, TValue value)
        {
            var property = ReflectionUtils.GetPropertyInfo(propertyExpression);
            _loggingScope.Set(property.Name, Convert.ToString(value));
            return this;
        }
    }
}
using System;
using System.Linq.Expressions;

namespace NLog.LoggingContext.WithSchemaExtension
{
    public class WithSchemaSetter<TSchema> where TSchema : class
    {
        private readonly LoggingContext _loggingContext;

        public WithSchemaSetter(LoggingContext loggingContext)
        {
            _loggingContext = loggingContext;
        }

        public WithSchemaSetter<TSchema> Set<TValue>(Expression<Func<TSchema, TValue>> propertyExpression, TValue value)
        {
            var property = ReflectionUtils.GetPropertyInfo(propertyExpression);
            DiagnosticContextUtils.Gdc.SetGdcByShortKey(property.Name, _loggingContext.ContextId, Convert.ToString(value));
            return this;
        }
    }
}
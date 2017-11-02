using System;
using System.Linq.Expressions;

namespace NLog.LoggingScope.WithSchemaExtension
{
    public class WithSchemaSetter<TSchema> where TSchema : class
    {
        private readonly LoggingScope _LoggingScope;

        public WithSchemaSetter(LoggingScope LoggingScope)
        {
            _LoggingScope = LoggingScope;
        }

        public WithSchemaSetter<TSchema> Set<TValue>(Expression<Func<TSchema, TValue>> propertyExpression, TValue value)
        {
            var property = ReflectionUtils.GetPropertyInfo(propertyExpression);
            DiagnosticContextUtils.Gdc.SetGdcByShortKey(property.Name, _LoggingScope.ScopeId, Convert.ToString(value));
            return this;
        }
    }
}
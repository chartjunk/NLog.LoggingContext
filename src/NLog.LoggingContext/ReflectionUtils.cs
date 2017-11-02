using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NLog.LoggingScope
{
    internal static class ReflectionUtils
    {
        public static PropertyInfo GetPropertyInfo<TType, TValue>(Expression<Func<TType, TValue>> expr)
            where TType : class => (expr.Body as MemberExpression)?.Member as PropertyInfo;
    }
}
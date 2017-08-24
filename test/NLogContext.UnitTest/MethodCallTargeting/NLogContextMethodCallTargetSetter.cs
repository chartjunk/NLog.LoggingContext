using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static NLogContext.Layouts;

namespace NLogContext.UnitTest.MethodCallTargeting
{
    internal static class NLogContextMethodCallTargetSetter
    {
        public static void SetTarget<TClass>(string targetName, Expression<Action<TClass>> methodCall) where TClass : class
        {
            var method = (methodCall as MethodCallExpression).Method;

            var target = new MethodCallTarget(targetName)
            {
                ClassName = typeof(TClass).AssemblyQualifiedName,
                MethodName = method.Name,
            };

            var methodParameters = method.GetParameters().ToList().Select(p => p.Name).ToList();

            new MethodCallParameter[]
            {
                new MethodCallParameter("contextId", Layouts.ContextIdLayout),
                new MethodCallParameter("contextName", Layouts.ContextNameLayout),
                new MethodCallParameter("level", Layouts.LevelLayout),
                new MethodCallParameter("parentContextId", Layouts.ParentContextIdLayout)
            }
            .Join(methodParameters.Select((pName, ix) => new { pName, ix }), a => a.Name, b => b.pName, (a, b) => new
            {
                methodCallParameter = a,
                order = b.ix
            })
            .OrderBy(i => i.order).Select(i => i.methodCallParameter).ToList()
            .ForEach(target.Parameters.Add);

            var rule = new LoggingRule("*", LogLevel.Trace, target);
            LogManager.Configuration = new LoggingConfiguration();
            LogManager.Configuration.AddTarget(targetName, target);
            LogManager.Configuration.LoggingRules.Add(rule);
            LogManager.ReconfigExistingLoggers();
        }
    }
}

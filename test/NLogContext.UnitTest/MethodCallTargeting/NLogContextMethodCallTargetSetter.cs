using System;
using System.Linq;
using System.Linq.Expressions;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Joona.NLogContext.UnitTest.MethodCallTargeting
{
    internal static class NLogContextMethodCallTargetSetter
    {
        public static void SetTarget(string targetName, Expression<Action> staticMethodCall)
        {
            var method = (((LambdaExpression)staticMethodCall).Body as MethodCallExpression).Method;

            var target = new MethodCallTarget(targetName)
            {
                ClassName = method.DeclaringType.AssemblyQualifiedName,
                MethodName = method.Name,
            };

            var methodParameters = method.GetParameters().ToList().Select(p => p.Name).ToList();

            var methodCallParameters =
                new MethodCallParameter[]
                {
                    new MethodCallParameter("contextId", Layouts.ContextIdLayout),
                    new MethodCallParameter("contextName", Layouts.ContextNameLayout),
                    new MethodCallParameter("level", Layouts.LevelLayout),
                    new MethodCallParameter("parentContextId", Layouts.ParentContextIdLayout),
                    new MethodCallParameter("dateTime", Layouts.DateTimeLayout),
                    new MethodCallParameter("exception", Layouts.ExceptionLayout),
                    new MethodCallParameter("topmostParentContextId", Layouts.TopmostParentContextIdLayout),
                    new MethodCallParameter("message", Layouts.MessageLayout),
                }
                .Join(methodParameters.Select((pName, ix) => new { pName, ix }), a => a.Name, b => b.pName, (a, b) => new
                {
                    methodCallParameter = a,
                    order = b.ix
                })
                .OrderBy(i => i.order).Select(i => i.methodCallParameter).ToList();

            methodCallParameters
                .ForEach(target.Parameters.Add);

            var rule = new LoggingRule("*", LogLevel.Trace, target);
            LogManager.Configuration = new LoggingConfiguration();
            LogManager.Configuration.AddTarget(targetName, target);
            LogManager.Configuration.LoggingRules.Add(rule);
            LogManager.ReconfigExistingLoggers();
        }
    }
}

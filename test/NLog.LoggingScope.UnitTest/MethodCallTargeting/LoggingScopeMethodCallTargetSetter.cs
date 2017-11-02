using System;
using System.Linq;
using System.Linq.Expressions;
using NLog.Config;
using NLog.Targets;

namespace NLog.LoggingScope.UnitTest.MethodCallTargeting
{
    internal static class LoggingScopeMethodCallTargetSetter
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
                    new MethodCallParameter("scopeId", Layouts.ScopeIdLayout),
                    new MethodCallParameter("scopeName", Layouts.GetGdcLayout("ScopeName")),
                    new MethodCallParameter("level", Layouts.LevelLayout),
                    new MethodCallParameter("parentScopeId", Layouts.GetGdcLayout("ParentScopeId")),
                    new MethodCallParameter("dateTime", Layouts.DateTimeLayout),
                    new MethodCallParameter("exception", Layouts.ExceptionLayout),
                    new MethodCallParameter("topmostParentScopeId", Layouts.GetGdcLayout("TopmostParentScopeId")),
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

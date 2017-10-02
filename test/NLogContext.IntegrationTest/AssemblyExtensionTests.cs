using System;
using System.Linq;
using Joona.NLogContext.IntegrationTest.SQLite;
using Joona.NLogContext.Targets;
using Joona.NLogContext.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Config;

namespace Joona.NLogContext.IntegrationTest
{
    [TestClass]
    public class AssemblyExtensionTests
    {
        [TestMethod]
        public void AddTargetWithNLogConfigFile()
        {
            LogManager.Configuration = new XmlLoggingConfiguration(".\\NLogConfigs\\Extended.config");
            var target = LogManager.Configuration.AllTargets.Single() as DefaultNLogContextDbTarget;
            var connectionString = target.ConnectionString.Render(new LogEventInfo());
            using (var access = new Access(connectionString, target.SchemaTableName))
            {
                SQLite.NLogContextDbTargetSetter.InstallTarget(target);
                var testMsg = "Hello world!";
                ContextUtils.DoWithContext(logger => logger.Info(testMsg));
                var actualMsg = access.GetLogRows().Single().Message;
                Assert.AreEqual(testMsg, actualMsg);
            }
        }
    }
}

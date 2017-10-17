using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.Config;
using NLog.LoggingContext.IntegrationTest.SQLite;
using NLog.LoggingContext.Targets;
using NLog.LoggingContext.TestUtils;

namespace NLog.LoggingContext.IntegrationTest
{
    [TestClass]
    public class AssemblyExtensionTests
    {
        [TestMethod]
        public void AddTargetWithNLogConfigFile()
        {
            LogManager.Configuration = new XmlLoggingConfiguration(".\\NLogConfigs\\Extended.config");
            var target = LogManager.Configuration.AllTargets.Single() as DefaultLoggingContextDbTarget;
            var connectionString = target.ConnectionString.Render(new LogEventInfo());
            using (var access = new Access(connectionString, target.SchemaTableName))
            {
                SQLite.LoggingContextDbTargetSetter.InstallTarget(target);
                var testMsg = "Hello world!";
                ContextUtils.DoWithContext(logger => logger.Info(testMsg));
                var actualMsg = access.GetLogRows().Single().Message;
                Assert.AreEqual(testMsg, actualMsg);
            }
        }
    }
}

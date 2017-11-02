using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.Config;
using NLog.LoggingScope.IntegrationTest.SQLite;
using NLog.LoggingScope.Targets;
using NLog.LoggingScope.TestUtils;

namespace NLog.LoggingScope.IntegrationTest
{
    [TestClass]
    public class AssemblyExtensionTests
    {
        [TestMethod]
        public void AddTargetWithExtendedNLogConfigFile() => AddTargetWithNLogConfigFile(
            ".\\NLogConfigs\\Extended.config", true);

        [TestMethod]
        public void AddTargetWithExtendedNLogConfigFileWithSharedConfig() => AddTargetWithNLogConfigFile(
            ".\\NLogConfigs\\ExtendedWithSharedConfig.config", true);

        public void AddTargetWithNLogConfigFile(string file, bool getCsFromTarget)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(file);
            var target = LogManager.Configuration.AllTargets.Single() as DefaultLoggingScopeDbTarget;            
            var connectionString = target.ConnectionString.Render(new LogEventInfo());
            using (var access = new Access(connectionString, target.SchemaTableName))
            {
                SQLite.LoggingScopeDbTargetSetter.InstallTarget(target);
                var testMsg = "Hello world!";
                ScopeUtils.DoWithContext(logger => logger.Info(testMsg));
                var actualMsg = access.GetLogRows().Single().Message;
                Assert.AreEqual(testMsg, actualMsg);
            }
        }
    }
}

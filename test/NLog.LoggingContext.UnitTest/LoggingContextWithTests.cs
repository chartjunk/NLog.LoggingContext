using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LoggingContext.TestUtils;
using NLog.LoggingContext.UnitTest.MethodCallTargeting;

namespace NLog.LoggingContext.UnitTest
{
    [TestClass]
    public class LoggingContextWithTests : MockTargetSingletonTests
    {
        [TestMethod]
        public void TestLoggingContextWithBasic()
        {
            //// Assign
            //var userName = "joona";
            //var message = "Hello World!";

            //// Act
            //var contextName = ContextUtils.DoWithContext(logger => logger.Info(message),
            //    getNewContext: cn => new LoggingContext(cn).With<DefaultLogSchemaWithUsername>(c => c.Set(f => f.SchemaUsername, userName)));

            //// Assert
            //var rows = MockTargetSingleton.GetLogRows();
            //Assert.AreEqual(1, rows.Count());
            //Assert.AreEqual(userName, rows.Single().SchemaUsername);
        }
    }
}
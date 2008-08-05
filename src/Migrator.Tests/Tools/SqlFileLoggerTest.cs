using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Migrator.Framework.Loggers;
using NUnit.Framework;

namespace Migrator.Tests.Tools
{
    [TestFixture]
    public class SqlFileLoggerTest
    {
        public SqlScriptFileLogger _logger;
        public StringBuilder _sb;

        [SetUp]
        public void Setup()
        {
            _sb = new StringBuilder();
            _logger = new SqlScriptFileLogger(Logger.ConsoleLogger(), new StringWriter(_sb));
        }

        [Test]
        public void CanWriteSql()
        {
            _logger.ApplyingDBChange("some_change");
            Assert.AreEqual("some_change" + Environment.NewLine, _sb.ToString());
        }

        [Test]
        public void CanRunTheRest()
        {
            List<long> appliedVersions = new List<long>();
            appliedVersions.Add(1L);
            appliedVersions.Add(2L);
            appliedVersions.Add(3L);

            _logger.ApplyingDBChange("some_change");
            _logger.Log("log something");
            _logger.Warn("danger will");
            _logger.Trace("trace");
            _logger.Started(appliedVersions, 123L);
            _logger.MigrateUp(123L, "foo");
            _logger.MigrateDown(123L, "bar");
            _logger.Skipping(123L);
            _logger.RollingBack(123L);
            _logger.Exception(123L, "baz", new Exception());
            _logger.Finished(appliedVersions, 123L);

            Assert.AreEqual("some_change" + Environment.NewLine, _sb.ToString());
        }
    }
}
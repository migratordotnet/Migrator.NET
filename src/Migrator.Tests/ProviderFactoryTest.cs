using System;
using System.Configuration;
using Migrator.Framework;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ProviderFactoryTest
    {

        [Test]
        public void CanGetDialectsForProvider()
        {
            string[] providers = new string[] { "SqlServer", "Mysql", "SQLite", "PostgreSQL", "SqlServer2005" };
            Array.ForEach(providers,
                          delegate(string provider) { Assert.IsNotNull(ProviderFactory.DialectForProvider(provider)); });
            Assert.IsNull(ProviderFactory.DialectForProvider(null));
            Assert.IsNull(ProviderFactory.DialectForProvider(""));
            Assert.IsNull(ProviderFactory.DialectForProvider("foofoofoo"));
        }

        [Test, Category("SqlServer")]
        public void CanLoad_SqlServerProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SqlServer",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SqlServerConnectionString"]);
            Assert.IsNotNull(provider);
        }

        [Test, Category("MySql")]
        public void CanLoad_MySqlProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("MySql",
                                                                      ConfigurationManager.AppSettings[
                                                                          "MySqlConnectionString"]);
            Assert.IsNotNull(provider);
        }

        [Test, Category("SQLite")]
        public void CanLoad_SQLiteProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SQLite",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SQLiteConnectionString"]);
            Assert.IsNotNull(provider);
        }

        [Test, Category("PostgreSQL")]
        public void CanLoad_PostgreSQLProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("PostgreSQL",
                                                                      ConfigurationManager.AppSettings[
                                                                          "NpgsqlConnectionString"]);
            Assert.IsNotNull(provider);
        }
    }
}
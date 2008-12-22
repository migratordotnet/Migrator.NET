using System;
using System.Configuration;
using Migrator.Providers.Oracle;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("Oracle")]
    public class OracleTransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {
            string constr = ConfigurationManager.AppSettings["OracleConnectionString"];
            if (constr == null)
                throw new ArgumentNullException("OracleConnectionString", "No config file");
            _provider = new OracleTransformationProvider(new OracleDialect(), constr);
            _provider.BeginTransaction();

            AddDefaultTable();
        }
    }
}
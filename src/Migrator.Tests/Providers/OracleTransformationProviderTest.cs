using System;
using System.Configuration;
using System.Data;
using Migrator.Framework;
using Migrator.Providers.Oracle;
using Migrator.Tests.Providers;
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
			
            _provider.AddTable("TestTwo",
                               new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                               new Column("TestId", DbType.Int32, ColumnProperty.ForeignKey)
                );
        }
    }
}
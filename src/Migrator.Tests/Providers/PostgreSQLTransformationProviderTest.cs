using System;
using System.Configuration;
using System.Data;
using Migrator.Framework;
using Migrator.Providers.PostgreSQL;
using Migrator.Tests.Providers;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("Postgre")]
    public class PostgreSQLTransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {
            string constr = ConfigurationManager.AppSettings["NpgsqlConnectionString"];

            if (constr == null)
                throw new ArgumentNullException("ConnectionString", "No config file");

            _provider = new PostgreSQLTransformationProvider(new PostgreSQLDialect(), constr);
            _provider.BeginTransaction();
            
            _provider.AddTable("TestTwo",
                               new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                               new Column("TestId", DbType.Int32, ColumnProperty.ForeignKey)
            );
        }
    }
}
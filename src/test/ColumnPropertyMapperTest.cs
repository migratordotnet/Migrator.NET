using System.Data;
using Migrator.Framework;
using Migrator.Providers;
using Migrator.Providers.Oracle;
using Migrator.Providers.PostgreSQL;
using Migrator.Providers.SqlServer;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ColumnPropertyMapperTest
    {

        [Test]
        public void OracleCreatesSql()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new OracleDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, 0));
            Assert.AreEqual("foo varchar(30)", mapper.ColumnSql);
        }

        [Test]
        public void OracleCreatesNotNullSql()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new OracleDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, ColumnProperty.NotNull));
            Assert.AreEqual("foo varchar(30) NOT NULL", mapper.ColumnSql);
        }

        [Test]
        public void OracleIndexSqlIsNullWhenIndexedFalse()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new OracleDialect(), "char(1)");
            mapper.MapColumnProperties(new Column("foo", DbType.StringFixedLength, 1, 0));
            Assert.IsNull(mapper.IndexSql);
        }

        [Test]
        public void OracleIndexSqlIsNoNullWhenIndexed()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new OracleDialect(), "char(1)");
            mapper.MapColumnProperties(new Column("foo", DbType.StringFixedLength, 1, ColumnProperty.Indexed));
            Assert.IsNotNull(mapper.IndexSql);
        }

        [Test]
        public void PostgresIndexSqlIsNullWhenIndexedFalse()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new PostgreSQLDialect(), "char(1)");
            mapper.MapColumnProperties(new Column("foo", DbType.StringFixedLength, 1, 0));
            Assert.IsNull(mapper.IndexSql);
        }

        [Test]
        public void PostgresIndexSqlIsNoNullWhenIndexed()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new PostgreSQLDialect(), "char(1)");
            mapper.MapColumnProperties(new Column("foo", DbType.StringFixedLength, 1, ColumnProperty.Indexed));
            Assert.IsNotNull(mapper.IndexSql);
        }

        [Test]
        public void SqlServerIndexSqlIsNoNullWhenIndexed()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "char(1)");
            mapper.MapColumnProperties(new Column("foo", DbType.StringFixedLength, 1, ColumnProperty.Indexed));
            Assert.IsNull(mapper.IndexSql);
        }

        [Test]
        public void SqlServerCreatesSql()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, 0));
            Assert.AreEqual("foo varchar(30)", mapper.ColumnSql);
        }

        [Test]
        public void SqlServerCreatesNotNullSql()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, ColumnProperty.NotNull));
            Assert.AreEqual("foo varchar(30) NOT NULL", mapper.ColumnSql);
        }

        [Test]
        public void SqlServerCreatesSqWithDefault()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, 0, "'NEW'"));
            Assert.AreEqual("foo varchar(30) DEFAULT 'NEW'", mapper.ColumnSql);
        }

        [Test]
        public void SqlServerCreatesSqWithNullDefault()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "varchar(30)");
            mapper.MapColumnProperties(new Column("foo", DbType.String, 0, "NULL"));
            Assert.AreEqual("foo varchar(30) DEFAULT NULL", mapper.ColumnSql);
        }

        [Test]
        public void SqlServerCreatesSqWithBooleanDefault()
        {
            ColumnPropertiesMapper mapper = new ColumnPropertiesMapper(new SqlServerDialect(), "bit");
            mapper.MapColumnProperties(new Column("foo", DbType.Boolean, 0, false));
            Assert.AreEqual("foo bit DEFAULT 0", mapper.ColumnSql);

            mapper.MapColumnProperties(new Column("bar", DbType.Boolean, 0, true));
            Assert.AreEqual("bar bit DEFAULT 1", mapper.ColumnSql);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Migrator.Framework;
using Migrator.Providers;
using Migrator.Providers.Mysql;
using NUnit.Framework;

namespace Migrator.Tests.Dialects
{
	[TestFixture]
	public class MysqlDialectTest
	{
		private MysqlDialect _dialect;

		[SetUp]
		public void SetUp()
		{
			_dialect = new MysqlDialect();
		}


		[Test]
		public void Int32_is_unsigned_compatible()
		{
			//arange
			Column column = new Column("test", DbType.Int32, ColumnProperty.Unsigned);
			
			//act
			ColumnPropertiesMapper mapper = _dialect.GetAndMapColumnProperties(column);

			//assert
			StringAssert.Contains("UNSIGNED", mapper.ColumnSql);
		}

		
		[Test]
		public void Guid_is_not_unsigned_compatible()
		{
			//arrange
			Column column = new Column("test", DbType.Guid, ColumnProperty.Unsigned);

			//act
			ColumnPropertiesMapper mapper = _dialect.GetAndMapColumnProperties(column);

			//assert
			Assert.IsFalse(mapper.ColumnSql.Contains("UNSIGNED"));
		}

	}
}

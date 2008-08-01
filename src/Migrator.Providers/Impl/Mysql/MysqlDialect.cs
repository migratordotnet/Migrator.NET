
using System;
using System.Data;
using Migrator.Framework;

namespace Migrator.Providers.Mysql
{
	public class MysqlDialect : Dialect
	{
	    public MysqlDialect()
	    {
	        RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiStringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiString, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.Binary, "LONGBLOB");
			RegisterColumnType(DbType.Binary, 127, "TINYBLOB");
			RegisterColumnType(DbType.Binary, 65535, "BLOB");
			RegisterColumnType(DbType.Binary, 16777215, "MEDIUMBLOB");
			RegisterColumnType(DbType.Boolean, "TINYINT(1)");
			RegisterColumnType(DbType.Byte, "TINYINT UNSIGNED");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMERIC(19, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Guid, "VARCHAR(40)");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.StringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.String, "VARCHAR(255)");
			RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 65535, "TEXT");
			RegisterColumnType(DbType.String, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.Time, "TIME");
			
            RegisterProperty(ColumnProperty.Unsigned, "UNSIGNED");
            RegisterProperty(ColumnProperty.Identity, "AUTO_INCREMENT");
        }

        public override Type TransformationProvider { get { return typeof(MySqlTransformationProvider); } }

        public override string QuoteTemplate
        {
            get { return "`{0}`"; }
        }

        public override string Default(object defaultValue)
        {
            if (defaultValue.GetType().Equals(typeof (bool)))
            {
                defaultValue = ((bool) defaultValue) ? 1 : 0;
            }
            return String.Format("DEFAULT {0}", defaultValue);
        }
    }
}
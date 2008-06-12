
using System;
using System.Data;
using Migrator.Framework;

namespace Migrator.Providers.PostgreSQL
{
	public class PostgreSQLDialect : Dialect
	{
	    public PostgreSQLDialect()
	    {
	        RegisterColumnType(DbType.AnsiStringFixedLength, "char(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "char($l)");
            RegisterColumnType(DbType.AnsiString, "varchar(255)");
            RegisterColumnType(DbType.AnsiString, 8000, "varchar($l)");
            RegisterColumnType(DbType.AnsiString, 2147483647, "text");
            RegisterColumnType(DbType.Binary, "bytea");
            RegisterColumnType(DbType.Binary, 2147483647, "bytea");
            RegisterColumnType(DbType.Boolean, "boolean");
            RegisterColumnType(DbType.Byte, "int2");
            RegisterColumnType(DbType.Currency, "decimal(16,4)");
            RegisterColumnType(DbType.Date, "date");
            RegisterColumnType(DbType.DateTime, "timestamp");
            RegisterColumnType(DbType.Decimal, "decimal(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "decimal(18, $l)");
            RegisterColumnType(DbType.Double, "float8");
            RegisterColumnType(DbType.Int16, "int2");
            RegisterColumnType(DbType.Int32, "int4");
            RegisterColumnType(DbType.Int64, "int8");
            RegisterColumnType(DbType.Single, "float4");
            RegisterColumnType(DbType.StringFixedLength, "char(255)");
            RegisterColumnType(DbType.StringFixedLength, 4000, "char($l)");
            RegisterColumnType(DbType.String, "varchar(255)");
            RegisterColumnType(DbType.String, 4000, "varchar($l)");
            RegisterColumnType(DbType.String, 1073741823, "text");
            RegisterColumnType(DbType.Time, "time");
            
            RegisterProperty(ColumnProperty.Identity, "serial");
        }

        public override Type TransformationProvider { get { return typeof(PostgreSQLTransformationProvider); } }
        
        public override bool TableNameNeedsQuote
        {
            get { return false; }
        }
        
        public override bool ConstraintNameNeedsQuote
        {
            get { return false; }
        }
        
        public override bool IdentityNeedsType
        {
            get { return false; }
        }
    }
}
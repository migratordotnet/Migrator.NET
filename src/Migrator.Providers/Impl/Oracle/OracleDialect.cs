
using System;
using System.Data;
using Migrator.Framework;

namespace Migrator.Providers.Oracle
{
	public class OracleDialect : Dialect
	{
	    public OracleDialect()
	    {
	        RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 2000, "CHAR($l)");
            RegisterColumnType(DbType.AnsiString, "VARCHAR2(255)");
            RegisterColumnType(DbType.AnsiString, 2000, "VARCHAR2($l)");
            RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB"); // should use the IType.ClobType
            RegisterColumnType(DbType.Binary, "RAW(2000)");
            RegisterColumnType(DbType.Binary, 2000, "RAW($l)");
            RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
            RegisterColumnType(DbType.Boolean, "NUMBER(1,0)");
            RegisterColumnType(DbType.Byte, "NUMBER(3,0)");
            RegisterColumnType(DbType.Currency, "NUMBER(19,1)");
            RegisterColumnType(DbType.Date, "DATE");
            RegisterColumnType(DbType.DateTime, "TIMESTAMP(4)");
            RegisterColumnType(DbType.Decimal, "NUMBER(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "NUMBER(19, $l)");
            // having problems with both ODP and OracleClient from MS not being able
            // to read values out of a field that is DOUBLE PRECISION
            RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //"FLOAT(53)" );
            //RegisterColumnType(DbType.Guid, "CHAR(38)");
            RegisterColumnType(DbType.Int16, "NUMBER(5,0)");
            RegisterColumnType(DbType.Int32, "NUMBER(10,0)");
            RegisterColumnType(DbType.Int64, "NUMBER(20,0)");
            RegisterColumnType(DbType.Single, "FLOAT(24)");
            RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, 2000, "NCHAR($l)");
            RegisterColumnType(DbType.String, "NVARCHAR2(255)");
            RegisterColumnType(DbType.String, 2000, "NVARCHAR2($l)");
            RegisterColumnType(DbType.String, 1073741823, "NCLOB");
            RegisterColumnType(DbType.Time, "DATE");
	        RegisterProperty(ColumnProperty.Null, String.Empty);
        }

        public override Type TransformationProvider { get { return typeof(OracleTransformationProvider); } }

    }
}
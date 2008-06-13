using System;
using System.Data;

namespace Migrator.Providers.SqlServer
{
    public class SqlServerCeDialect : SqlServerDialect
    {
		public SqlServerCeDialect()
        {
			RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR($l)");
			RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 1073741823, "TEXT");
        }

        public override Type TransformationProvider { get { return typeof (SqlServerCeTransformationProvider); } }

    }
}

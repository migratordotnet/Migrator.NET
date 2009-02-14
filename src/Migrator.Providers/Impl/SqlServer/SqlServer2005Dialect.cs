using System;
using System.Data;

namespace Migrator.Providers.SqlServer
{
    public class SqlServer2005Dialect : SqlServerDialect
    {
        public SqlServer2005Dialect() :base()
        {
            RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
            RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
            RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
            RegisterColumnType(DbType.Xml, "XML");
        }

        public override Type TransformationProvider { get { return typeof (SqlServerTransformationProvider); } }

    }
}

using System.Data;
using Migrator.Framework;

namespace Migrator.Providers.SqlServer
{
    public class SqlServer2005Dialect : SqlServerDialect
    {
        public SqlServer2005Dialect()
        {
            RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
            RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
            RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
            RegisterColumnType(DbType.Xml, "XML");
        }

		public override ITransformationProvider GetTransformationProvider(Dialect dialect, string connectionString)
		{
			return new SqlServerTransformationProvider(dialect, connectionString);
		}
    }
}

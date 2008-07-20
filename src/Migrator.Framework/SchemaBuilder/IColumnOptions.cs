using System.Data;
using Migrator.Framework.SchemaBuilder;

namespace Migrator.Framework.SchemaBuilder
{
	public interface IColumnOptions
	{
		SchemaBuilder OfType(DbType dbType);

		SchemaBuilder WithSize(int size);

		IForeignKeyOptions AsForeignKey();
	}
}
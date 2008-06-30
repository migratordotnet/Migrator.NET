using System.Data;

namespace Migrator.Framework
{
	public interface IColumn
	{
		ColumnProperty ColumnProperty { get; set; }

		string Name { get; set; }

		DbType Type { get; set; }

		int Size { get; set; }

		bool IsIdentity { get; }

		bool IsPrimaryKey { get; }

		object DefaultValue { get; set; }
	}

	public interface IFluentColumn : IColumn
	{
		ForeignKeyConstraint Constraint { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	class PostgreSQLColumnPropertiesMapper: ColumnPropertiesMapper
	{

		public PostgreSQLColumnPropertiesMapper(string type)
			: base(type)
		{
		}

		public override string IndexSql
		{
			get
			{
				if (indexed)
					return string.Format("INDEX(`{0}`)", name.Trim('`'));
				return null;
			}
		}

		public override void NotNull()
		{
			sqlNull = "NOT NULL";
		}

		public override void PrimaryKey()
		{
			sqlPrimaryKey = "PRIMARY KEY";
		}

		public override void Unique()
		{
			sqlUnique = "UNIQUE";
		}

		public override void Unsigned()
		{
			sqlUnsigned = string.Empty;
		}

		public override void Identity()
		{
		}

		public override void Default(string defaultValue)
		{
			sqlDefault = string.Format("DEFAULT={0}", defaultValue);
		}
	}
}

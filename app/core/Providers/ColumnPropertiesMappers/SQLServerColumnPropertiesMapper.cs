using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	class SQLServerColumnPropertiesMapper: ColumnPropertiesMapper
	{

		public SQLServerColumnPropertiesMapper(string type)
			: base(type)
		{
		}

		public override string IndexSql
		{
			get 
			{
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
			sqlUnsigned = "";
		}

		public override void Identity()
		{
			sqlIdentity = "IDENTITY";
		}

		public override void Default(object defaultValue)
		{
			sqlDefault = string.Format("DEFAULT={0}", defaultValue);
		}

		public override string Quote(string value)
		{
			return string.Format("[{0]]", value);
		}
	}
}

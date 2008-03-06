using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	public class MysqlColumnPropertiesMapper : ColumnPropertiesMapper
	{
		public MysqlColumnPropertiesMapper(string type)
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
			sqlUnsigned = "UNSIGNED";
		}

		public override void Identity()
		{
			sqlIdentity = "AUTO_INCREMENT";
		}

		public override void Default(object defaultValue)
		{
			if (defaultValue.GetType().Equals(typeof(bool)))
			{
				defaultValue = ((bool)defaultValue) ? 1 : 0;
			}
			sqlDefault = string.Format("DEFAULT '{0}'", defaultValue.ToString());
		}

		public override string Quote(string value)
		{
			return string.Format("`{0}`", value);
		}

	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
    class OracleColumnPropertiesMapper: ColumnPropertiesMapper
	{

        public OracleColumnPropertiesMapper(string type)
			: base(type)
		{
            sqlNull = string.Empty;
		}

		public override string IndexSql
		{
			get
			{
				if (indexed)
					return string.Format("INDEX({0})", name.Trim('`'));
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
		    sqlIdentity = string.Empty;
		}

		public override void Default(object defaultValue)
		{
			sqlDefault = string.Format("DEFAULT {0}", defaultValue);
		}

        public override string ColumnSql
        {
            get
            {
                return String.Join(" ", new string[] { name, type, sqlUnsigned, sqlDefault, sqlNull, sqlIdentity, sqlUnique, sqlPrimaryKey });
            }
        }

		public override string Quote(string value)
		{
			return string.Format("\"{0}\"", value);
		}
	}
}

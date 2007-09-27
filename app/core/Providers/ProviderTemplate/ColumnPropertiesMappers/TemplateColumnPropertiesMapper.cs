using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	class TemplateColumnPropertiesMapper: ColumnPropertiesMapper
	{

		public TemplateColumnPropertiesMapper(string type)
			: base(type)
		{
		}

		public override string IndexSql
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void NotNull()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void PrimaryKey()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Unique()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Unsigned()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Identity()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Default(string defaultValue)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

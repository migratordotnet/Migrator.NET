using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;
using Migrator.Providers.TypeToSqlProviders;

namespace Migrator.Columns
{
	public class DateColumn: Column
	{
		public DateColumn(string name)
			:base(name, typeof(DateTime))
		{
		}

		public override IColumnPropertiesMapper TypeHint(ITypeToSqlProvider provider)
		{
			return provider.Date;
		}
	}
}

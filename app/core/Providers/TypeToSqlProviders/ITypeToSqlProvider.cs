using System;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
	public interface ITypeToSqlProvider
	{
		IColumnPropertiesMapper PrimaryKey { get;}

		IColumnPropertiesMapper Char(byte size);
		IColumnPropertiesMapper String(ushort size);
		IColumnPropertiesMapper Text {get;}
		IColumnPropertiesMapper LongText { get;}

		IColumnPropertiesMapper Integer { get;}
		IColumnPropertiesMapper Long { get;}

		IColumnPropertiesMapper Float { get;}
		IColumnPropertiesMapper Double { get;}
		IColumnPropertiesMapper Decimal(int whole);
		IColumnPropertiesMapper Decimal(int whole, int part);

		IColumnPropertiesMapper Bool { get;}
		IColumnPropertiesMapper DateTime { get;}

		IColumnPropertiesMapper Binary(byte size);
		IColumnPropertiesMapper Blob { get;}
		IColumnPropertiesMapper LongBlob { get;}
	}
}
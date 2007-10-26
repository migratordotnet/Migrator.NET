using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
	public class SQLServerTypeToSqlProvider: ITypeToSqlProvider
	{

		#region ITypeToSqlProvider Members

		public IColumnPropertiesMapper PrimaryKey
		{
            get { return Integer; }
		}

		public IColumnPropertiesMapper Char(byte size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("nchar({0})", size));
		}

		public IColumnPropertiesMapper String(ushort size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("nvarchar({0})", size));
		}

		public IColumnPropertiesMapper Text
		{
			get { return new SQLServerColumnPropertiesMapper("ntext"); }
		}

		public IColumnPropertiesMapper LongText
		{
			get { return new SQLServerColumnPropertiesMapper("nvarchar(max)"); }
		}

		public IColumnPropertiesMapper Binary(byte size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("VARBINARY({0})", size));
		}

		public IColumnPropertiesMapper Blob
		{
			get { return new SQLServerColumnPropertiesMapper("image"); }
		}

		public IColumnPropertiesMapper LongBlob
		{
			get { return new SQLServerColumnPropertiesMapper("image"); }
		}

		public IColumnPropertiesMapper Integer
		{
			get { return new SQLServerColumnPropertiesMapper("int"); }
		}

		public IColumnPropertiesMapper Long
		{
			get { return new SQLServerColumnPropertiesMapper("bigint"); }
		}

		public IColumnPropertiesMapper Float
		{
			get { return new SQLServerColumnPropertiesMapper("real"); }
		}

		public IColumnPropertiesMapper Double
		{
			get { return new SQLServerColumnPropertiesMapper("float"); }
		}

		public IColumnPropertiesMapper Decimal(int whole)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("numeric({0})", whole));
		}

		public IColumnPropertiesMapper Decimal(int whole, int part)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("numeric({0}, {1})", whole, part));
		}

		public IColumnPropertiesMapper Bool
		{
			get
			{
				IColumnPropertiesMapper mapper = new SQLServerColumnPropertiesMapper("bit");
				mapper.Default("0");
				return mapper;
			}
		}

		public IColumnPropertiesMapper DateTime
		{
			get { return new SQLServerColumnPropertiesMapper("datetime"); }
		}

		#endregion

	}
}

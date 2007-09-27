using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
	public class TemplateTypeToSqlProvider: ITypeToSqlProvider
	{

		#region ITypeToSqlProvider Members

		public IColumnPropertiesMapper Char(byte size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("char({0})", size));
		}

		public IColumnPropertiesMapper String(ushort size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("varchar({0})", size));
		}

		public IColumnPropertiesMapper Text
		{
			get { return new SQLServerColumnPropertiesMapper("TEXT"); }
		}

		public IColumnPropertiesMapper LongText
		{
			get { return new SQLServerColumnPropertiesMapper("LONGTEXT"); }
		}

		public IColumnPropertiesMapper Binary(byte size)
		{
			return new SQLServerColumnPropertiesMapper(string.Format("VARBINARY({0})", size));
		}

		public IColumnPropertiesMapper Blob
		{
			get { return new SQLServerColumnPropertiesMapper("BLOB"); }
		}

		public IColumnPropertiesMapper LongBlob
		{
			get { return new SQLServerColumnPropertiesMapper("LONGBLOB"); }
		}

		public IColumnPropertiesMapper Integer
		{
			get { return new SQLServerColumnPropertiesMapper("INT4"); }
		}

		public IColumnPropertiesMapper Long
		{
			get { return new SQLServerColumnPropertiesMapper("INT8"); }
		}

		public IColumnPropertiesMapper Float
		{
			get { return new SQLServerColumnPropertiesMapper("FLOAT4"); }
		}

		public IColumnPropertiesMapper Double
		{
			get { return new SQLServerColumnPropertiesMapper("FLOAT8"); }
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
				IColumnPropertiesMapper mapper = new SQLServerColumnPropertiesMapper("TINYINT(1)");
				mapper.Default("0");
				return mapper;
			}
		}

		public IColumnPropertiesMapper DateTime
		{
			get { return new SQLServerColumnPropertiesMapper("DATETIME"); }
		}

		#endregion
	}
}
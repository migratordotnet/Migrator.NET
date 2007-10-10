using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
	public class MysqlTypeToSqlProvider: ITypeToSqlProvider
	{

		#region ITypeToSqlProvider Members

		public IColumnPropertiesMapper PrimaryKey
		{
			get { return Integer; }
		}

		public IColumnPropertiesMapper Char(byte size)
		{
			return new MysqlColumnPropertiesMapper(string.Format("char({0})", size));
		}

		public IColumnPropertiesMapper String(ushort size)
		{
			return new MysqlColumnPropertiesMapper(string.Format("varchar({0})", size));
		}

		public IColumnPropertiesMapper Text
		{
			get { return new MysqlColumnPropertiesMapper("TEXT"); }
		}

		public IColumnPropertiesMapper LongText
		{
			get { return new MysqlColumnPropertiesMapper("LONGTEXT"); }
		}

		public IColumnPropertiesMapper Binary(byte size)
		{
			return new MysqlColumnPropertiesMapper(string.Format("VARBINARY({0})", size));
		}

		public IColumnPropertiesMapper Blob
		{
			get { return new MysqlColumnPropertiesMapper("BLOB"); }
		}

		public IColumnPropertiesMapper LongBlob
		{
			get { return new MysqlColumnPropertiesMapper("LONGBLOB"); }
		}

		public IColumnPropertiesMapper Integer
		{
			get { return new MysqlColumnPropertiesMapper("INT4"); }
		}

		public IColumnPropertiesMapper Long
		{
			get { return new MysqlColumnPropertiesMapper("INT8"); }
		}

		public IColumnPropertiesMapper Float
		{
			get { return new MysqlColumnPropertiesMapper("FLOAT4"); }
		}

		public IColumnPropertiesMapper Double
		{
			get { return new MysqlColumnPropertiesMapper("FLOAT8"); }
		}

		public IColumnPropertiesMapper Decimal(int whole)
		{
			return new MysqlColumnPropertiesMapper(string.Format("numeric({0})", whole));
		}

		public IColumnPropertiesMapper Decimal(int whole, int part)
		{
			// whole must be greater than part in mysql, not sure why.
			// FIXME: perhaps this should throw an exception, unless that would affect portability
			// FIXME: will this cause an error for large values of part (i.e. the datatype is not big enough), not sure
			return new MysqlColumnPropertiesMapper(string.Format("numeric({0}, {1})", (whole >= part ? whole : part), part));
		}

		public IColumnPropertiesMapper Bool
		{
			get
			{
				IColumnPropertiesMapper mapper = new MysqlColumnPropertiesMapper("TINYINT(1)");
				mapper.Default("0");
				return mapper;
			}
		}

		public IColumnPropertiesMapper DateTime
		{
			get { return new MysqlColumnPropertiesMapper("DATETIME"); }
		}

		#endregion
	}
}
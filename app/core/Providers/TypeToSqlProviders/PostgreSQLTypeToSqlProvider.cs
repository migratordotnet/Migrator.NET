using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
	public class PostgreSQLTypeToSqlProvider: ITypeToSqlProvider
	{

		#region ITypeToSqlProvider Members

		public IColumnPropertiesMapper PrimaryKey
		{
			get 
			{
				return new PostgreSQLColumnPropertiesMapper("serial");
			}
		}

		public IColumnPropertiesMapper Char(byte size)
		{
			return new PostgreSQLColumnPropertiesMapper(string.Format("char({0})", size));
		}

		public IColumnPropertiesMapper String(ushort size)
		{
			return new PostgreSQLColumnPropertiesMapper(string.Format("varchar({0})", size));
		}

		public IColumnPropertiesMapper Text
		{
			get { return new PostgreSQLColumnPropertiesMapper("text"); }
		}

		public IColumnPropertiesMapper LongText
		{
			get { return Text; }
		}

		public IColumnPropertiesMapper Binary(byte size)
		{
			return Blob;
			//return new PostgreSQLColumnPropertiesMapper(string.Format("BLOB", size));
		}

		public IColumnPropertiesMapper Blob
		{
			get { return new PostgreSQLColumnPropertiesMapper("blob"); }
		}

		public IColumnPropertiesMapper LongBlob
		{
			get { return Blob; }
		}

		public IColumnPropertiesMapper Integer
		{
			get { return new PostgreSQLColumnPropertiesMapper("int4"); }
		}

		public IColumnPropertiesMapper Long
		{
			get { return new PostgreSQLColumnPropertiesMapper("int8"); }
		}

		public IColumnPropertiesMapper Float
		{
			get { return new PostgreSQLColumnPropertiesMapper("float4"); }
		}

		public IColumnPropertiesMapper Double
		{
			get { return new PostgreSQLColumnPropertiesMapper("float8"); }
		}

		public IColumnPropertiesMapper Decimal(int whole)
		{
			return new PostgreSQLColumnPropertiesMapper(string.Format("numeric({0})", whole));
		}

		public IColumnPropertiesMapper Decimal(int whole, int part)
		{
			return new PostgreSQLColumnPropertiesMapper(string.Format("numeric({0}, {1})", whole, part));
		}

		public IColumnPropertiesMapper Bool
		{
			get
			{
				IColumnPropertiesMapper mapper = new PostgreSQLColumnPropertiesMapper("boolean");
				mapper.Default("0");
				return mapper;
			}
		}

		public IColumnPropertiesMapper DateTime
		{
			get { return new PostgreSQLColumnPropertiesMapper("timestamp"); }
		}

		#endregion

	}
}
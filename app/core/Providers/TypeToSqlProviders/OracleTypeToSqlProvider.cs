using System;
using System.Collections.Generic;
using System.Text;
using Migrator.Providers.ColumnPropertiesMappers;

namespace Migrator.Providers.TypeToSqlProviders
{
    public class OracleTypeToSqlProvider : ITypeToSqlProvider
    {

        #region ITypeToSqlProvider Members

        public IColumnPropertiesMapper PrimaryKey
        {
            get { return Long; }
        }

        public IColumnPropertiesMapper Char(byte size)
        {
            return new OracleColumnPropertiesMapper(string.Format("nchar({0})", size));
        }

        public IColumnPropertiesMapper String(ushort size)
        {
            return new OracleColumnPropertiesMapper(string.Format("nvarchar2({0})", size));
        }

        public IColumnPropertiesMapper Text
        {
            get { return new OracleColumnPropertiesMapper("nclob"); }
        }

        public IColumnPropertiesMapper LongText
        {
            get { return Text; }
        }

        public IColumnPropertiesMapper Binary(byte size)
        {
            return Blob;
        }

        public IColumnPropertiesMapper Blob
        {
            get { return new OracleColumnPropertiesMapper("BLOB"); }
        }

        public IColumnPropertiesMapper LongBlob
        {
            get { return Blob; }
        }

        public IColumnPropertiesMapper Integer
        {
            get { return new OracleColumnPropertiesMapper("number(10)"); }
        }

        public IColumnPropertiesMapper Long
        {
            get { return new OracleColumnPropertiesMapper("number(20)"); }
        }

        public IColumnPropertiesMapper Float
        {
            get { return new OracleColumnPropertiesMapper("number(19,5)"); }
        }

        public IColumnPropertiesMapper Double
        {
            get { return Float; }
        }

        public IColumnPropertiesMapper Decimal(int whole)
        {
            return new OracleColumnPropertiesMapper(string.Format("number({0})", whole));
        }

        public IColumnPropertiesMapper Decimal(int whole, int part)
        {
            return new OracleColumnPropertiesMapper(string.Format("number({0}, {1})", whole, part));
        }

        public IColumnPropertiesMapper Bool
        {
            get
            {
                IColumnPropertiesMapper mapper = new OracleColumnPropertiesMapper("number(1)");
                mapper.Default("0");
                return mapper;
            }
        }

        public IColumnPropertiesMapper DateTime
        {
            get { return new OracleColumnPropertiesMapper("timestamp"); }
        }

		public IColumnPropertiesMapper Date
		{
			get { return new OracleColumnPropertiesMapper("date"); }
		}

		#endregion
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Migrator.Providers.ForeignKeys;
using Migrator.Providers.TypeToSqlProviders;
using Oracle.DataAccess.Client;
using ForeignKeyConstraint=Migrator.Providers.ForeignKeys.ForeignKeyConstraint;

namespace Migrator.Providers
{
    public class OracleTransformationProvider: TransformationProvider
	{
		private string _connectionString;

        public OracleTransformationProvider(string connectionString)
		{
			_connectionString = connectionString;
		    _connection = new OracleConnection();
			_connection.ConnectionString = _connectionString;
			_connection.Open();
		}

		public override void RemoveTable(string name)
		{
			if (TableExists(name))
			{
				string sql = string.Format("DROP TABLE {0}", name.ToLower());
				Logger.Log(sql);
				ExecuteNonQuery(sql);
			}
		}

        public override void AddColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(string.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
        }

		public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint)
		{
			if (ConstraintExists(name, primaryTable))
			{
				Logger.Warn("Constraint {0} already exists", name);
				return;
			}
			string sql = string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4})",
									   primaryTable.ToLower(), name.ToLower(), string.Join(",", primaryColumns),
									   refTable.ToLower(), string.Join(",", refColumns));
			Logger.Log(sql);
			ExecuteNonQuery(sql);
		}

		public override void RemoveForeignKey(string name, string table)
		{
			if (TableExists(table) && ConstraintExists(name, table))
			{
				string sql = string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", table.ToLower(), name.ToLower());
				Logger.Log(sql);
				ExecuteNonQuery(sql);
			}
		}

		public override bool ConstraintExists(string name, string table)
		{
			string sql = string.Format("SELECT COUNT(constraint_name) FROM user_constraints WHERE lower(constraint_name) = '{0}' AND lower(table_name) = '{1}'", name.ToLower(), table.ToLower());
			Logger.Log(sql);
			object scalar = ExecuteScalar(sql);
			return Convert.ToInt32(scalar) == 1;
		}

		public override bool ColumnExists(string table, string column)
		{
			if (!TableExists(table))
				return false;

			string sql = string.Format("SELECT COUNT(column_name) FROM user_tab_columns WHERE lower(table_name) = '{0}' AND lower(column_name) = '{1}'",
										table.ToLower(), column.ToLower());
			Logger.Log(sql);
			object scalar = ExecuteScalar(sql);
			return Convert.ToInt32(scalar) == 1;
		}

		public override bool TableExists(string table)
		{
			string sql = string.Format("SELECT COUNT(table_name) FROM user_tables WHERE lower(table_name) = '{0}'",
									   table.ToLower());
			Logger.Log(sql);
			object count = ExecuteScalar(sql);
			return Convert.ToInt32(count) == 1;
		}
		
		public override string[] GetTables()
		{
            List<string> tables = new List<string>();
	
			using (IDataReader reader =
				ExecuteQuery("SELECT table_name FROM user_tables"))
			{
				while (reader.Read())
				{
					tables.Add(reader[0].ToString());
				}
			}
		
			return tables.ToArray();
		}
		
		public override Column[] GetColumns(string table)
		{
            List<Column> columns = new List<Column>();


            using (IDataReader reader = ExecuteQuery(string.Format("select column_name, data_type, data_length, data_precision, data_scale FROM USER_TAB_COLUMNS WHERE lower(table_name) = '{0}'", table)))
            {
                while (reader.Read())
                {
                    string colName = reader[0].ToString();
                    Type colType = typeof (string);
                    string dataType = reader[1].ToString().ToLower();
                    if (dataType.Equals("number"))
                    {
                        int precision = reader.GetInt32(3);
                        int scale = reader.GetInt32(4);
                        if (scale == 0)
                        {
                            colType = precision <= 10 ? typeof (int) : typeof(long);
                        }
                        else
                        {
                            colType = typeof (decimal);
                        }
                    }
                    else if (dataType.StartsWith("timestamp") || dataType.Equals("date"))
                    {
                        colType = typeof (DateTime);
                    }
                    columns.Add(new Column(colName, colType));
                }
            }

            return columns.ToArray();
		}

		public override void AddTable(string name, string columns)
		{
			string sqlCreate = string.Format("CREATE TABLE {0} ({1})", name, columns);
			ExecuteNonQuery(sqlCreate);
		}

		public override IForeignKeyConstraintMapper ForeignKeyMapper
		{
			get 
			{
				return new ForeignKeys.OracleForeignKeyConstraintMapper();
			}
		}

		public override ITypeToSqlProvider TypeToSqlProvider
		{
			get { return new OracleTypeToSqlProvider(); }
		}

		public override ProviderType ProviderType
		{
			get { return ProviderType.Oracle; }
		}
	}
}

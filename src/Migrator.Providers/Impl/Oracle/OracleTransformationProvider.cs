using System;
using System.Collections.Generic;
using System.Data;
using Migrator.Framework;
using Oracle.DataAccess.Client;

namespace Migrator.Providers.Oracle
{
    public class OracleTransformationProvider : TransformationProvider
    {
        public OracleTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString)
        {
            _connection = new OracleConnection();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }

        public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable,
                                          string[] refColumns, Migrator.Framework.ForeignKeyConstraint constraint)
        {
            if (ConstraintExists(primaryTable, name))
            {
                Logger.Warn("Constraint {0} already exists", name);
                return;
            }

            ExecuteNonQuery(
                String.Format(
                    "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4})",
                    primaryTable, name, String.Join(",", primaryColumns),
                    refTable, String.Join(",", refColumns)));
        }

        public override void AddColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(String.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
        }

        public override bool ConstraintExists(string table, string name)
        {
            string sql =
                string.Format(
                    "SELECT COUNT(constraint_name) FROM user_constraints WHERE lower(constraint_name) = '{0}' AND lower(table_name) = '{1}'",
                    name.ToLower(), table.ToLower());
            Logger.Log(sql);
            object scalar = ExecuteScalar(sql);
            return Convert.ToInt32(scalar) == 1;
        }

        public override bool ColumnExists(string table, string column)
        {
            if (!TableExists(table))
                return false;

            string sql =
                string.Format(
                    "SELECT COUNT(column_name) FROM user_tab_columns WHERE lower(table_name) = '{0}' AND lower(column_name) = '{1}'",
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


            using (
                IDataReader reader =
                    ExecuteQuery(
                        string.Format(
                            "select column_name, data_type, data_length, data_precision, data_scale FROM USER_TAB_COLUMNS WHERE lower(table_name) = '{0}'",
                            table)))
            {
                while (reader.Read())
                {
                    string colName = reader[0].ToString();
                    DbType colType = DbType.String;
                    string dataType = reader[1].ToString().ToLower();
                    if (dataType.Equals("number"))
                    {
                        int precision = reader.GetInt32(3);
                        int scale = reader.GetInt32(4);
                        if (scale == 0)
                        {
                            colType = precision <= 10 ? DbType.Int16 : DbType.Int64;
                        }
                        else
                        {
                            colType = DbType.Decimal;
                        }
                    }
                    else if (dataType.StartsWith("timestamp") || dataType.Equals("date"))
                    {
                        colType = DbType.DateTime;
                    }
                    columns.Add(new Column(colName, colType));
                }
            }

            return columns.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using Migrator.Framework;
using MySql.Data.MySqlClient;

namespace Migrator.Providers.Mysql
{
    /// <summary>
    /// Summary description for MySqlTransformationProvider.
    /// </summary>
    public class MySqlTransformationProvider : TransformationProvider
    {
        public MySqlTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString)
        {
            _connection = new MySqlConnection(_connectionString);
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }

        public override void RemoveForeignKey(string table, string name)
        {
            if (ConstraintExists(table, name))
            {
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP FOREIGN KEY {1}", table, _dialect.Quote(name)));
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP KEY {1}", table, _dialect.Quote(name)));
            }
        }
        
        public override void RemoveConstraint(string table, string name) 
        {
            if (ConstraintExists(table, name))
            {
                ExecuteNonQuery(String.Format("ALTER TABLE {0} DROP KEY {1}", table, _dialect.Quote(name)));
            }
        }

        public override bool ConstraintExists(string table, string name)
        {
            if (!TableExists(table)) 
            return false;

            string sqlConstraint = string.Format("SHOW KEYS FROM {0}", table);

            using (IDataReader reader = ExecuteQuery(sqlConstraint))
            {
                while (reader.Read())
                {
                    if (reader["Key_name"].ToString().ToLower() == name.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public override bool PrimaryKeyExists(string table, string name)
        {
            return ConstraintExists(table, "PRIMARY");
        }

        // XXX: Using INFORMATION_SCHEMA.COLUMNS should work, but it was causing trouble, so I used the MySQL specific thing.
        public override Column[] GetColumns(string table)
        {
            List<Column> columns = new List<Column>();
            using (
                IDataReader reader =
                    ExecuteQuery(
                        String.Format("SHOW COLUMNS FROM {0}", table)))
            {
                while (reader.Read())
                {
                    Column column = new Column(reader.GetString(0), DbType.String);
                    string nullableStr = reader.GetString(2);
                    bool isNullable = nullableStr == "YES";
                    column.ColumnProperty |= isNullable ? ColumnProperty.Null : ColumnProperty.NotNull;

                    columns.Add(column);
                }
            }

            return columns.ToArray();
        }

        public override string[] GetTables()
        {
            List<string> tables = new List<string>();
            using (IDataReader reader = ExecuteQuery("SHOW TABLES"))
            {
                while (reader.Read())
                {
                    tables.Add((string) reader[0]);
                }
            }

            return tables.ToArray();
        }

        public override void ChangeColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(String.Format("ALTER TABLE {0} MODIFY {1}", table, sqlColumn));
        }

        public override void AddTable(string name, params Column[] columns)
        {
            AddTable(name, "INNODB", columns);
        }

        public override void AddTable(string name, string engine, string columns)
        {
            string sqlCreate = string.Format("CREATE TABLE {0} ({1}) ENGINE = {2}", name, columns, engine);
            ExecuteNonQuery(sqlCreate);
        }
        
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{0}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
            {
                string definition = null;
                using (IDataReader reader = ExecuteQuery(String.Format("SHOW COLUMNS FROM {0} WHERE Field='{1}'", tableName, oldColumnName))) 
                {
                    if (reader.Read()) 
                    {
                        // TODO: Could use something similar to construct the columns in GetColumns
                        definition = reader["Type"].ToString();
                        if ("NO" == reader["Null"].ToString())
                        {
                            definition += " " + "NOT NULL";
                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("Key")))
                        {
                            string key = reader["Key"].ToString();
                            if ("PRI" == key)
                            {
                                definition += " " + "PRIMARY KEY";
                            }
                            else if ("UNI" == key)
                            {
                                definition += " " + "UNIQUE";
                            }
                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("Extra")))
                        {
                            definition += " " + reader["Extra"].ToString();
                        }
                    }
                }
                
                if (!String.IsNullOrEmpty(definition)) 
                {
                    ExecuteNonQuery(String.Format("ALTER TABLE {0} CHANGE {1} {2} {3}", tableName, oldColumnName, newColumnName, definition));
                }
            }
        }
    }
}
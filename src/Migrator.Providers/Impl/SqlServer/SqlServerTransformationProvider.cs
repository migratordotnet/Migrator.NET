#region License

//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Migrator.Framework;

namespace Migrator.Providers.SqlServer
{
    /// <summary>
    /// Migration transformations provider for Microsoft SQL Server.
    /// </summary>
    public class SqlServerTransformationProvider : TransformationProvider
    {
        public SqlServerTransformationProvider(Dialect dialect, string connectionString)
            : base(dialect, connectionString)
        {
            CreateConnection();
        }

    	protected virtual void CreateConnection()
    	{
    		_connection = new SqlConnection();
    		_connection.ConnectionString = _connectionString;
    		_connection.Open();
    	}

        // FIXME: We should look into implementing this with INFORMATION_SCHEMA if possible
        // so that it would be usable by all the SQL Server implementations
    	public override bool ConstraintExists(string table, string name)
        {
            using (IDataReader reader =
                ExecuteQuery(string.Format("SELECT TOP 1 * FROM sysobjects WHERE id = object_id('{0}')", name)))
            {
                return reader.Read();
            }
        }

        public override void AddColumn(string table, string sqlColumn)
        {
            ExecuteNonQuery(string.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
        }

		public override bool ColumnExists(string table, string column)
		{
			if (!TableExists(table))
				return false;

			using (IDataReader reader =
				ExecuteQuery(String.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}'", table, column)))
			{
				return reader.Read();
			}
		}

		public override bool TableExists(string table)
		{
            string tableWithoutBrackets = this.RemoveBrackets(table);
            string schemaName = GetSchemaName(tableWithoutBrackets);
            string tableName = this.GetTableName(tableWithoutBrackets);		    
			using (IDataReader reader =
				ExecuteQuery(String.Format("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='{0}' AND TABLE_NAME='{1}'", schemaName,tableName)))
			{
				return reader.Read();
			}
		}

        protected string GetSchemaName(string longTableName)
        {
            var splitTable = this.SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[0] : "dbo";
        }

        protected string[] SplitTableName(string longTableName)
        {            
            return longTableName.Split('.');
            
        }

        protected string GetTableName(string longTableName)
        {
            var splitTable = this.SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[1] : longTableName;
        }

        protected string RemoveBrackets(string stringWithBrackets)
        {
            return stringWithBrackets.Replace("[", "").Replace("]", "");
        }

        public override void RemoveColumn(string table, string column)
        {
            DeleteColumnConstraints(table, column);
            base.RemoveColumn(table, column);
        }
        
        public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            if (ColumnExists(tableName, newColumnName))
                throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));
                
            if (ColumnExists(tableName, oldColumnName)) 
                ExecuteNonQuery(String.Format("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", tableName, oldColumnName, newColumnName));
        }

        public override void RenameTable(string oldName, string newName)
        {
            if (TableExists(newName))
                throw new MigrationException(String.Format("Table with name '{0}' already exists", newName));

            if (TableExists(oldName))
                ExecuteNonQuery(String.Format("EXEC sp_rename {0}, {1}", oldName, newName));
        }

        // Deletes all constraints linked to a column. Sql Server
        // doesn't seems to do this.
        private void DeleteColumnConstraints(string table, string column)
        {
            string sqlContrainte = FindConstraints(table, column);
            List<string> constraints = new List<string>();
            using (IDataReader reader = ExecuteQuery(sqlContrainte))
            {
                while (reader.Read())
                {
                    constraints.Add(reader.GetString(0));
                }
            }
            // Can't share the connection so two phase modif
            foreach (string constraint in constraints)
            {
                RemoveForeignKey(table, constraint);
            }
        }

        // FIXME: We should look into implementing this with INFORMATION_SCHEMA if possible
        // so that it would be usable by all the SQL Server implementations
    	protected virtual string FindConstraints(string table, string column)
    	{
    		return string.Format(
				"SELECT cont.name FROM SYSOBJECTS cont, SYSCOLUMNS col, SYSCONSTRAINTS cnt  "
				+ "WHERE cont.parent_obj = col.id AND cnt.constid = cont.id AND cnt.colid=col.colid "
    		    + "AND col.name = '{1}' AND col.id = object_id('{0}')",
    		    table, column);
    	}
    }
}

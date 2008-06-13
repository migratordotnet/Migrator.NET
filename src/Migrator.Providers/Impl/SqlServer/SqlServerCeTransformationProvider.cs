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
using System.Data;
using System.Data.SqlServerCe;
using Migrator.Framework;

namespace Migrator.Providers.SqlServer
{
	/// <summary>
	/// Migration transformations provider for Microsoft SQL Server.
	/// </summary>
	public class SqlServerCeTransformationProvider : SqlServerTransformationProvider
	{
		public SqlServerCeTransformationProvider(Dialect dialect, string connectionString)
			: base(dialect, connectionString)
		{

		}

		protected override void CreateConnection()
		{
			_connection = new SqlCeConnection();
			_connection.ConnectionString = _connectionString;
			_connection.Open();
		}

		public override bool ConstraintExists(string table, string name)
		{
			using (IDataReader reader =
				ExecuteQuery(string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS cont WHERE cont.Constraint_Name='{0}'", name)))
			{
				return reader.Read();
			}
		}

		public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
		{
			if (ColumnExists(tableName, newColumnName))
				throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));

			if (ColumnExists(tableName, oldColumnName))
			{
				Column column = GetColumnByName(tableName, oldColumnName);

				AddColumn(tableName, new Column(newColumnName, column.Type, column.ColumnProperty, column.DefaultValue));
				ExecuteNonQuery(string.Format("UPDATE {0} SET {1}={2}", tableName, newColumnName, oldColumnName));
				RemoveColumn(tableName, oldColumnName);
			}
		}

		// Not supported by SQLCe when we have a better schemadumper which gives the exact sql construction including constraints we may use it to insert into a new table and then drop the old table...but this solution is dangerous for big tables.
		public override void RenameTable(string oldName, string newName)
		{
			
			if (TableExists(newName))
				throw new MigrationException(String.Format("Table with name '{0}' already exists", newName));

			//if (TableExists(oldName))
			//    ExecuteNonQuery(String.Format("EXEC sp_rename {0}, {1}", oldName, newName));
		}

		protected override string FindConstraints(string table, string column)
		{
			return
				string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE cont "
					+ "WHERE cont.Table_Name='{0}' AND cont.column_name = '{1}'",
					table, column);
		}
	}
}

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
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using ForeignKeyConstraint=Migrator.Providers.ForeignKeys.ForeignKeyConstraint;
using Migrator.Providers.TypeToSqlProviders;

namespace Migrator.Providers
{
	/// <summary>
	/// Migration transformations provider for Microsoft SQL Server.
	/// </summary>
	public class SqlServerTransformationProvider : TransformationProvider
	{
		private string _connectionString;
		
		public SqlServerTransformationProvider(string connectionString)
		{
			_connectionString = connectionString;
			_connection = new SqlConnection();
			_connection.ConnectionString = _connectionString;
			_connection.Open();
		}
		
		// Not sure how SQL server handles ON UPDATRE & ON DELETE
		public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint)
		{
			if (ConstraintExists(name, primaryTable))
			{
				Logger.Warn("The contraint {0} already exists", name);
				return;
			}
			ExecuteNonQuery( string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4})",
			        primaryTable, name, string.Join(",", primaryColumns),
			        refTable, string.Join(",", refColumns)));
		}
		
		public override void RemoveForeignKey(string name, string table)
		{
			if (TableExists(table) && ConstraintExists(name, table))
				ExecuteNonQuery( string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", table, name));
		}
		
		public override bool ConstraintExists(string name, string table)
		{
			using (IDataReader reader =
			       ExecuteQuery( string.Format("SELECT TOP 1 * FROM sysobjects WHERE id = object_id('{0}')",
			                   name)))
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
			       ExecuteQuery( string.Format("SELECT TOP 1 * FROM syscolumns WHERE id=object_id('{0}') and name='{1}'",
			                   table, column)))
			{
				return reader.Read();
			}
		}
		
		public override bool TableExists(string table)
		{
			using (IDataReader reader =
			       ExecuteQuery(string.Format("SELECT TOP 1 * FROM syscolumns WHERE id=object_id('{0}')",
			                   table)))
			{
				return reader.Read();
			}
		}
		
		public override string[] GetTables()
		{
			ArrayList tables = new ArrayList();
		
			using (IDataReader reader =
				ExecuteQuery("SELECT name FROM sysobjects WHERE xtype = 'U'"))
			{
				while (reader.Read())
				{
					tables.Add(reader[0]);
				}
			}
		
			return (string[]) tables.ToArray(typeof (string));
		}
		
		public override Column[] GetColumns(string table)
		{
			ArrayList columns = new ArrayList();
			
			using (IDataReader reader = ExecuteQuery(string.Format("select COLUMN_NAME from information_schema.columns where table_name = '{0}';", table)))
			{
				while(reader.Read())
				{
					columns.Add(new Column(reader[0].ToString(), typeof(string)));
				}
			}
			
			return (Column[])columns.ToArray(typeof(Column));
		}
		
		public override ForeignKeys.IForeignKeyConstraintMapper ForeignKeyMapper
		{
			get { return new ForeignKeys.SQLServerForeignKeyConstraintMapper(); }
		}

		public override void AddTable(string name, string columns)
		{
			string sqlCreate = string.Format("CREATE TABLE {0} ({1})", name, columns);
			ExecuteNonQuery(sqlCreate);
		}

		public override global::Migrator.Providers.TypeToSqlProviders.ITypeToSqlProvider TypeToSqlProvider
		{
			get { return new SQLServerTypeToSqlProvider(); }
		}
		
		public override void RemoveColumn(string table, string column)
		{
			DeleteColumnConstraints(table, column);
			base.RemoveColumn(table, column);
		}

		// Deletes all constraints linked to a column. Sql Server
		// doesn't seems to do this.
		private void DeleteColumnConstraints(string table, string column)
		{
			string sqlContrainte =
				string.Format("SELECT cont.name FROM SYSOBJECTS cont, SYSCOLUMNS col, SYSCONSTRAINTS cnt  "
							+ "WHERE cont.parent_obj = col.id AND cnt.constid = cont.id AND cnt.colid=col.colid "
							+ "AND col.name = '{1}' AND col.id = object_id('{0}')",
							 table, column);
			ArrayList constraints = new ArrayList();
			
			using (IDataReader reader = ExecuteQuery(sqlContrainte))
			{
				while (reader.Read())
				{
					constraints.Add(reader.GetString(0));
				}
			}
			// Can't share the connection so two phase modif
			foreach (string constraint in constraints) {
				RemoveForeignKey(constraint, table);
			}
		}

		public override ProviderType ProviderType
		{
			get
			{
				return ProviderType.SqlServer;
			}
		}

		public override string Quote(string text)
		{
			return string.Format("[{0}]", text);
		}
	}
}

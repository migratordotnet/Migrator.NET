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
		
		public override void RemoveTable(string name)
		{
			if (TableExists(name))
				ExecuteNonQuery( string.Format("DROP TABLE {0}", name));
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
						
		public override void AddColumn(string table, string column, Type type, int size, ColumnProperties property, object defaultValue)
		{
			if (ColumnExists(table, column))
			{
				Logger.Warn("The column {0}.{1} already exists", table, column);
				return;
			}
			
			string sqlColumn = GetSQLForColumn(new Column(column, type, size, property, defaultValue));
			
			ExecuteNonQuery( string.Format("ALTER TABLE {0} ADD {1}", table, sqlColumn));
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
		
		// Deletes all constraints linked to a column. Sql Server
		// doesn't seems to do this.
		private void DeleteColumnConstraints(string table, string column)
		{
			string sqlContrainte =
				string.Format("SELECT cont.name FROM SYSOBJECTS cont, SYSCOLUMNS col "
							+ "WHERE cont.parent_obj = col.id "
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
		
		#region Helper methods
		private string GetSQLForColumn(Column col)
		{
			string sqlType = ToSqlType(col.Type, col.Size);
			string sqlNull = col.ColumnProperty == ColumnProperties.Null ? "NULL" : "NOT NULL";
			string sqlDefault = "";
			string sqlIdentity = "";
			
			if (col.DefaultValue != null)
			{
				string sep = col.Type == typeof(string) ? "'" : "";
				sqlDefault = string.Format("DEFAULT {0}{1}{0}", sep, col.DefaultValue);
			}
			else if (col.Type == typeof(bool)) // Boolean must always have default value
			{
				sqlDefault = "DEFAULT (0)";
			}
			
			if (col.ColumnProperty == ColumnProperties.PrimaryKeyWithIdentity
			    || col.ColumnProperty == ColumnProperties.Identity)
			{
				sqlIdentity = string.Format("IDENTITY (1, 1)");
			}
			
			return string.Join(" ", new string[] { col.Name, sqlType, sqlIdentity, sqlNull, sqlDefault });
		}
				
		private string ToSqlType(Type type, int size)
		{
			if (type == typeof(string))
			{
				if (size <= 255)
					return string.Format("varchar({0})", size);
				else
					return "text";
			}
			else if (type == typeof(int))
			{
				if(size >= 8)
					return "bigint";
				else 
					return "int";
			}
			else if (type == typeof(float) || type == typeof(double))
			{
				if (size == 0)
					return "real";
				else
					return string.Format("float({0})", size);
			}
			else if (type == typeof(bool))
			{
				return "bit";
			}
			else if (type == typeof(DateTime))
			{
				return "datetime";
			}
			else if (type == typeof(char))
			{
				return string.Format("char({0})", size);
			}
			else if (type == typeof(Guid))
			{
				return "uniqueidentifier";
			}
			else
			{
				throw new NotSupportedException("Type not supported : " + type.Name);
			}
		}
		#endregion

		public override ForeignKeys.IForeignKeyConstraintMapper ForeignKeyMapper
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		protected override void AddTable(string name, string columns)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override global::Migrator.Providers.TypeToSqlProviders.ITypeToSqlProvider TypeToSqlProvider
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void AddColumn(string table, string sqlColumn)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

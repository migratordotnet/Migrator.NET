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
using Migrator.Providers.ForeignKeys;
using Migrator.Providers.TypeToSqlProviders;
using Npgsql;
using Migrator.Providers.ColumnPropertiesMappers;
using System.Collections.Generic;

namespace Migrator.Providers
{
	/// <summary>
	/// Migration transformations provider for Microsoft SQL Server.
	/// </summary>
	public class PostgreSQLTransformationProvider : TransformationProvider
	{
		private string _connectionString;
		
		public PostgreSQLTransformationProvider(string connectionString)
		{
			_connectionString = connectionString;
			_connection = new NpgsqlConnection();
			_connection.ConnectionString = _connectionString;
			_connection.Open();
		}

		public override void RemoveTable(string name)
		{
			if (TableExists(name))
			{
				string sql = string.Format("DROP TABLE \"{0}\"", name.ToLower());
				Logger.Trace(sql);
				ExecuteNonQuery(sql);
			}
		}

		public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint)
		{
			if (ConstraintExists(name, primaryTable))
			{
				Logger.Warn("Constraint {0} already exists", name);
				return;
			}

			List<string> primaryColumnsLowerCase = new List<string>(primaryColumns);
			primaryColumnsLowerCase = primaryColumnsLowerCase.ConvertAll(new Converter<string, string>(delegate(string toLower) { return "\"" + toLower.ToLower() + "\""; }));

			string sql = string.Format("ALTER TABLE \"{0}\" ADD CONSTRAINT \"{1}\" FOREIGN KEY ({2}) REFERENCES \"{3}\" ({4})",
						   primaryTable.ToLower(), name.ToLower(), string.Join(",", primaryColumnsLowerCase.ToArray()),
						   refTable.ToLower(), string.Join(",", refColumns));
			Logger.Trace(sql);
			ExecuteNonQuery(sql);
		}

		public override void RemoveForeignKey(string name, string table)
		{
			if (TableExists(table) && ConstraintExists(name, table))
			{
				string sql = string.Format("ALTER TABLE \"{0}\" DROP CONSTRAINT \"{1}\"", table.ToLower(), name.ToLower());
				Logger.Trace(sql);
				ExecuteNonQuery(sql);
			}
		}

		public override bool ConstraintExists(string name, string table)
		{
			string sql = string.Format("SELECT COUNT(constraint_name) FROM information_schema.table_constraints WHERE table_schema = 'public' AND constraint_schema = 'public' AND constraint_name = '{0}'", name.ToLower());
			Logger.Trace(sql);
			object scalar = ExecuteScalar(sql);
			return Convert.ToInt32(scalar) == 1;
		}

		public override bool ColumnExists(string table, string column)
		{
			if (!TableExists(table))
				return false;

			string sql = string.Format("SELECT COUNT(column_name) FROM information_schema.columns WHERE table_schema = 'public' AND table_name = '{0}' AND column_name = '{1}'",
										table.ToLower(), column.ToLower());
			Logger.Trace(sql);
			object scalar = ExecuteScalar(sql);
			return Convert.ToInt32(scalar) == 1;
		}

		private void DeleteColumnConstraints(string table, string column)
		{
			string sqlContrainte =
				string.Format("SELECT a.constraint_name FROM information_schema.table_constraints a, information_schema.key_column_usage b "
				+ "WHERE a.table_name = '{0}' AND a.table_schema = 'public' AND a.constraint_schema = 'public' "
				+ "AND a.table_name = b.table_name AND a.table_schema = b.table_schema "
				+ "AND a.constraint_name = b.constraint_name AND b.column_name = '{1}'",
				table.ToLower(), column.ToLower());
			ArrayList constraints = new ArrayList();

			using (IDataReader reader = ExecuteQuery(sqlContrainte))
			{
				while (reader.Read())
				{
					constraints.Add(reader.GetString(0));
				}
			}
			foreach (string constraint in constraints)
			{
				RemoveForeignKey(constraint, table);
			}
		}

		public override bool TableExists(string table)
		{
			string sql = string.Format("SELECT COUNT(table_name) FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '{0}'",
									   table.ToLower());
			Logger.Trace(sql);
			object count = ExecuteScalar(sql);
			return Convert.ToInt32(count) == 1;
		}
		
		public override string[] GetTables()
		{
			throw new NotImplementedException();
		}
		
		public override Column[] GetColumns(string table)
		{
			throw new NotImplementedException();
		}
		
		#region Helper methods
		#endregion


		public override void AddTable(string name, params Column[] columns)
		{
			if (TableExists(name))
			{
				Logger.Warn("Table {0} already exists", name);
				return;
			}

			ArrayList columnProviders = new ArrayList(columns.Length);
			foreach (Column column in columns)
			{
				IColumnPropertiesMapper mapper = GetAndMapColumnProperties(column);
				columnProviders.Add(mapper);
			}

			IColumnPropertiesMapper[] columnArray = (IColumnPropertiesMapper[])columnProviders.ToArray(typeof(IColumnPropertiesMapper));
			AddTable(name, JoinColumns(columnArray));

			IList<string> indexes = new List<string>(columnArray.Length);
			foreach (IColumnPropertiesMapper mapper in columnArray)
			{
				string indexSql = mapper.IndexSql;
				if (indexSql != null)
					indexes.Add(string.Format("CREATE INDEX {0}_{1}_index on \"{3}\" ({2})", name, mapper.Name, mapper.Quote(mapper.Name.ToLower()), name.ToLower()));
			}

			foreach(string index in indexes)
			{
				ExecuteNonQuery(index);
			}
		}

		public override void AddTable(string name, string columns)
		{
			string sqlCreate = string.Format("CREATE TABLE \"{0}\" ({1})", name.ToLower(), columns);
			ExecuteNonQuery(sqlCreate);
		}

		public override IForeignKeyConstraintMapper ForeignKeyMapper
		{
			get 
			{
				return new ForeignKeys.PostgreSQLForeignKeyConstraintMapper();
			}
		}

		public override ITypeToSqlProvider TypeToSqlProvider
		{
			get { return new PostgreSQLTypeToSqlProvider(); }
		}

		public override ProviderType ProviderType
		{
			get
			{
				return ProviderType.Postgres;
			}
		}

	}
}

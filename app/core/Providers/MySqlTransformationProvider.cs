using System;
using System.Data;
using System.Collections;

using MySql.Data.MySqlClient;
using Migrator.Providers.ColumnPropertiesMappers;
using Migrator.Providers.TypeToSqlProviders;

namespace Migrator.Providers
{
	/// <summary>
	/// Summary description for MySqlTransformationProvider.
	/// </summary>
	public class MySqlTransformationProvider : TransformationProvider
	{
		private string _connectionString;
		

		public MySqlTransformationProvider( string constr )
		{
			_connectionString = constr;
			_connection = new MySqlConnection( _connectionString );
			_connection.ConnectionString = _connectionString;
			_connection.Open();
		}
		
		public override void RemoveForeignKey( string name, string table )
		{
			if( TableExists( table ) && ConstraintExists( name, table ) )
			{
				ExecuteNonQuery( string.Format("ALTER TABLE {0} DROP FOREIGN KEY {1}", table, Quote(name) ));
				ExecuteNonQuery( string.Format( "ALTER TABLE {0} DROP KEY {1}", table, Quote( name ) ) );
			}
		}

		public override bool ConstraintExists( string name, string table )
		{
			if (!TableExists(table)) return false;
			
			string sqlConstraint = string.Format("SHOW KEYS FROM {0}", table);
			
			using( IDataReader reader = ExecuteQuery( sqlConstraint ) )
			{
				while( reader.Read() )
				{
					if (reader["Key_name"].ToString().ToLower() == name.ToLower())
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		public override string[] GetTables()
		{
			ArrayList tables = new ArrayList();
			
			using (IDataReader reader = ExecuteQuery("SHOW TABLES"))
			{
				while(reader.Read())
				{
					tables.Add(reader[0]);
				}
			}
			
			return (string[]) tables.ToArray(typeof(string));
		}
		
		public override Column[] GetColumns(string table)
		{
			ArrayList columns = new ArrayList();
			
			using (IDataReader reader = ExecuteQuery("SHOW FIELDS FROM " + table))
			{
				while(reader.Read())
				{
					// TODO retreive some more info about the column
					columns.Add(new Column(reader[0].ToString(), typeof(string)));
				}
			}
			
			return (Column[]) columns.ToArray(typeof(Column));
		}

		public override ForeignKeys.IForeignKeyConstraintMapper ForeignKeyMapper
		{
			get { return new ForeignKeys.MysqlForeignKeyConstraintMapper(); }
		}

		protected override void AddTable(string name, string columns)
		{
			string sqlCreate = string.Format("CREATE TABLE {0} ({1}) ENGINE = INNODB", name, columns);
			ExecuteNonQuery(sqlCreate);
		}

		public override ITypeToSqlProvider TypeToSqlProvider
		{
			get { return new MysqlTypeToSqlProvider(); }
		}

	}
}

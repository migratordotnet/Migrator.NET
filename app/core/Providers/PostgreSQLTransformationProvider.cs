using System;
using System.Collections;
using System.Data;
using Migrator.Providers.ForeignKeys;
using Npgsql;
using ForeignKeyConstraint=Migrator.Providers.ForeignKeys.ForeignKeyConstraint;

namespace Migrator.Providers
{
	public class PostgreSQLTransformationProvider : TransformationProvider
	{
		private string _connectionString;

		public PostgreSQLTransformationProvider( string constr )
		{
			_connectionString = constr;
			_connection = new NpgsqlConnection( _connectionString );
			_connection.Open();
		}

		public override void AddTable( string name, params Column[] columns )
		{
			if (TableExists(name))
			{
				Logger.Warn("Table {0} already exists", name);
				return;
			}
			
			string[] sqlColumns = new string[columns.Length];
			int i = 0;
			ArrayList pk = new ArrayList();
			
			foreach (Column col in columns)
			{
				sqlColumns[i] = GetSQLForColumn(col);
				
				if (col.ColumnProperty == ColumnProperties.PrimaryKey
					|| col.ColumnProperty == ColumnProperties.PrimaryKeyWithIdentity
					)
				{
					pk.Add(col.Name);
				}
				
				i++;
			}

			string sql = string.Format("CREATE TABLE \"{0}\" ({1})", name.ToLower(), string.Join(", ", sqlColumns) );
			Logger.Log( sql );
			ExecuteNonQuery( sql );
			
			if (pk.Count > 0)
				AddPrimaryKey(string.Format("PK_{0}", name), name, (string[]) pk.ToArray(typeof(string)));
		}

		public override void RemoveTable( string name )
		{
			if( TableExists( name ) )
			{
				string sql = string.Format( "DROP TABLE \"{0}\"", name.ToLower() );
				Logger.Log( sql );
				ExecuteNonQuery( sql );
			}
		}

		public override void AddColumn( string table, string column, Type type, int size, ColumnProperties property, object defaultValue )
		{
			if( ColumnExists( table, column ) )
			{
				Logger.Warn( "Column {0}.{1} already exists", table, column );
				return;
			}

			string sqlColumn = GetSQLForColumn( new Column( column, type, size, property, defaultValue ) );

			string sql = string.Format("ALTER TABLE \"{0}\" ADD COLUMN {1}", table.ToLower(), sqlColumn );
			Logger.Log( sql );
			ExecuteNonQuery( sql);
		}

		public override void RemoveColumn( string table, string column )
		{
			if( ColumnExists( table, column ) )
			{
				DeleteColumnConstraints( table, column );
				string sql = string.Format("ALTER TABLE \"{0}\" DROP COLUMN \"{1}\" ", table.ToLower(), column.ToLower() );
				Logger.Log( sql );
				ExecuteNonQuery( sql );
			}
		}

		public override bool ColumnExists( string table, string column )
		{
			if( !TableExists( table ) )
				return false;

			string sql = string.Format( "SELECT COUNT(column_name) FROM information_schema.columns WHERE table_schema = 'public' AND table_name = '{0}' AND column_name = '{1}'",
			                            table.ToLower(), column.ToLower() );
			Logger.Log( sql );
			object scalar = ExecuteScalar( sql );
			return Convert.ToInt32( scalar ) == 1;
		}

		public override bool TableExists( string table )
		{
			string sql = string.Format("SELECT COUNT(table_name) FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '{0}'",
			                           table.ToLower());
			Logger.Log( sql );
			object count = ExecuteScalar( sql );
			return Convert.ToInt32( count ) == 1;
		}

		public override void AddPrimaryKey( string name, string table, params string[] columns )
		{
			if( ConstraintExists( name, table ) )
			{
				Logger.Warn( "Primary key {0} already exists", name );
				return;
			}
			string sql = string.Format("ALTER TABLE \"{0}\" ADD CONSTRAINT \"{1}\" PRIMARY KEY ({2}) ", table.ToLower(), name.ToLower(), string.Join( ",", columns ) );
			Logger.Log( sql );
			ExecuteNonQuery( sql);
		}

		public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint)
		{
			if( ConstraintExists( name, primaryTable ) )
			{
				Logger.Warn( "Constraint {0} already exists", name );
				return;
			}
			string sql = string.Format("ALTER TABLE \"{0}\" ADD CONSTRAINT \"{1}\" FOREIGN KEY ({2}) REFERENCES \"{3}\" ({4})",
			                           primaryTable.ToLower(), name.ToLower(), string.Join( ",", primaryColumns ),
			                           refTable.ToLower(), string.Join( ",", refColumns ) );
			Logger.Log( sql );
			ExecuteNonQuery( sql);
		}

		public override void RemoveForeignKey( string name, string table )
		{
			if( TableExists( table ) && ConstraintExists( name, table ) )
			{
				string sql = string.Format( "ALTER TABLE \"{0}\" DROP CONSTRAINT \"{1}\"", table.ToLower(), name.ToLower() );
				Logger.Log( sql );
				ExecuteNonQuery( sql );
			}
		}

		public override bool ConstraintExists( string name, string table )
		{
			string sql = string.Format( "SELECT COUNT(constraint_name) FROM information_schema.table_constraints WHERE table_schema = 'public' AND constraint_schema = 'public' AND constraint_name = '{0}'", name.ToLower() );
			Logger.Log( sql );
			object scalar = ExecuteScalar( sql );
			return Convert.ToInt32( scalar ) == 1;
		}

		private void DeleteColumnConstraints( string table, string column )
		{
			string sqlContrainte =
				string.Format( "SELECT a.constraint_name FROM information_schema.table_constraints a, information_schema.key_column_usage b "
				+ "WHERE a.table_name = '{0}' AND a.table_schema = 'public' AND a.constraint_schema = 'public' "
				+ "AND a.table_name = b.table_name AND a.table_schema = b.table_schema "
				+ "AND a.constraint_name = b.constraint_name AND b.column_name = '{1}'",
				table.ToLower(), column.ToLower() );
			ArrayList constraints = new ArrayList();

			using( IDataReader reader = ExecuteQuery( sqlContrainte ) )
			{
				while( reader.Read() )
				{
					constraints.Add( reader.GetString( 0 ) );
				}
			}
			foreach( string constraint in constraints )
			{
				RemoveForeignKey( constraint, table );
			}
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

		private string GetSQLForColumn( Column col )
		{
			string sqlType = ToSqlType( col.Type, col.Size );
			string sqlNull = col.ColumnProperty == ColumnProperties.Null ? "NULL" : "NOT NULL";
			string sqlDefault = "";
			string sqlIdentity = "";

			if( col.DefaultValue != null )
			{
				string sep = col.Type == typeof( string ) ? "'" : "";
				sqlDefault = string.Format( "DEFAULT {0}{1}{0}", sep, col.DefaultValue );
			}
			else if( col.Type == typeof( bool ) )
			{
				sqlDefault = "DEFAULT ('FALSE')";
			}

			if( col.ColumnProperty == ColumnProperties.PrimaryKeyWithIdentity
				|| col.ColumnProperty == ColumnProperties.Identity )
			{
				sqlType = "serial";
			}

			return string.Join( " ", new string[] {string.Format( "\"{0}\"",col.Name.ToLower()), sqlType, sqlIdentity, sqlNull, sqlDefault} );
		}

		private string ToSqlType( Type type, int size )
		{
			if( type == typeof( string ) )
				if( size <= 255 )
					return string.Format( "varchar({0})", size );
				else
					return "text";
			if ( type == typeof( char ) )
			    return string.Format( "char({0})", size );
			else if( type == typeof( int ) )
				return "int4";
			else if( type == typeof( float ) )
				return "float4";
			else if( type == typeof( double ) )
				if( size == 0 )
					return "float8";
				else
					return string.Format( "numeric({0})", size );
			else if( type == typeof( bool ) )
				return "boolean";
			else if( type == typeof( DateTime ) )
				return "timestamp";
			else
				throw new NotSupportedException( "Type not supported" );
		}

		#endregion

		public override IForeignKeyConstraintMapper ForeignKeyMapper
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

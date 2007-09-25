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
using Migrator.Loggers;
using Migrator.Providers.ForeignKeys;
using ForeignKeyConstraint=Migrator.Providers.ForeignKeys.ForeignKeyConstraint;
using Migrator.Providers.TypeToSqlProviders;
using System.Collections;
using Migrator.Providers.ColumnPropertiesMappers;
using Migrator.Columns;

namespace Migrator.Providers
{
	/// <summary>
	/// Base class for every transformation providers.
	/// A 'tranformation' is an operation that modifies the database.
	/// </summary>
	public abstract class TransformationProvider : ITransformationProvider
	{
		private ILogger _logger = new NullLogger();
		protected IDbConnection _connection;
		private IDbTransaction _transaction;

		/// <summary>
		/// Returns the event logger
		/// </summary>
		virtual public ILogger Logger
		{
			get { return _logger; }
			set { _logger = value; }
		}
		
		/// <summary>
		/// Add a new table
		/// </summary>
		/// <param name="name">Table name</param>
		/// <param name="columns">Columns</param>
		/// <example>
		/// Adds the Test table with two columns:
		/// <code>
		/// Database.AddTable("Test",
		///	                  new Column("Id", typeof(int), ColumnProperties.PrimaryKey),
		///	                  new Column("Title", typeof(string), 100)
		///	                 );
		/// </code>
		/// </example>
		public virtual void AddTable(string name, params Column[] columns)
		{
			if (TableExists(name))
			{
				Logger.Warn("Table {0} already exists", name);
				return;
			}

			ArrayList columnProviders = new ArrayList(columns.Length);
			foreach(Column column in columns)
			{
				IColumnPropertiesMapper mapper = GetAndMapColumnProperties(column);
				columnProviders.Add(mapper);
			}

			IColumnPropertiesMapper[] columnArray = (IColumnPropertiesMapper[])columnProviders.ToArray(typeof(IColumnPropertiesMapper));
			string columnsAndIndexes = JoinColumnsAndIndexes(columnArray);
			AddTable(name, columnsAndIndexes);
		}

		public virtual void RemoveTable(string name)
		{
			if (TableExists(name))
				ExecuteNonQuery(string.Format("DROP TABLE {0}", name));
		}

		public virtual void AddColumn(string table, string sqlColumn)
		{
			ExecuteNonQuery(string.Format("ALTER TABLE {0} ADD COLUMN {1}", table, sqlColumn));
		}

		public virtual void RemoveColumn(string table, string column)
		{
			if (ColumnExists(table, column))
			{
				ExecuteNonQuery(string.Format("ALTER TABLE {0} DROP COLUMN {1} ", table, column));
			}
		}

		public virtual bool ColumnExists(string table, string column)
		{
			try
			{
				ExecuteNonQuery(string.Format("SELECT {0} FROM {1}", column, table));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public virtual bool TableExists(string table)
		{
			try
			{
				ExecuteNonQuery("SELECT COUNT(*) FROM " + table);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		protected virtual string JoinColumnsAndIndexes(IColumnPropertiesMapper[] columns)
		{
			string indexes = JoinIndexes(columns);
			string columnsAndIndexes = JoinColumns(columns) + (indexes != null ? "," + indexes : string.Empty);
			return columnsAndIndexes;
		}

		protected abstract void AddTable(string name, string columns);

		protected IColumnPropertiesMapper GetAndMapColumnProperties(Column column)
		{
			IColumnPropertiesMapper mapper = GetColumnMapper(column);
			MapColumnProperties(mapper, column);
			return mapper;
		}

		protected void MapColumnProperties(IColumnPropertiesMapper mapper, Column column)
		{
			mapper.Name = column.Name;
			ColumnProperties properties = column.ColumnProperty;
			if ((properties & ColumnProperties.NotNull) == ColumnProperties.NotNull)
				mapper.NotNull();
			if ((properties & ColumnProperties.PrimaryKey) == ColumnProperties.PrimaryKey)
				mapper.PrimaryKey();
			if ((properties & ColumnProperties.Identity) == ColumnProperties.Identity)
				mapper.Identity();
			if ((properties & ColumnProperties.Unique) == ColumnProperties.Unique)
				mapper.Unique();
			if ((properties & ColumnProperties.Indexed) == ColumnProperties.Indexed)
				mapper.Indexed();
			if ((properties & ColumnProperties.Unsigned) == ColumnProperties.Unsigned)
				mapper.Unsigned();
		}

		protected IColumnPropertiesMapper GetColumnMapper(Column column)
		{
			if (column.Type == typeof(char))
			{
				if (column.Size <= Convert.ToInt32(byte.MaxValue))
					return TypeToSqlProvider.Char(Convert.ToByte(column.Size));
				else if (column.Size <= Convert.ToInt32(ushort.MaxValue))
					return TypeToSqlProvider.Text;
				else
					return TypeToSqlProvider.LongText;
			}

			if (column.Type== typeof(string))
			{
				if (column.Size <= 255)
					return TypeToSqlProvider.String(Convert.ToUInt16(column.Size));
				else if (column.Size <= Convert.ToInt32(ushort.MaxValue))
					return TypeToSqlProvider.Text;
				else
					return TypeToSqlProvider.LongText;
			}

			if (column.Type == typeof(int))
				return TypeToSqlProvider.Integer;

			if (column.Type == typeof(long))
				return TypeToSqlProvider.Long;

			if (column.Type == typeof(float))
				return TypeToSqlProvider.Float;

			if (column.Type == typeof(double))
			{
				if (column.Size == 0)
					return TypeToSqlProvider.Double;
				else
					return TypeToSqlProvider.Decimal(column.Size);
			}

			if (column.Type == typeof(decimal))
			{
				if (typeof(DecimalColumn).IsAssignableFrom(column.GetType()))
				{
					return TypeToSqlProvider.Decimal(column.Size, (column as DecimalColumn).Remainder);
				}
				else
				{
					return TypeToSqlProvider.Decimal(column.Size);
				}
			}

			if (column.Type == typeof(bool))
				return TypeToSqlProvider.Bool;

			if (column.Type == typeof(DateTime))
				return TypeToSqlProvider.DateTime;

			if (column.Type == typeof(byte[]))
			{
				if (column.Size <= Convert.ToInt32(byte.MaxValue))
					return TypeToSqlProvider.Binary(Convert.ToByte(column.Size));
				else if (column.Size <= Convert.ToInt32(ushort.MaxValue))
					return TypeToSqlProvider.Blob;
				else
					return TypeToSqlProvider.LongBlob;
			}

			throw new ArgumentOutOfRangeException("column.Type", "The " + column.Type.ToString() + " type is not supported");
		}

		protected virtual string JoinIndexes(IColumnPropertiesMapper[] columns)
		{
			ArrayList indexes = new ArrayList(columns.Length);
			foreach (IColumnPropertiesMapper column in columns)
			{
				string indexSql = column.IndexSql;
				if (indexSql != null)
					indexes.Add(indexSql);
			}

			if (indexes.Count == 0)
				return null;

			return string.Join(", ", (string[])indexes.ToArray(typeof(string)));
		}

		protected virtual string JoinColumns(IColumnPropertiesMapper[] columns)
		{
			string[] columnStrings = new string[columns.Length];
			int i = 0;
			foreach (IColumnPropertiesMapper column in columns)
				columnStrings[i++] = column.ColumnSql;
			return String.Join(", ", columnStrings);
		}
		
		/// <summary>
		/// Add a new column to an existing table.
		/// </summary>
		/// <param name="table">Table to which to add the column</param>
		/// <param name="column">Column name</param>
		/// <param name="type">Date type of the column</param>
		/// <param name="size">Max length of the column</param>
		/// <param name="property">Properties of the column, see <see cref="ColumnProperties">ColumnProperties</see>,</param>
		/// <param name="defaultValue">Default value</param>
		public virtual void AddColumn(string table, string column, Type type, int size, ColumnProperties property, object defaultValue)
		{
			if (ColumnExists(table, column))
			{
				Logger.Warn("Column {0}.{1} already exists", table, column);
				return;
			}

			IColumnPropertiesMapper mapper = GetAndMapColumnProperties(new Column(column, type, size, property, defaultValue));

			AddColumn(table, mapper.ColumnSql);
		}
		
		/// <summary>
		/// <see cref="TransformationProvider.AddColumn(string, string, Type, int, ColumnProperties, object)">
		/// AddColumn(string, string, Type, int, ColumnProperties, object)
		/// </see>
		/// </summary>
		public virtual void AddColumn(string table, string column, Type type)
		{
			AddColumn(table, column, type, 0, ColumnProperties.Null, null);
		}
		
		/// <summary>
		/// <see cref="TransformationProvider.AddColumn(string, string, Type, int, ColumnProperties, object)">
		/// AddColumn(string, string, Type, int, ColumnProperties, object)
		/// </see>
		/// </summary>
		public virtual void AddColumn(string table, string column, Type type, int size)
		{
			AddColumn(table, column, type, size, ColumnProperties.Null, null);
		}
		
		/// <summary>
		/// <see cref="TransformationProvider.AddColumn(string, string, Type, int, ColumnProperties, object)">
		/// AddColumn(string, string, Type, int, ColumnProperties, object)
		/// </see>
		/// </summary>
		public virtual void AddColumn(string table, string column, Type type, ColumnProperties property)
		{
			AddColumn(table, column, type, 0, property, null);
		}
		
		/// <summary>
		/// <see cref="TransformationProvider.AddColumn(string, string, Type, int, ColumnProperties, object)">
		/// AddColumn(string, string, Type, int, ColumnProperties, object)
		/// </see>
		/// </summary>
		public virtual void AddColumn(string table, string column, Type type, int size, ColumnProperties property)
		{
			AddColumn(table, column, type, size, property, null);
		}

		/// <summary>
		/// Append a primary key to a table.
		/// </summary>
		/// <param name="name">Constraint name</param>
		/// <param name="table">Table name</param>
		/// <param name="columns">Primary column names</param>
		public virtual void AddPrimaryKey( string name, string table, params string[] columns )
		{
			if( ConstraintExists( name, table ) )
			{
				Logger.Warn( "Primary key {0} already exists", name );
				return;
			}
			ExecuteNonQuery( string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY ({2}) ", table, name, string.Join( ",", columns ) ));
		}


		/// <summary>
		/// Guesses the name of the foreign key and add it
		/// </summary>
		public virtual void GenerateForeignKey(string primaryTable, string primaryColumn, string refTable, string refColumn)
		{
			AddForeignKey("FK_" + primaryTable + "_" + refTable, primaryTable, primaryColumn, refTable, refColumn);
		}

		/// <summary>
		/// Guesses the name of the foreign key and add it
		/// </see>
		/// </summary>
		public virtual void GenerateForeignKey(string primaryTable, string[] primaryColumns, string refTable, string[] refColumns)
		{
			AddForeignKey("FK_" + primaryTable + "_" + refTable, primaryTable, primaryColumns, refTable, refColumns);
		}

		/// <summary>
		/// Guesses the name of the foreign key and add it
		/// </summary>
		public virtual void GenerateForeignKey(string primaryTable, string primaryColumn, string refTable, string refColumn, ForeignKeyConstraint constraint)
		{
			AddForeignKey("FK_" + primaryTable + "_" + refTable, primaryTable, primaryColumn, refTable, refColumn, constraint);
		}

		/// <summary>
		/// Guesses the name of the foreign key and add it
		/// </see>
		/// </summary>
		public virtual void GenerateForeignKey(string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint)
		{
			AddForeignKey("FK_" + primaryTable + "_" + refTable, primaryTable, primaryColumns, refTable, refColumns, constraint);
		}

		/// <summary>
		/// Append a foreign key (relation) between two tables.
		/// tables.
		/// </summary>
		/// <param name="name">Constraint name</param>
		/// <param name="primaryTable">Table name containing the primary key</param>
		/// <param name="primaryColumn">Primary key column name</param>
		/// <param name="refTable">Foreign table name</param>
		/// <param name="refColumn">Foreign column name</param>
		public virtual void AddForeignKey(string name, string primaryTable, string primaryColumn, string refTable, string refColumn)
		{
			AddForeignKey(name, primaryTable, new string[] {primaryColumn}, refTable, new string[] {refColumn});
		}
		/// <summary>
		/// <see cref="TransformationProvider.AddForeignKey(string, string, string, string, string)">
		/// AddForeignKey(string, string, string, string, string)
		/// </see>
		/// </summary>
		public virtual void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns)
		{
			AddForeignKey(name, primaryTable, primaryColumns, refTable, refColumns, ForeignKeyConstraint.NoAction);
		}

		public virtual void AddForeignKey(string name, string primaryTable, string primaryColumn, string refTable, string refColumn, ForeignKeyConstraint constraint)
		{
			AddForeignKey(name, primaryTable, new string[] { primaryColumn }, refTable, new string[] { refColumn }, constraint);
			
		}

		public virtual void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeys.ForeignKeyConstraint constraint)
		{
			if( ConstraintExists( name, primaryTable ) )
			{
				Logger.Warn( "Constraint {0} already exists", name );
				return;
			}

			string constraintResolved = ForeignKeyMapper.Resolve(constraint);
			ExecuteNonQuery( string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}) ON UPDATE {5} ON DELETE {6}",
				Quote(primaryTable), Quote(name), string.Join( ",", primaryColumns ),
				Quote(refTable), string.Join( ",", refColumns ), constraintResolved, constraintResolved ));
		}


		/// <summary>
		/// Removes a constraint.
		/// </summary>
		/// <param name="name">Constraint name</param>
		/// <param name="table">Table owning the constraint</param>
		public abstract void RemoveForeignKey(string name, string table);
		
		/// <summary>
		/// Determines if a constraint exists.
		/// </summary>
		/// <param name="name">Constraint name</param>
		/// <param name="table">Table owning the constraint</param>
		/// <returns><c>true</c> if the constraint exists.</returns>
		public abstract bool ConstraintExists(string name, string table);

		public abstract IForeignKeyConstraintMapper ForeignKeyMapper { get;}

		public abstract string[] GetTables();
		
		public abstract Column[] GetColumns(string table);
		
		public int ExecuteNonQuery( string sql )
		{
			IDbCommand cmd = BuildCommand( sql );
			return cmd.ExecuteNonQuery();
		}

		private IDbCommand BuildCommand( string sql )
		{
			IDbCommand cmd = _connection.CreateCommand();
			cmd.CommandText = sql;
			cmd.CommandType = CommandType.Text;
			if( _transaction != null )
			{
				cmd.Transaction = _transaction;
			}
			return cmd;
		}

		/// <summary>
		/// Execute an SQL query returning results.
		/// </summary>
		/// <param name="sql">The SQL command.</param>
		/// <returns>A data iterator, <see cref="System.Data.IDataReader">IDataReader</see>.</returns>
		public IDataReader ExecuteQuery( string sql )
		{
			IDbCommand cmd = BuildCommand( sql );
			return cmd.ExecuteReader();
		}

		public object ExecuteScalar( string sql )
		{
			IDbCommand cmd = BuildCommand( sql );
			return cmd.ExecuteScalar();
		}
		
		public IDataReader Select(string what, string from)
		{
			return Select(what, from, "1=1");
		}
		public virtual IDataReader Select(string what, string from, string where)
		{
			return ExecuteQuery(string.Format("SELECT {0} FROM {1} WHERE {2}", what, from, where));
		}
		
		public object SelectScalar(string what, string from)
		{
			return SelectScalar(what, from, "1=1");
		}
		public virtual object SelectScalar(string what, string from, string where)
		{
			return ExecuteScalar(string.Format("SELECT {0} FROM {1} WHERE {2}", what, from, where));
		}
		
		public virtual int Update(string table, params string[] columnValues)
		{
			return ExecuteNonQuery(string.Format("UPDATE {0} SET {1}", table, string.Join(", ", columnValues)));
		}
		
		public virtual int Insert(string table, params string[] columnValues)
		{
			string[] columns = new string[columnValues.Length];
			string[] values = new string[columnValues.Length];
			int i = 0;
			
			foreach (string cs in columnValues)
			{
				columns[i] = cs.Split('=')[0];
				values[i] = cs.Split('=')[1];
				i ++;
			}
			
			return ExecuteNonQuery(string.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, string.Join(", ", columns), string.Join(", ", values)));
		}
        
		/// <summary>
		/// Starts a transaction. Called by the migration mediator.
		/// </summary>
		public void BeginTransaction()
		{
			if( _transaction == null && _connection != null )
			{
				EnsureHasConnection();
				_transaction = _connection.BeginTransaction( IsolationLevel.ReadCommitted );
			}
		}
		
		protected void EnsureHasConnection()
		{
			if(_connection.State != ConnectionState.Open)
			{
				_connection.Open();
			}
		}
		
		/// <summary>
		/// Rollback the current migration. Called by the migration mediator.
		/// </summary>
		public virtual void Rollback()
		{
			if( _transaction != null && _connection != null && _connection.State == ConnectionState.Open )
			{
				try
				{
					_transaction.Rollback();
				}
				finally
				{
					_connection.Close();
				}
			}
			_transaction = null;
		}
		
		/// <summary>
		/// Commit the current transaction. Called by the migrations mediator.
		/// </summary>
		public void Commit()
		{
			if( _transaction != null && _connection != null && _connection.State == ConnectionState.Open )
			{
				try
				{
					_transaction.Commit();
				}
				finally
				{
					_connection.Close();
				}
			}
			_transaction = null;
		}
		
		/// <summary>
		/// Get or set the current version of the database.
		/// This determines if the migrator should migrate up or down
		/// in the migration numbers.
		/// </summary>
		/// <remark>
		/// This value should not be modified inside a migration.
		/// </remark>
		public virtual int CurrentVersion
		{
			get
			{
				CreateSchemaInfoTable();
				object version = SelectScalar("Version", "SchemaInfo");
				if (version == null)
				{
					return 0;
				}
				else
				{
					return (int) Convert.ToInt32(version);
				}
			}
			set
			{
				CreateSchemaInfoTable();
				int count = Update("SchemaInfo", "Version=" + value);
				if (count == 0)
				{
					Insert("SchemaInfo", "Version=" + value);
				}
			}
		}
		
		protected void CreateSchemaInfoTable()
 		{
 			EnsureHasConnection();
			if (!TableExists("SchemaInfo"))
			{
 				AddTable("SchemaInfo",
		                 new Column("Version", typeof(int), ColumnProperties.PrimaryKey)
		                );
			}
 		}

		#region ITransformationProvider Members

		public abstract ITypeToSqlProvider TypeToSqlProvider { get;}

		public void AddColumn(string table, Column column)
		{
			AddColumn(table, column.Name, column.Type, column.ColumnProperty);
		}

		public void GenerateForeignKey(string primaryTable, string refTable)
		{
			GenerateForeignKey(primaryTable, refTable, ForeignKeyConstraint.NoAction);
		}

		public void GenerateForeignKey(string primaryTable, string refTable, ForeignKeyConstraint constraint)
		{
			GenerateForeignKey(primaryTable, refTable, refTable, "Id");
		}

		#endregion

		#region Helper methods

		protected virtual string Quote(string text)
		{
			return string.Format("`{0}`", text);
		}

		#endregion

	}
}

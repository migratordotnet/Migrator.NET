using System.Data;

namespace Migrator.Framework
{
    
    /// <summary>
    /// The main interface to use in Migrations to make changes on a database schema.
    /// </summary>
    public interface ITransformationProvider
    {
        
        /// <summary>
        /// Get this provider or a NoOp provider if you are not running in the context of 'provider'.
        /// </summary>
        ITransformationProvider this[string provider] { get;}
        
        /// <summary>
        /// The current version that the database is at.
        /// </summary>
        int CurrentVersion { get; set; }
        
        ILogger Logger { get; set; }

        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">The name of the new column</param>
        /// <param name="type">The data type for the new columnd</param>
        /// <param name="size">The precision or size of the column</param>
        /// <param name="property">Properties that can be ORed together</param>
        /// <param name="defaultValue">The default value of the column if no value is given in a query</param>
        void AddColumn(string table, string column, DbType type, int size, ColumnProperty property, object defaultValue);
        
        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">The name of the new column</param>
        /// <param name="type">The data type for the new columnd</param>
        void AddColumn(string table, string column, DbType type);
        
        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">The name of the new column</param>
        /// <param name="type">The data type for the new columnd</param>
        /// <param name="size">The precision or size of the column</param>
        void AddColumn(string table, string column, DbType type, int size);
        
        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">The name of the new column</param>
        /// <param name="type">The data type for the new columnd</param>
        /// <param name="size">The precision or size of the column</param>
        /// <param name="property">Properties that can be ORed together</param>
        void AddColumn(string table, string column, DbType type, int size, ColumnProperty property);
        
        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">The name of the new column</param>
        /// <param name="type">The data type for the new columnd</param>
        /// <param name="property">Properties that can be ORed together</param>
        void AddColumn(string table, string column, DbType type, ColumnProperty property);

        /// <summary>
        /// Add a column to an existing table
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">An instance of a <see cref="Column">Column</see> with the specified properties</param>
        void AddColumn(string table, Column column);

        /// <summary>
        /// Add a foreign key constraint
        /// </summary>
        /// <param name="name">The name of the foreign key. e.g. FK_TABLE_REF</param>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="columns">The columns that reference the foreign keys</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumns">The foreign columns that are referred to</param>
        /// <param name="constraint">Constraint parameters</param>
        void AddForeignKey(string name, string table, string[] columns, string foreignTable,
                           string[] foreignColumns, ForeignKeyConstraint constraint);

        /// <summary>
        /// Add a foreign key constraint
        /// </summary>
        /// <param name="name">The name of the foreign key. e.g. FK_TABLE_REF</param>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="column">The column that reference the foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumn">The foreign column that is referred to</param>
        void AddForeignKey(string name, string table, string column, string foreignTable, string foreignColumn);

        /// <summary>
        /// Add a foreign key constraint
        /// </summary>
        /// <param name="name">The name of the foreign key. e.g. FK_TABLE_REF</param>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="columns">The columns that reference the foreign keys</param>
        /// <param name="foreignTable">The foreign table that are referred to</param>
        /// <param name="foreignColumns">The foreign columns that is referred to</param>
        void AddForeignKey(string name, string table, string[] columns, string foreignTable, string[] foreignColumns);

        /// <summary>
        /// Add a foreign key constraint
        /// </summary>
        /// <param name="name">The name of the foreign key. e.g. FK_TABLE_REF</param>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="column">The column that reference the foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumn">The foreign column that is referred to</param>
        /// <param name="constraint">Constraint parameters</param>
        void AddForeignKey(string name, string table, string column, string foreignTable, string foreignColumn,
                           ForeignKeyConstraint constraint);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="column">The column that reference the foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumn">The foreign column that is referred to</param>
        void GenerateForeignKey(string table, string column, string foreignTable, string foreignColumn);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="columns">The columns that reference the foreign keys</param>
        /// <param name="foreignTable">The foreign table that are referred to</param>
        /// <param name="foreignColumns">The foreign columns that is referred to</param>
        void GenerateForeignKey(string table, string[] columns, string foreignTable, string[] foreignColumns);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="columns">The columns that reference the foreign keys</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumns">The foreign columns that are referred to</param>
        /// <param name="constraint">Constraint parameters</param>
        void GenerateForeignKey(string table, string[] columns, string foreignTable, string[] foreignColumns,
                                ForeignKeyConstraint constraint);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="column">The column that reference the foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="foreignColumn">The foreign column that is referred to</param>
        /// <param name="constraint">Constraint parameters</param>
        void GenerateForeignKey(string table, string column, string foreignTable, string foreignColumn,
                                ForeignKeyConstraint constraint);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        ///
        /// The current expectations are that there is a column named the same as the foreignTable present in
        /// the table. This is subject to change because I think it's not a good convention.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        void GenerateForeignKey(string table, string foreignTable);

        /// <summary>
        /// Add a foreign key constraint when you don't care about the name of the constraint.
        /// Warning: This will prevent you from dropping the constraint since you won't know the name.
        ///
        /// The current expectations are that there is a column named the same as the foreignTable present in
        /// the table. This is subject to change because I think it's not a good convention.
        /// </summary>
        /// <param name="table">The table that contains a reference to a foreign key</param>
        /// <param name="foreignTable">The foreign table that is referred to</param>
        /// <param name="constraint"></param>
        void GenerateForeignKey(string table, string foreignTable, ForeignKeyConstraint constraint);

        /// <summary>
        /// Add a primary key to a table
        /// </summary>
        /// <param name="name">The name of the primary key to add.</param>
        /// <param name="table">The name of the table that will get the primary key.</param>
        /// <param name="columns">The name of the column or columns that are in the primary key.</param>
        void AddPrimaryKey(string name, string table, params string[] columns);

        /// <summary>
        /// Add a constraint to a table
        /// </summary>
        /// <param name="name">The name of the constraint to add.</param>
        /// <param name="table">The name of the table that will get the constraint</param>
        /// <param name="column">The name of the column that will get the constraint.</param>
        void AddUniqueConstraint(string name, string table, string column);
        
        /// <summary>
        /// Add a constraint to a table
        /// </summary>
        /// <param name="name">The name of the constraint to add.</param>
        /// <param name="table">The name of the table that will get the constraint</param>
        /// <param name="checkSql">The check constraint definition.</param>
        void AddCheckConstraint(string name, string table, string checkSql);
        
        /// <summary>
        /// Add a table
        /// </summary>
        /// <param name="name">The name of the table to add.</param>
        /// <param name="columns">The columns that are part of the table.</param>
        void AddTable(string name, params Column[] columns);

        /// <summary>
        /// Start a transction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Change the definition of an existing column.
        /// </summary>
        /// <param name="table">The name of the table that will get the new column</param>
        /// <param name="column">An instance of a <see cref="Column">Column</see> with the specified properties and the name of an existing column</param>
        void ChangeColumn(string table, Column column);
        
        /// <summary>
        /// Check to see if a column exists
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        bool ColumnExists(string table, string column);

        /// <summary>
        /// Commit the running transction
        /// </summary>
        void Commit();
        
        /// <summary>
        /// Check to see if a constraint exists
        /// </summary>
        /// <param name="name">The name of the constraint</param>
        /// <param name="table">The table that the constraint lives on.</param>
        /// <returns></returns>
        bool ConstraintExists(string table, string name);
        
        /// <summary>
        /// Check to see if a primary key constraint exists on the table
        /// </summary>
        /// <param name="name">The name of the primary key</param>
        /// <param name="table">The table that the constraint lives on.</param>
        /// <returns></returns>
        bool PrimaryKeyExists(string table, string name);
        
        /// <summary>
        /// Execute an arbitrary SQL query
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// Execute an arbitrary SQL query
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns></returns>
        IDataReader ExecuteQuery(string sql);

        /// <summary>
        /// Execute an arbitrary SQL query
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns>A single value that is returned.</returns>
        object ExecuteScalar(string sql);
        
        /// <summary>
        /// Get the information about the columns in a table
        /// </summary>
        /// <param name="table">The table name that you want the columns for.</param>
        /// <returns></returns>
        Column[] GetColumns(string table);
        
        /// <summary>
        /// Get the names of all of the tables
        /// </summary>
        /// <returns>The names of all the tables.</returns>
        string[] GetTables();
        
        /// <summary>
        /// Insert data into a table
        /// </summary>
        /// <param name="table">The table that will get the new data</param>
        /// <param name="columns">The names of the columns</param>
        /// <param name="values">The values in the same order as the columns</param>
        /// <returns></returns>
        int Insert(string table, string[] columns, string[] values);
        
        /// <summary>
        /// Remove an existing column from a table
        /// </summary>
        /// <param name="table">The name of the table to remove the column from</param>
        /// <param name="column">The column to remove</param>
        void RemoveColumn(string table, string column);

        /// <summary>
        /// Remove an existing foreign key constraint
        /// </summary>
        /// <param name="table">The table that contains the foreign key.</param>
        /// <param name="name">The name of the foreign key to remove</param>
        void RemoveForeignKey(string table, string name);

        /// <summary>
        /// Remove an existing constraint
        /// </summary>
        /// <param name="table">The table that contains the foreign key.</param>
        /// <param name="name">The name of the constraint to remove</param>
        void RemoveConstraint(string table, string name);
        
        /// <summary>
        /// Remove an existing table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        void RemoveTable(string tableName);
        
        /// <summary>
        /// Rename an existing table
        /// </summary>
        /// <param name="oldName">The old name of the table</param>
        /// <param name="newName">The new name of the table</param>
        void RenameTable(string oldName, string newName);
        
        /// <summary>
        /// Rename an existing table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="oldColumnName">The old name of the column</param>
        /// <param name="newColumnName">The new name of the column</param>
        void RenameColumn(string tableName, string oldColumnName, string newColumnName);
        
        /// <summary>
        /// Rollback the currently running transaction.
        /// </summary>
        void Rollback();
        
        /// <summary>
        /// Get values from a table
        /// </summary>
        /// <param name="what">The columns to select</param>
        /// <param name="from">The table to select from</param>
        /// <param name="where">The where clause to limit the selection</param>
        /// <returns></returns>
        IDataReader Select(string what, string from, string where);

        /// <summary>
        /// Get values from a table
        /// </summary>
        /// <param name="what">The columns to select</param>
        /// <param name="from">The table to select from</param>
        /// <returns></returns>
        IDataReader Select(string what, string from);
     
        /// <summary>
        /// Get a single value from a table
        /// </summary>
        /// <param name="what">The columns to select</param>
        /// <param name="from">The table to select from</param>
        /// <param name="where"></param>
        /// <returns></returns>
        object SelectScalar(string what, string from, string where);

        /// <summary>
        /// Get a single value from a table
        /// </summary>
        /// <param name="what">The columns to select</param>
        /// <param name="from">The table to select from</param>
        /// <returns></returns>
        object SelectScalar(string what, string from);
        
        /// <summary>
        /// Check if a table already exists
        /// </summary>
        /// <param name="tableName">The name of the table that you want to check on.</param>
        /// <returns></returns>
        bool TableExists(string tableName);
        
        /// <summary>
        /// Update the values in a table
        /// </summary>
        /// <param name="table">The name of the table to update</param>
        /// <param name="columns">The names of the columns.</param>
        /// <param name="columnValues">The values for the columns in the same order as the names.</param>
        /// <returns></returns>
        int Update(string table, string[] columns, string[] columnValues);
        
        /// <summary>
        /// Update the values in a table
        /// </summary>
        /// <param name="table">The name of the table to update</param>
        /// <param name="columns">The names of the columns.</param>
        /// <param name="values">The values for the columns in the same order as the names.</param>
        /// <param name="where">A where clause to limit the update</param>
        /// <returns></returns>
        int Update(string table, string[] columns, string[] values, string where);
        
        IDbCommand GetCommand();
    }
}

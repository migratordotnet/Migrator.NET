using System;
using Migrator.Providers.ForeignKeys;
using Migrator.Providers.TypeToSqlProviders;
namespace Migrator.Providers
{
	interface ITransformationProvider
	{
		/// <summary>
		/// Provides Type To <see cref="IMigrateSqlProvider"/>
		/// </summary>
		ITypeToSqlProvider TypeToSqlProvider { get;}

		void AddColumn(string table, string column, Type type, int size, ColumnProperties property, object defaultValue);
		void AddColumn(string table, string column, Type type);
		void AddColumn(string table, string column, Type type, int size);
		void AddColumn(string table, string column, Type type, int size, ColumnProperties property);
		void AddColumn(string table, string column, Type type, ColumnProperties property);

		void AddColumn(string table, Column column);

		void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint);
		void AddForeignKey(string name, string primaryTable, string primaryColumn, string refTable, string refColumn);
		void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns);
		void AddForeignKey(string name, string primaryTable, string primaryColumn, string refTable, string refColumn, ForeignKeyConstraint constraint);

		void GenerateForeignKey(string primaryTable, string primaryColumn, string refTable, string refColumn);
		void GenerateForeignKey(string primaryTable, string[] primaryColumns, string refTable, string[] refColumns);
		void GenerateForeignKey(string primaryTable, string[] primaryColumns, string refTable, string[] refColumns, ForeignKeyConstraint constraint);
		void GenerateForeignKey(string primaryTable, string primaryColumn, string refTable, string refColumn, ForeignKeyConstraint constraint);
		void GenerateForeignKey(string primaryTable, string refTable);
		void GenerateForeignKey(string primaryTable, string refTable, ForeignKeyConstraint constraint);
		
		void AddPrimaryKey(string name, string table, params string[] columns);
		void AddTable(string name, params Column[] columns);
		void BeginTransaction();
		bool ColumnExists(string table, string column);
		void Commit();
		bool ConstraintExists(string name, string table);
		int CurrentVersion { get; set; }
		int ExecuteNonQuery(string sql);
		System.Data.IDataReader ExecuteQuery(string sql);
		object ExecuteScalar(string sql);
		global::Migrator.Providers.ForeignKeys.IForeignKeyConstraintMapper ForeignKeyMapper { get; }
		Column[] GetColumns(string table);
		string[] GetTables();
		int Insert(string table, params string[] columnValues);
		global::Migrator.Loggers.ILogger Logger { get; set; }
		void RemoveColumn(string table, string column);
		void RemoveForeignKey(string name, string table);
		void RemoveTable(string name);
		void Rollback();
		System.Data.IDataReader Select(string what, string from, string where);
		System.Data.IDataReader Select(string what, string from);
		object SelectScalar(string what, string from, string where);
		object SelectScalar(string what, string from);
		bool TableExists(string table);
		int Update(string table, params string[] columnValues);
	}
}

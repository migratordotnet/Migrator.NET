using System;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	public interface IColumnPropertiesMapper
	{
		
		/// <summary>
		/// Get the Sql, called after all the column options have been generated
		/// </summary>
		string ColumnSql { get;}

		/// <summary>
		/// Get the Sql for the index if there is an index
		/// </summary>
		string IndexSql { get;}

		/// <summary>
		/// Set the name for this column provider
		/// </summary>
		string Name { get; set;}

		/// <summary>
		/// Disallow Nulls
		/// </summary>
		void NotNull();

		/// <summary>
		/// It is a Primary Key
		/// </summary>
		void PrimaryKey();

		/// <summary>
		/// Unique Column
		/// </summary>
		void Unique();

		/// <summary>
		/// Indexed Column
		/// </summary>
		void Indexed();

		/// <summary>
		/// Unsigned Column
		/// </summary>
		void Unsigned();

		/// <summary>
		/// Identity column (autoinc..?)
		/// </summary>
		void Identity();

		/// <summary>
		/// Default value for this column, quote if needed
		/// </summary>
		/// <param name="defaultValue"></param>
		void Default(string defaultValue);
	}
}

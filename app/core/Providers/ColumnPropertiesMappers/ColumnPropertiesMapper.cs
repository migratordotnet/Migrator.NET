using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ColumnPropertiesMappers
{
	/// <summary>
	/// This is basically a just a helper base class
	/// per-database implementors may want to override ColumnSql
	/// </summary>
	public abstract class ColumnPropertiesMapper : IColumnPropertiesMapper
	{
		/// <summary>
		/// the type of the column
		/// </summary>
		protected string type;
		
		/// <summary>
		/// name of the column
		/// </summary>
		protected string name;
		
		/// <summary>
		/// This should be set to whatever passes for NULL in implementing 
		/// classes constructors, if it is not NULL
		/// </summary>
		protected string sqlNull = "NULL";

		/// <summary>
		/// Sql if This column is a primary key
		/// </summary>
		protected string sqlPrimaryKey;

		/// <summary>
		/// Sql if This column is Unique
		/// </summary>
		protected string sqlUnique;

		/// <summary>
		/// Sql if This column is Indexed
		/// </summary>
		protected bool indexed = false;

		/// <summary>
		/// Sql if This column is Unsigned
		/// </summary>
		protected string sqlUnsigned;

		/// <summary>
		/// Sql if This column is an Identity Colu,m
		/// </summary>
		protected string sqlIdentity;

		/// <summary>
		/// Sql if this column has a default value
		/// </summary>
		protected string sqlDefault;

		public ColumnPropertiesMapper(string type)
		{
			this.type = type;
		}

		#region IColumnPropertiesMapper Members

		/// <summary>
		/// The sql for this column, override in database-specific implementation classes
		/// </summary>
		string IColumnPropertiesMapper.ColumnSql
		{
			get
			{
				return String.Join(" ", new string[] { name, type, sqlUnsigned, sqlNull, sqlIdentity, sqlUnique, sqlPrimaryKey, sqlDefault });
			}
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public void Indexed()
		{
			indexed = true;
		}

		public abstract string IndexSql { get;}

		public abstract void NotNull();

		public abstract void PrimaryKey();

		public abstract void Unique();

		public abstract void Unsigned();

		public abstract void Identity();

		public abstract void Default(string defaultValue);

		#endregion
	}
}

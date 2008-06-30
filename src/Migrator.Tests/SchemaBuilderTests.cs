using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Migrator.Framework;
using NUnit.Framework;
using ForeignKeyConstraint=Migrator.Framework.ForeignKeyConstraint;

namespace Migrator.Tests
{
	[TestFixture]
	public class SchemaBuilderTests
	{
		private SchemaBuilder _schemaBuilder;

		[SetUp]
		public void Setup()
		{
			_schemaBuilder = new SchemaBuilder();
		}

		[Test]
		public void Can_AddTable()
		{
			_schemaBuilder.AddTable("MyUserTable");

			Assert.AreEqual("MyUserTable", _schemaBuilder.Table);
		}

		[Test]
		public void Can_AddColumn()
		{
			string columnName = "MyUserId";

			_schemaBuilder
				.AddColumn(columnName);

			Assert.IsTrue(_schemaBuilder.Columns.Count == 1);
			Assert.AreEqual(columnName, _schemaBuilder.Columns[0].Name);
		}

		[Test]
		public void Can_chain_AddColumn_OfType()
		{
			_schemaBuilder
				.AddColumn("SomeColumn")
				.OfType(DbType.Int32);

			Assert.AreEqual(DbType.Int32, _schemaBuilder.Columns[0].Type, "Column.Type was not as expected");
		}

		[Test]
		public void Can_chain_AddColumn_WithProperty()
		{
			_schemaBuilder
				.AddColumn("MyColumn")
				.OfType(DbType.Int32)
				.WithProperty(ColumnProperty.PrimaryKey);

			Assert.IsTrue(_schemaBuilder.Columns[0].IsPrimaryKey);
		}

		[Test]
		public void Can_chain_AddColumn_WithSize()
		{
			_schemaBuilder
				.AddColumn("column")
				.WithSize(100);

			Assert.AreEqual(100, _schemaBuilder.Columns[0].Size);
		}

		[Test]
		public void Can_chain_AddColumn_WithDefaultValue()
		{
			_schemaBuilder
				.AddColumn("something")
				.OfType(DbType.Int32)
				.WithDefaultValue("default value");

			Assert.AreEqual("default value", _schemaBuilder.Columns[0].DefaultValue);
		}

		[Test]
		public void Can_chain_AddTable_WithForeignKey()
		{
			_schemaBuilder
				.AddColumn("MyColumnThatIsForeignKey")
				.AsForeignKey().ReferencedTo("PrimaryKeyTable", "PrimaryKeyColumn").WithConstraint(ForeignKeyConstraint.NoAction);

			Assert.IsTrue(_schemaBuilder.Columns[0].ColumnProperty == ColumnProperty.ForeignKey);
		}
	}

	public interface IColumnOptions
	{
		SchemaBuilder OfType(DbType dbType);

		SchemaBuilder WithSize(int size);

		IForeignKeyOptions AsForeignKey();
	}

	public interface IForeignKeyOptions
	{
		SchemaBuilder ReferencedTo(string primaryKeyTable, string primaryKeyColumn);
	}

	public class FluentColumn : IFluentColumn
	{
		private Column _inner;
		private ForeignKeyConstraint _constraint;

		public FluentColumn(string columnName)
		{
			_inner = new Column(columnName);
		}

		public ColumnProperty ColumnProperty
		{
			get { return _inner.ColumnProperty; }
			set { _inner.ColumnProperty = value; }
		}

		public string Name
		{
			get { return _inner.Name; }
			set { _inner.Name = value; }
		}

		public DbType Type
		{
			get { return _inner.Type; }
			set { _inner.Type = value; }
		}

		public int Size
		{
			get { return _inner.Size; }
			set { _inner.Size = value; }
		}

		public bool IsIdentity
		{
			get { return _inner.IsIdentity; }
		}

		public bool IsPrimaryKey
		{
			get { return _inner.IsPrimaryKey; }
		}

		public object DefaultValue
		{
			get { return _inner.DefaultValue; }
			set { _inner.DefaultValue = value; }
		}

		public ForeignKeyConstraint Constraint
		{
			get { return _constraint; }
			set { _constraint = value; }
		}
	}

	public class SchemaBuilder : IColumnOptions, IForeignKeyOptions
	{
		private string _table;
		private IList<IFluentColumn> _columns;
		private IFluentColumn _lastColumn;

		public SchemaBuilder()
		{
			_columns = new List<IFluentColumn>();
		}

		public string Table {
			get { return _table; }
		}

		public ReadOnlyCollection<IFluentColumn> Columns
		{
			get { return new ReadOnlyCollection<IFluentColumn>(_columns); }
		}

		/// <summary>
		/// Adds a Table to be created to the Schema
		/// </summary>
		/// <param name="name">Table name to be created</param>
		/// <returns>SchemaBuilder for chaining</returns>
		public SchemaBuilder AddTable(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_table = name;

			return this;
		}

		/// <summary>
		/// Adds a Column to be created
		/// </summary>
		/// <param name="name">Column name to be added</param>
		/// <returns>IColumnOptions to restrict chaining</returns>
		public IColumnOptions AddColumn(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			IFluentColumn column = new FluentColumn(name);
			_lastColumn = column;
			_columns.Add(column);

			return this;
		}

		public SchemaBuilder OfType(DbType columnType)
		{
			_lastColumn.Type = columnType;

			return this;
		}

		public SchemaBuilder WithProperty(ColumnProperty columnProperty)
		{
			_lastColumn.ColumnProperty = columnProperty;

			return this;
		}

		public SchemaBuilder WithSize(int size)
		{
			if (size == 0)
				throw new ArgumentNullException("size", "Size must be greater than zero");

			_lastColumn.Size = size;

			return this;
		}

		public SchemaBuilder WithDefaultValue(string defaultValue)
		{
			if (string.IsNullOrEmpty(defaultValue))
				throw new ArgumentNullException("defaultValue", "DefaultValue cannot be null or empty");

			_lastColumn.DefaultValue = defaultValue;

			return this;
		}

		public IForeignKeyOptions AsForeignKey()
		{
			_lastColumn.ColumnProperty = ColumnProperty.ForeignKey;

			return this;
		}

		public SchemaBuilder ReferencedTo(string primaryKeyTable, string primaryKeyColumn)
		{
			_lastColumn.Constraint = ForeignKeyConstraint.NoAction;
			//
			// Need to create ForeignKey column type to store primary table/column for FK
			//

			return this;
		}

		public SchemaBuilder WithConstraint(ForeignKeyConstraint action)
		{
			_lastColumn.Constraint = action;

			return this;
		}
	}
}

using System;
using System.Data;
using Migrator.Framework;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
  /// <summary>
  /// Base class for Provider tests for all non-constraint oriented tests.
  /// </summary>
  public class TransformationProviderBase
  {
    protected ITransformationProvider _provider;

    [TearDown]
    public virtual void TearDown()
    {	
      DropTestTables();

      _provider.Rollback();
    }

    protected void DropTestTables()
    {
      // Because MySql doesn't support schema transaction
      // we got to remove the tables manually... sad...
      _provider.RemoveTable("TestTwo");
      _provider.RemoveTable("Test");
      _provider.RemoveTable("SchemaInfo");
    }

    public void AddDefaultTable()
    {
      _provider.AddTable("TestTwo",
      new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
      new Column("TestId", DbType.Int32, ColumnProperty.ForeignKey)
      );
    }

    public void AddTable()
    {
      _provider.AddTable("Test",
      new Column("Id", DbType.Int32, ColumnProperty.NotNull),
      new Column("Title", DbType.String, 100, ColumnProperty.Null),
      new Column("name", DbType.String, 50, ColumnProperty.Null),
      new Column("blobVal", DbType.Binary, ColumnProperty.Null),
      new Column("boolVal", DbType.Boolean, ColumnProperty.Null),
      new Column("bigstring", DbType.String, 50000, ColumnProperty.Null)
      );
    }

    public void AddTableWithPrimaryKey()
    {
      _provider.AddTable("Test",
      new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
      new Column("Title", DbType.String, 100, ColumnProperty.Null),
      new Column("name", DbType.String, 50, ColumnProperty.NotNull),
      new Column("blobVal", DbType.Binary),
      new Column("boolVal", DbType.Boolean),
      new Column("bigstring", DbType.String, 50000)
      );
    }

    [Test]
    public void TableExistsWorks() 
    {
      Assert.IsFalse(_provider.TableExists("gadadadadseeqwe"));
      Assert.IsTrue(_provider.TableExists("TestTwo"));
    }

    [Test]
    public void ColumnExistsWorks() 
    {
      Assert.IsFalse(_provider.ColumnExists("gadadadadseeqwe", "eqweqeq"));
      Assert.IsFalse(_provider.ColumnExists("TestTwo", "eqweqeq"));
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "Id"));
    }

    [Test]
    public void CanExecuteBadSqlForNonCurrentProvider()
    {
      _provider["foo"].ExecuteNonQuery("select foo from bar 123");
    }

    [Test]
    public void TableCanBeAdded()
    {
      AddTable();
      Assert.IsTrue(_provider.TableExists("Test"));
    }

    [Test]
    public void GetTablesWorks() 
    {
      foreach (string name in _provider.GetTables())
      {
        _provider.Logger.Log("Table: {0}", name);
      }
      Assert.AreEqual(1, _provider.GetTables().Length);
      AddTable();
      Assert.AreEqual(2, _provider.GetTables().Length);
    }

    [Test]
    public void GetColumnsReturnsProperCount()
    {
      AddTable();
      Column[] cols = _provider.GetColumns("Test");
      Assert.IsNotNull(cols);
      Assert.AreEqual(6, cols.Length);
    }

    [Test]
    public void GetColumnsContainsProperNullInformation()
    {
      AddTableWithPrimaryKey();
      Column[] cols = _provider.GetColumns("Test");
      Assert.IsNotNull(cols);
      foreach (Column column in cols)
      {
        if (column.Name == "name")
        Assert.IsTrue((column.ColumnProperty & ColumnProperty.NotNull) == ColumnProperty.NotNull);
        else if (column.Name == "Title")
        Assert.IsTrue((column.ColumnProperty & ColumnProperty.Null) == ColumnProperty.Null);
      }
    }

    [Test]
    public void CanAddTableWithPrimaryKey()
    {
      AddTableWithPrimaryKey();
      Assert.IsTrue(_provider.TableExists("Test"));
    }

    [Test]
    public void RemoveTable()
    {
      AddTable();
      _provider.RemoveTable("Test");
      Assert.IsFalse(_provider.TableExists("Test"));
    }

    [Test]
    public virtual void RenameTableThatExists()
    {
      AddTable();
      _provider.RenameTable("Test", "Test_Rename");

      Assert.IsTrue(_provider.TableExists("Test_Rename"));
      Assert.IsFalse(_provider.TableExists("Test"));
      _provider.RemoveTable("Test_Rename");
    }

    [Test]
    [ExpectedException(typeof(MigrationException))]
    public void RenameTableToExistingTable()
    {
      AddTable();
      _provider.RenameTable("Test", "TestTwo");
    }

    [Test]
    public void RenameColumnThatExists()
    {
      AddTable();
      _provider.RenameColumn("Test", "name", "name_rename");

      Assert.IsTrue(_provider.ColumnExists("Test", "name_rename"));
      Assert.IsFalse(_provider.ColumnExists("Test", "name"));
    }

    [Test]
    [ExpectedException(typeof(MigrationException))]
    public void RenameColumnToExistingColumn()
    {
      AddTable();
      _provider.RenameColumn("Test", "Title", "name");
    }

    [Test]
    public void RemoveUnexistingTable()
    {
      _provider.RemoveTable("abc");
    }

    [Test]
    public void AddColumn()
    {
      _provider.AddColumn("TestTwo", "Test", DbType.String, 50);
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "Test"));
    }

    [Test]
    public void ChangeColumn()
    {
      _provider.ChangeColumn("TestTwo", new Column("TestId", DbType.String, 50));
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestId"));
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "Not an Int val." });
    }

    [Test]
    public void AddDecimalColumn()
    {
      _provider.AddColumn("TestTwo", "TestDecimal", DbType.Decimal, 38);
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestDecimal"));
    }

    [Test]
    public void AddColumnWithDefault()
    {
      _provider.AddColumn("TestTwo", "TestWithDefault", DbType.Int32, 50, 0, 10);
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestWithDefault"));
    }

    [Test]
    public void AddColumnWithDefaultButNoSize()
    {
      _provider.AddColumn("TestTwo", "TestWithDefault", DbType.Int32, 10);
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestWithDefault"));


      _provider.AddColumn("TestTwo", "TestWithDefaultString", DbType.String, "'foo'");
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestWithDefaultString"));
    }

    [Test]
    public void AddBooleanColumnWithDefault()
    {
      _provider.AddColumn("TestTwo", "TestBoolean", DbType.Boolean, 0, 0, false);
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "TestBoolean"));
    }

    [Test]
    public void CanGetNullableFromProvider()
    {
      _provider.AddColumn("TestTwo", "NullableColumn", DbType.String, 30, ColumnProperty.Null);
      Column[] columns = _provider.GetColumns("TestTwo");
      foreach (Column column in columns)
      {
        if (column.Name == "NullableColumn")
        {
          Assert.IsTrue((column.ColumnProperty & ColumnProperty.Null) == ColumnProperty.Null);
        }
      }
    }

    [Test]
    public void RemoveColumn()
    {
      AddColumn();
      _provider.RemoveColumn("TestTwo", "Test");
      Assert.IsFalse( _provider.ColumnExists( "TestTwo", "Test" ) );
    }

    [Test]
    public void RemoveColumnWithDefault()
    {
      AddColumnWithDefault();
      _provider.RemoveColumn("TestTwo", "TestWithDefault");
      Assert.IsFalse(_provider.ColumnExists("TestTwo", "TestWithDefault"));
    }

    [Test]
    public void RemoveUnexistingColumn()
    {
      _provider.RemoveColumn("TestTwo", "abc");
      _provider.RemoveColumn("abc", "abc");
    }

    /// <summary>
    /// Supprimer une colonne bit causait une erreur à cause
    /// de la valeur par défaut.
    /// </summary>
    [Test]
    public void RemoveBoolColumn()
    {
      AddTable();
      _provider.AddColumn("Test", "Inactif", DbType.Boolean);
      Assert.IsTrue(_provider.ColumnExists("Test", "Inactif"));

      _provider.RemoveColumn("Test", "Inactif");
      Assert.IsFalse(_provider.ColumnExists("Test", "Inactif"));
    }

    [Test]
    public void HasColumn()
    {
      AddColumn();
      Assert.IsTrue(_provider.ColumnExists("TestTwo", "Test"));
      Assert.IsFalse(_provider.ColumnExists("TestTwo", "TestPasLa"));
    }

    [Test]
    public void HasTable()
    {
      Assert.IsTrue(_provider.TableExists("TestTwo"));
    }

    [Test]
    public void AppliedMigrations()
    {
      Assert.IsFalse(_provider.TableExists("SchemaInfo"));

      // Check that a "get" call works on the first run.
      Assert.AreEqual(0, _provider.AppliedMigrations.Count);
      Assert.IsTrue(_provider.TableExists("SchemaInfo"), "No SchemaInfo table created");

      // Check that a "set" called after the first run works.
      _provider.MigrationApplied(1);
      Assert.AreEqual(1, _provider.AppliedMigrations[0]);

      _provider.RemoveTable("SchemaInfo");
      // Check that a "set" call works on the first run.
      _provider.MigrationApplied(1);
      Assert.AreEqual(1, _provider.AppliedMigrations[0]);
      Assert.IsTrue(_provider.TableExists("SchemaInfo"), "No SchemaInfo table created");
    }

    /// <summary>
    /// Reproduce bug reported by Luke Melia & Daniel Berlinger :
    /// http://macournoyer.wordpress.com/2006/10/15/migrate-nant-task/#comment-113
    /// </summary>
    [Test]
    public void CommitTwice()
    {
      _provider.Commit();
      Assert.AreEqual(0, _provider.AppliedMigrations.Count);
      _provider.Commit();
    }

    [Test]
    public void InsertData()
    {
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "1" });
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "2" });
      using (IDataReader reader = _provider.Select("TestId", "TestTwo")) 
      {
        int[] vals = GetVals(reader);

        Assert.IsTrue(Array.Exists(vals, delegate(int val){ return val == 1; }));
        Assert.IsTrue(Array.Exists(vals, delegate(int val){ return val == 2; }));
      }
    }
    
    [Test]
    public void CanInsertNullData()
    {
      AddTable();
      _provider.Insert("Test", new string[] { "Id", "Title" }, new string[] { "1", "foo" });
      _provider.Insert("Test", new string[] { "Id", "Title" }, new string[] { "2", null });
      using (IDataReader reader = _provider.Select("Title", "Test")) 
      {
        string[] vals = GetStringVals(reader);

        Assert.IsTrue(Array.Exists(vals, delegate(string val){ return val == "foo"; }));
        Assert.IsTrue(Array.Exists(vals, delegate(string val){ return val == null; }));
      }
    }

    [Test]
    public void CanInsertDataWithSingleQuotes()
    {
      AddTable();
      _provider.Insert("Test", new string[] {"Id", "Title"}, new string[] {"1", "Muad'Dib"});
      using (IDataReader reader = _provider.Select("Title", "Test"))
      {
        Assert.IsTrue(reader.Read());
        Assert.AreEqual("Muad'Dib", reader.GetString(0));
        Assert.IsFalse(reader.Read());
      }
    }

    [Test]
    public void DeleteData()
    {
      InsertData();
      _provider.Delete("TestTwo", "TestId", "1");

      using (IDataReader reader = _provider.Select("TestId", "TestTwo"))
      {
        Assert.IsTrue(reader.Read());
        Assert.AreEqual(2, Convert.ToInt32(reader[0]));
        Assert.IsFalse(reader.Read());
      }
    }

    [Test]
    public void DeleteDataWithArrays()
    {
      InsertData();
      _provider.Delete("TestTwo", new string[] {"TestId"}, new string[] {"1"});

      using (IDataReader reader = _provider.Select("TestId", "TestTwo"))
      {
        Assert.IsTrue(reader.Read());
        Assert.AreEqual(2, Convert.ToInt32(reader[0]));
        Assert.IsFalse(reader.Read());
      }
    }

    [Test]
    public void UpdateData()
    {            
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "1" });
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "2" });

      _provider.Update("TestTwo", new string[] { "TestId" }, new string[] { "3" });

      using (IDataReader reader = _provider.Select("TestId", "TestTwo"))
      {
        int[] vals = GetVals(reader);

        Assert.IsTrue(Array.Exists(vals, delegate(int val){ return val == 3; }));
        Assert.IsFalse(Array.Exists(vals, delegate(int val){ return val == 1; }));
        Assert.IsFalse(Array.Exists(vals, delegate(int val){ return val == 2; }));
      }
    }

    [Test]
    public void CanUpdateWithNullData()
    {
      AddTable();
      _provider.Insert("Test", new string[] { "Id", "Title" }, new string[] { "1", "foo" });
      _provider.Insert("Test", new string[] { "Id", "Title" }, new string[] { "2", null });
      
      _provider.Update("Test", new string[] { "Title" }, new string[] { null });
      
      using (IDataReader reader = _provider.Select("Title", "Test")) 
      {
        string[] vals = GetStringVals(reader);

        Assert.IsNull(vals[0]);
        Assert.IsNull(vals[1]);
      }
    }

    [Test]
    public void UpdateDataWithWhere()
    {
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "1" });
      _provider.Insert("TestTwo", new string[] { "TestId" }, new string[] { "2" });

      _provider.Update("TestTwo", new string[] { "TestId" }, new string[] { "3" }, "TestId='1'");

      using (IDataReader reader = _provider.Select("TestId", "TestTwo")) 
      {
        int[] vals = GetVals(reader);

        Assert.IsTrue(Array.Exists(vals, delegate(int val){ return val == 3; }));
        Assert.IsTrue(Array.Exists(vals, delegate(int val){ return val == 2; }));
        Assert.IsFalse(Array.Exists(vals, delegate(int val){ return val == 1; }));
      }
    }

    private int[] GetVals(IDataReader reader)
    {
      int[] vals = new int[2];
      Assert.IsTrue(reader.Read());
      vals[0] = Convert.ToInt32(reader[0]);
      Assert.IsTrue(reader.Read());
      vals[1] = Convert.ToInt32(reader[0]);
      return vals;
    }
    
    private string[] GetStringVals(IDataReader reader)
    {
      string[] vals = new string[2];
      Assert.IsTrue(reader.Read());
      vals[0] = reader[0] as string;
      Assert.IsTrue(reader.Read());
      vals[1] = reader[0] as string;
      return vals;
    }
  }
}

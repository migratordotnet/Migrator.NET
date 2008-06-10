using System;
using System.Data;
using Migrator.Framework;
using Migrator.Providers;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    /// <summary>
    /// Base class for Provider tests for all non-constraint oriented tests.
    /// </summary>
    public class TransformationProviderBase
    {
        protected TransformationProvider _provider;
		
        [TearDown]
        public void TearDown()
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
        public void AddTable()
        {
            _provider.AddTable("Test",
                               new Column("Id", DbType.Int32, ColumnProperty.NotNull),
                               new Column("Title", DbType.String, 100),
                               new Column("name", DbType.String, 50), 
                               new Column("blobVal", DbType.Binary),
                               new Column("boolVal", DbType.Boolean),
                               new Column("bigstring", DbType.String, 50000)
                );
            Assert.IsTrue(_provider.TableExists("Test"));
        }
        
        [Test]
		public void GetTablesWorks() 
		{
		    foreach (string name in _provider.GetTables())
		    {
		        Console.Out.WriteLine("Table: " +name);
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

        [Test, Ignore("FIXME: This is not working for MySQL for some reason")]
        public void GetColumnsContainsProperNullInformation()
        {
            // FIXME: The query appears to work just fine when run externally.

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
        public void RenameTableThatExists()
        {
            AddTable();
            _provider.RenameTable("Test", "Test_Rename");
            
            Assert.IsTrue(_provider.TableExists("Test_Rename"));
            Assert.IsFalse(_provider.TableExists("Test"));
            _provider.RemoveTable("Test_Rename");
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
        public void CurrentVersion()
        {
            Assert.IsFalse(_provider.TableExists("SchemaInfo"));
            
            // Check that a "get" call works on the first run.
            Assert.AreEqual(0, _provider.CurrentVersion);
            Assert.IsTrue(_provider.TableExists("SchemaInfo"), "No SchemaInfo table created");
            
            // Check that a "set" called after the first run works.
            _provider.CurrentVersion = 1;
            Assert.AreEqual(1, _provider.CurrentVersion);
            
            _provider.RemoveTable("SchemaInfo");
            // Check that a "set" call works on the first run.
            _provider.CurrentVersion = 1;
            Assert.AreEqual(1, _provider.CurrentVersion);
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
            Assert.AreEqual(0, _provider.CurrentVersion);
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
    }
}

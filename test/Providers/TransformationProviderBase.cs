using NUnit.Framework;

namespace Migrator.Providers.Tests
{
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
			_provider.RemoveTable("Test2");
			_provider.RemoveTable("Test");
			_provider.RemoveTable("SchemaInfo");
		}
		
		[Test]
		public void AddTable()
		{
			_provider.AddTable("Test",
			                   new Column("Id", typeof(int), ColumnProperties.NotNull),
			                   new Column("Title", typeof(string), 100)
				);
			Assert.IsTrue(_provider.TableExists("Test"));
		}

		[Test]
		public void AddTableWithPrimaryKey()
		{
			_provider.AddTable("Test",
			                   new Column("Id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity),
			                   new Column("Title", typeof(string), 100)
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
		public void RemoveUnexistingTable()
		{
			_provider.RemoveTable("abc");
		}

		[Test]
		public void AddPrimaryKey()
		{
			AddTable();
			_provider.AddPrimaryKey("PK_Test", "Test", "Id");
		}

		[Test]
		public void AddForeignKey()
		{
			AddTableWithPrimaryKey();
			_provider.AddForeignKey("FK_Test_Test2", "Test2", "TestId", "Test", "Id");
			Assert.IsTrue( _provider.ConstraintExists( "FK_Test_Test2", "Test2" ) );
		}

		[Test]
		public void RemoveForeignKey()
		{
			AddForeignKey();
			_provider.RemoveForeignKey("FK_Test_Test2", "Test2");
			Assert.IsFalse( _provider.ConstraintExists( "FK_Test_Test2", "Test2" ) );
		}

		[Test]
		public void RemoveUnexistingForeignKey()
		{
			AddForeignKey();
			_provider.RemoveForeignKey("FK_Test_Test2", "abc");
			_provider.RemoveForeignKey("abc", "abc");
			_provider.RemoveForeignKey("abc", "Test");
		}

		[Test]
		public void ConstraintExist()
		{
			AddForeignKey();
			Assert.IsTrue(_provider.ConstraintExists("FK_Test_Test2", "Test2"));
			Assert.IsFalse(_provider.ConstraintExists("abc", "abc"));
		}

		[Test]
		public void AddColumn()
		{
			_provider.AddColumn("Test2", "Test", typeof(string), 50);
			Assert.IsTrue( _provider.ColumnExists( "Test2", "Test" ) );
		}

		[Test]
		public void RemoveColumn()
		{
			AddColumn();
			_provider.RemoveColumn("Test2", "Test");
			Assert.IsFalse( _provider.ColumnExists( "Test2", "Test" ) );
		}

		[Test]
		public void RemoveUnexistingColumn()
		{
			_provider.RemoveColumn("Test2", "abc");
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
			_provider.AddColumn("Test", "Inactif", typeof(bool));
			Assert.IsTrue(_provider.ColumnExists("Test", "Inactif"));
			
			_provider.RemoveColumn("Test", "Inactif");
			Assert.IsFalse(_provider.ColumnExists("Test", "Inactif"));
		}

		[Test]
		public void HasColumn()
		{
			AddColumn();
			Assert.IsTrue(_provider.ColumnExists("Test2", "Test"));
			Assert.IsFalse(_provider.ColumnExists("Test2", "TestPasLa"));
		}

		[Test]
		public void HasTable()
		{
			Assert.IsTrue(_provider.TableExists("Test2"));
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
	}
}
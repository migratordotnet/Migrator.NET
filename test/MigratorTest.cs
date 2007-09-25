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
using System.Collections;
using NUnit.Framework;
using NMock;
using Migrator.Providers;

namespace Migrator.Tests
{
	[TestFixture]
	public class MigratorTest
	{
		private Migrator _migrator;
		
		// Collections qui vont contenir les # de versions
		// des migrations invoquées après un appel au migrateur.
		private static ArrayList _upCalled = new ArrayList();
		private static ArrayList _downCalled = new ArrayList();
		
		[SetUp]
		public void SetUp()
		{
			SetUpCurrentVersion(0);
		}
		
		[Test]
		public void MigrateUpward()
		{
			SetUpCurrentVersion(1);
			_migrator.MigrateTo(3);
			
			Assert.AreEqual(1, _migrator.CurrentVersion);
			
			Assert.AreEqual(2, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(2, _upCalled[0]);
			Assert.AreEqual(3, _upCalled[1]);
		}
		
		[Test]
		public void MigrateBackward()
		{
			SetUpCurrentVersion(3);
			_migrator.MigrateTo(1);
						
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(2, _downCalled.Count);
			
			Assert.AreEqual(3, _downCalled[0]);
			Assert.AreEqual(2, _downCalled[1]);
		}
		
		[Test]
		public void MigrateUpwardWithRollback()
		{
			SetUpCurrentVersion(3, true);
			
			try
			{
				_migrator.MigrateTo(6);
				Assert.Fail("La migration 5 devrait lancer une exception");
			}
			catch (Exception) {}
			
			Assert.AreEqual(1, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(4, _upCalled[0]);
		}
		
		[Test]
		public void MigrateDownwardWithRollback()
		{
			SetUpCurrentVersion(6, true);
			
			try
			{
				_migrator.MigrateTo(3);
				Assert.Fail("La migration 5 devrait lancer une exception");
			}
			catch (Exception) {}
			
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(1, _downCalled.Count);
			
			Assert.AreEqual(6, _downCalled[0]);
		}
		
		[Test]
		public void MigrateToCurrentVersion()
		{
			SetUpCurrentVersion(3);
			
			_migrator.MigrateTo(3);
			
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
		}
		
		[Test]
		public void LastVersion()
		{
			Assert.AreEqual(6, _migrator.LastVersion);
		}
		
		[Test]
		public void CurrentVersion()
		{
			SetUpCurrentVersion(4);
			Assert.AreEqual(4, _migrator.CurrentVersion);
			SetUpCurrentVersion(1);
			Assert.AreEqual(1, _migrator.CurrentVersion);
		}
		
		[Test]
		[ExpectedException(typeof(DuplicatedVersionException), "Migration version #1 is duplicated")]
		public void CheckForDuplicatedVersion()
		{
			_migrator.MigrationsTypes.Add(typeof(FirstMigration));
			_migrator.CheckForDuplicatedVersion();
		}
		
		[Test]
		public void ToHumanName()
		{
			Assert.AreEqual("Create a table", Migrator.ToHumanName("CreateATable"));
		}
		
		#region Helper methods and classes
		private void SetUpCurrentVersion(int version)
		{
			SetUpCurrentVersion(version, false);
		}
		private void SetUpCurrentVersion(int version, bool assertRollbackIsCalled)
		{
			DynamicMock providerMock = new DynamicMock(typeof(TransformationProvider));
			
			providerMock.SetupResult("CurrentVersion", version);
			if (assertRollbackIsCalled)
				providerMock.Expect("Rollback");
			else
				providerMock.ExpectNoCall("Rollback");
			
			_migrator = new Migrator((TransformationProvider) providerMock.MockInstance, null, true);
			
			// Enlève toutes les migrations trouvée automatiquement
			_migrator.MigrationsTypes.Clear();
			_upCalled.Clear();
			_downCalled.Clear();
			
			_migrator.MigrationsTypes.Add(typeof(FirstMigration));
			_migrator.MigrationsTypes.Add(typeof(SecondMigration));
			_migrator.MigrationsTypes.Add(typeof(ThirdMigration));
			_migrator.MigrationsTypes.Add(typeof(ForthMigration));
			_migrator.MigrationsTypes.Add(typeof(BadMigration));
			_migrator.MigrationsTypes.Add(typeof(SixthMigration));
		}
		
		private class AbstractTestMigration : Migration
		{
			override public void Up()
			{
				_upCalled.Add(Migrator.GetMigrationVersion(GetType()));
			}
			override public void Down()
			{
				_downCalled.Add(Migrator.GetMigrationVersion(GetType()));
			}
		}
		
		[Migration(1, Ignore=true)]
		private class FirstMigration : AbstractTestMigration {}
		[Migration(2, Ignore=true)]
		private class SecondMigration : AbstractTestMigration {}
		[Migration(3, Ignore=true)]
		private class ThirdMigration : AbstractTestMigration {}
		[Migration(4, Ignore=true)]
		private class ForthMigration : AbstractTestMigration {}
		[Migration(5, Ignore=true)]
		private class BadMigration : AbstractTestMigration {
			override public void Up()
			{
				throw new Exception("oh uh!");
			}
			override public void Down()
			{
				throw new Exception("oh uh!");
			}
		}
		[Migration(6, Ignore=true)]
		private class SixthMigration : AbstractTestMigration {}
		
		#endregion
	}
}

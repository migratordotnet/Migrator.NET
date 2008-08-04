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
using System.Collections.Generic;
using System.Reflection;
using Migrator.Framework;
using Migrator.Framework.Loggers;
using NUnit.Framework;
using NUnit.Mocks;

namespace Migrator.Tests
{
	[TestFixture]
	public class MigratorTestDates
	{
		private Migrator _migrator;
		
		// Collections that contain the version that are called migrating up and down
		private static readonly List<long> _upCalled = new List<long>();
		private static readonly List<long> _downCalled = new List<long>();
		
		[SetUp]
		public void SetUp()
		{
            SetUpCurrentVersion(0);
		}
		
		[Test]
		public void MigrateUpward()
		{
			SetUpCurrentVersion(2008010195);
			_migrator.MigrateTo(2008030195);
			
			Assert.AreEqual(2, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(2008020195, _upCalled[0]);
			Assert.AreEqual(2008030195, _upCalled[1]);
		}
		
		[Test]
		public void MigrateBackward()
		{
			SetUpCurrentVersion(2008030195);
			_migrator.MigrateTo(2008010195);
						
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(2, _downCalled.Count);
			
			Assert.AreEqual(2008030195, _downCalled[0]);
			Assert.AreEqual(2008020195, _downCalled[1]);
		}
		
		[Test]
		public void MigrateUpwardWithRollback()
		{
			SetUpCurrentVersion(2008030195, true);
			
			try
			{
				_migrator.MigrateTo(2008060195);
				Assert.Fail("La migration 5 devrait lancer une exception");
			}
			catch (Exception) {}
			
			Assert.AreEqual(1, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(2008040195, _upCalled[0]);
		}
		
		[Test]
		public void MigrateDownwardWithRollback()
		{
			SetUpCurrentVersion(2008060195, true);
			
			try
			{
				_migrator.MigrateTo(3);
				Assert.Fail("La migration 5 devrait lancer une exception");
			}
			catch (Exception) {}
			
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(1, _downCalled.Count);
			
			Assert.AreEqual(2008060195, _downCalled[0]);
		}
		
		[Test]
		public void MigrateToCurrentVersion()
		{
			SetUpCurrentVersion(2008030195);
			
			_migrator.MigrateTo(2008030195);
			
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
		}

        [Test]
        public void MigrateToLastVersion()
        {
            SetUpCurrentVersion(2008030195, false, false);

            _migrator.MigrateToLastVersion();

            Assert.AreEqual(2, _upCalled.Count);
            Assert.AreEqual(0, _downCalled.Count);
        }
		
        [Test]
        public void MigrateUpWithHoles()
        {
        	List<long> migs = new List<long>();
        	migs.Add(2008010195);
        	migs.Add(2008030195);
        	SetUpCurrentVersion(2008030195, migs,false, false);
        	_migrator.MigrateTo(2008040195);
        	
        	
			Assert.AreEqual(2, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(2008020195, _upCalled[0]);
			Assert.AreEqual(2008040195, _upCalled[1]);
        	
        }
		
        [Test]
        public void MigrateDownWithHoles()
        {
        	List<long> migs = new List<long>();
        	migs.Add(2008010195);
        	migs.Add(2008030195);
        	migs.Add(2008040195);
        	SetUpCurrentVersion(2008040195, migs,false, false);
        	_migrator.MigrateTo(2008030195);
        	
			Assert.AreEqual(1, _upCalled.Count);
			Assert.AreEqual(1, _downCalled.Count);
			
			Assert.AreEqual(2008020195, _upCalled[0]);
			Assert.AreEqual(2008040195, _downCalled[0]);
        	
        }
        
        [Test]
        public void PostMergeMigrateDown()
        {
        	// Assume trunk had versions 1 2 and 4.  A branch is merged with 3, then 
        	// rollback to version 2.  v3 should be untouched, and v4 should be rolled back
        	List<long> migs = new List<long>();
        	migs.Add(2008010195);
        	migs.Add(2008020195);
        	migs.Add(2008040195);
        	SetUpCurrentVersion(2008040195, migs,false, false);
        	_migrator.MigrateTo(2008020195);
        	
			Assert.AreEqual(0, _upCalled.Count);
			Assert.AreEqual(1, _downCalled.Count);
			
			Assert.AreEqual(2008040195, _downCalled[0]);
        	
        }
        
        [Test]
        public void PostMergeOldAndMigrateLatest()
        {
        	// Assume trunk had versions 1 2 and 4.  A branch is merged with 3, then 
        	// we migrate to Latest.  v3 should be applied and nothing else done.
        	List<long> migs = new List<long>();
        	migs.Add(2008010195);
        	migs.Add(2008020195);
        	migs.Add(2008040195);
        	SetUpCurrentVersion(2008040195, migs,false, false);
        	_migrator.MigrateTo(2008040195);
        	
			Assert.AreEqual(1, _upCalled.Count);
			Assert.AreEqual(0, _downCalled.Count);
			
			Assert.AreEqual(2008030195, _upCalled[0]);
        	
        }
        
        
        
		[Test]
		public void ToHumanName()
		{
            Assert.AreEqual("Create a table", StringUtils.ToHumanName("CreateATable"));
		}
		
		#region Helper methods and classes

        private void SetUpCurrentVersion(long version)
		{
			SetUpCurrentVersion(version, false);
		}

        private void SetUpCurrentVersion(long version, bool assertRollbackIsCalled)
        {
            SetUpCurrentVersion(version, assertRollbackIsCalled, true);
        }

        private void SetUpCurrentVersion(long version, bool assertRollbackIsCalled, bool includeBad)
        {
            List<long> appliedVersions = new List<long>();
            for (long i = 2008010195; i <= version; i+=10000){
            	appliedVersions.Add(i);
            }
            SetUpCurrentVersion(version, appliedVersions, assertRollbackIsCalled, includeBad);
        }
        
        private void SetUpCurrentVersion(long version, List<long> appliedVersions, bool assertRollbackIsCalled, bool includeBad)
		{
			DynamicMock providerMock = new DynamicMock(typeof(ITransformationProvider));

            providerMock.SetReturnValue("get_MaxVersion", version);
            providerMock.SetReturnValue("get_AppliedMigrations", appliedVersions);
            providerMock.SetReturnValue("get_Logger", new Logger(false));
			if (assertRollbackIsCalled)
				providerMock.Expect("Rollback");
			else
				providerMock.ExpectNoCall("Rollback");

            _migrator = new Migrator((ITransformationProvider)providerMock.MockInstance, Assembly.GetExecutingAssembly(), false);
			
			// Enlève toutes les migrations trouvée automatiquement
			_migrator.MigrationsTypes.Clear();
			_upCalled.Clear();
			_downCalled.Clear();
			
			_migrator.MigrationsTypes.Add(typeof(FirstMigration));
			_migrator.MigrationsTypes.Add(typeof(SecondMigration));
			_migrator.MigrationsTypes.Add(typeof(ThirdMigration));
			_migrator.MigrationsTypes.Add(typeof(FourthMigration));
            _migrator.MigrationsTypes.Add(typeof(SixthMigration));

			if (includeBad)
                _migrator.MigrationsTypes.Add(typeof(BadMigration));

		}
		
		public class AbstractTestMigration : Migration
		{
			override public void Up()
			{
				_upCalled.Add(MigrationLoader.GetMigrationVersion(GetType()));
			}
			override public void Down()
			{
                _downCalled.Add(MigrationLoader.GetMigrationVersion(GetType()));
			}
		}
		
		[Migration(2008010195, Ignore=true)]
		public class FirstMigration : AbstractTestMigration {}
		[Migration(2008020195, Ignore=true)]
        public class SecondMigration : AbstractTestMigration { }
		[Migration(2008030195, Ignore=true)]
        public class ThirdMigration : AbstractTestMigration { }
		[Migration(2008040195, Ignore=true)]
        public class FourthMigration : AbstractTestMigration { }
		
        [Migration(2008050195, Ignore=true)]
        public class BadMigration : AbstractTestMigration
        {
			override public void Up()
			{
				throw new Exception("oh uh!");
			}
			override public void Down()
			{
				throw new Exception("oh uh!");
			}
		}
		
        [Migration(2008060195, Ignore=true)]
        public class SixthMigration : AbstractTestMigration { }

        [Migration(2008070195)]
        public class NonIgnoredMigration : AbstractTestMigration { }

		#endregion
	}
}

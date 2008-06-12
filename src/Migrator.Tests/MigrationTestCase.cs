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
using System.Reflection;
using NUnit.Framework;
using Migrator.Providers;

namespace Migrator.Tests
{
	/// <summary>
	/// Extend this classe to test your migrations
	/// </summary>
	public abstract class MigrationsTestCase
	{
		private Migrator _migrator;
		
		protected abstract TransformationProvider TransformationProvider { get; }
		protected abstract string ConnectionString { get; }
		protected abstract Assembly MigrationAssembly { get; }
		
		[SetUp]
		public void SetUp()
		{			
            _migrator = new Migrator(TransformationProvider, MigrationAssembly, true);
			
			Assert.IsTrue(_migrator.MigrationsTypes.Count > 0, "No migrations in assembly " + MigrationAssembly.Location);
			
			_migrator.MigrateTo(0);
		}
		
		[TearDown]
		public void TearDown()
		{
			_migrator.MigrateTo(0);
		}
		
		[Test]
		public void Up()
		{
			_migrator.MigrateToLastVersion();
		}
		
		[Test]
		public void Down()
		{
			_migrator.MigrateToLastVersion();
			_migrator.MigrateTo(0);
		}
	}
}

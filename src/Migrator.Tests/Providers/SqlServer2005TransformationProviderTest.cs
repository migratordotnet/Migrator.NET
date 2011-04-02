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
using System.Configuration;
using System.Data;
using Migrator.Providers.SqlServer;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("SqlServer2005")]
    public class SqlServer2005TransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {
            string constr = ConfigurationManager.AppSettings["SqlServer2005ConnectionString"];
            if (constr == null)
                throw new ArgumentNullException("SqlServer2005ConnectionString", "No config file");

            _provider = new SqlServerTransformationProvider(new SqlServer2005Dialect(), constr);
            _provider.BeginTransaction();

            AddDefaultTable();
        }
        [TearDown]
        public override void TearDown()
        {
          base.TearDown();
        }
        [Test]
        public void RemoveDboColumnWithDefault()
        {
          AddDboColumnWithDefault();
          _provider.RemoveColumn("dbo.TestTwo", "TestWithDefault");
          Assert.IsFalse(_provider.ColumnExists("dbo.TestTwo", "TestWithDefault"));
        }

        [Test]
        public void AddDboColumnWithDefault()
        {
          _provider.AddColumn("dbo.TestTwo", "TestWithDefault", DbType.Int32, 50, 0, 10);
          Assert.IsTrue(_provider.ColumnExists("dbo.TestTwo", "TestWithDefault"));
        }
        [Test]
        public void ColumnExistsDbo()
        {
          _provider.AddColumn("TestTwo", "TestWithDefault", DbType.Int32, 50, 0, 10);
          Assert.IsTrue(_provider.ColumnExists("dbo.TestTwo", "TestWithDefault"));
        }
    }
}
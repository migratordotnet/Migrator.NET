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
using Migrator.Framework;
using Migrator.Providers.Mysql;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("MySql")]
    public class MySqlTransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {
            string constr = ConfigurationManager.AppSettings["MySqlConnectionString"];
            if (constr == null)
                throw new ArgumentNullException("MySqlConnectionString", "No config file");
            _provider = new MySqlTransformationProvider(new MysqlDialect(), constr);
            // _provider.Logger = new Logger(true, new ConsoleWriter());

            AddDefaultTable();
        }

        [TearDown]
        public override void TearDown()
        {
            DropTestTables();
        }

        [Test]
        public void AddTableWithMyISAMEngine()
        {
            _provider.AddTable("Test", "MyISAM",
                               new Column("Id", DbType.Int32, ColumnProperty.NotNull),
                               new Column("name", DbType.String, 50)
                );
        }

		// [Test,Ignore("MySql doesn't support check constraints")]
        public override void CanAddCheckConstraint() {}

    }
}
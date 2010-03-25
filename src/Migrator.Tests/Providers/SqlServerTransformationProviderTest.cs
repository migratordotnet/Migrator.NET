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
using Migrator.Providers;
using Migrator.Providers.SqlServer;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("SqlServer")]
    public class SqlServerTransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {
            string constr = ConfigurationManager.AppSettings["SqlServerConnectionString"];
            if (constr == null)
                throw new ArgumentNullException("SqlServerConnectionString", "No config file");

            _provider = new SqlServerTransformationProvider(new SqlServerDialect(), constr);
            _provider.BeginTransaction();

            AddDefaultTable();
        }

        [Test]
        public void QuoteCreatesProperFormat()
        {
            Dialect dialect = new SqlServerDialect();
            Assert.AreEqual("[foo]", dialect.Quote("foo"));
        }

        [Test]
        public void InstanceForProvider()
        {
            ITransformationProvider localProv = _provider["sqlserver"];
            Assert.IsTrue(localProv is SqlServerTransformationProvider);

            ITransformationProvider localProv2 = _provider["foo"];
            Assert.IsTrue(localProv2 is NoOpTransformationProvider);
        }
        
        [Test]
        public void ByteColumnWillBeCreatedAsBlob()
        {
            _provider.AddColumn("TestTwo", "BlobColumn", DbType.Byte);
            Assert.IsTrue(_provider.ColumnExists("TestTwo", "BlobColumn"));
        }

        [Test]
        public void TableExistsShouldWorkWithTableNamesWithBracket()
        {
            Assert.IsTrue(_provider.TableExists("[TestTwo]"));            
        }

        [Test]
        public void TableExistsShouldWorkWithSchemaNameAndTableName()
        {
            Assert.IsTrue(_provider.TableExists("dbo.TestTwo"));
        }
        
        [Test]
        public void TableExistsShouldWorkWithBracketsAndSchemaNameAndTableName()
        {
            Assert.IsTrue(_provider.TableExists("[dbo].[TestTwo]"));
        }
    }
}
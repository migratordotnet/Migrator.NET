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
using System.Data.SqlServerCe;
using Migrator.Providers.SqlServer;
using NUnit.Framework;
using System.IO;

namespace Migrator.Tests.Providers
{
    [TestFixture, Category("SqlServerCe")]
    public class SqlServerCeTransformationProviderTest : TransformationProviderConstraintBase
    {
        [SetUp]
        public void SetUp()
        {

            string constr = ConfigurationManager.AppSettings["SqlServerCeConnectionString"];
            if (constr == null)
                throw new ArgumentNullException("SqlServerCeConnectionString", "No config file");

			EnsureDatabase(constr);

            _provider = new SqlServerCeTransformationProvider(new SqlServerCeDialect(), constr);
            _provider.BeginTransaction();

            AddDefaultTable();
        }

        private void EnsureDatabase(string constr)
        {
            SqlCeConnection connection = new SqlCeConnection(constr);
            if (!File.Exists(connection.Database))
            {
                SqlCeEngine engine = new SqlCeEngine(constr);
                engine.CreateDatabase();
            }
        }

        // [Test,Ignore("SqlServerCe doesn't support check constraints")]
		public override void CanAddCheckConstraint() { }

		// [Test,Ignore("SqlServerCe doesn't support table renaming")]
		// see: http://www.pocketpcdn.com/articles/articles.php?&atb.set(c_id)=74&atb.set(a_id)=8145&atb.perform(details)=&
		public override void RenameTableThatExists() { }
    }
}

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
using NUnit.Framework;

namespace Migrator.Providers.Tests
{
	[TestFixture, Category("SqlServer")]
	public class SqlServerTransformationProviderTest : TransformationProviderBase
	{
		[SetUp]
		public void SetUp()
		{
#if DOTNET2
			string constr = ConfigurationManager.AppSettings["SqlServerConnectionString"];
#else
			string constr = ConfigurationSettings.AppSettings["SqlServerConnectionString"];
#endif
			if (constr == null)
				throw new ArgumentNullException("SqlServerConnectionString", "No config file");

			_provider = new SqlServerTransformationProvider(constr);
			_provider.BeginTransaction();
			
			_provider.AddTable("Test2",
			                   new Column("Id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity),
								new Column("TestId", typeof(int)));
		}
		
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using NUnit.Framework;

namespace Migrator.Providers.Tests
{
	[TestFixture, Category("Oracle")]
	public class OracleTransformationProviderTest : TransformationProviderBase
	{
		[SetUp]
		public void SetUp()
		{
#if DOTNET2
			string constr = ConfigurationManager.AppSettings["OracleConnectionString"];
#else
			string constr = ConfigurationSettings.AppSettings["MySqlConnectionString"];
#endif
			if (constr == null)
				throw new ArgumentNullException("OracleConnectionString", "No config file");

			_provider = new OracleTransformationProvider(constr);
			_provider.BeginTransaction();
			
			_provider.AddTable("Test2",
				new Column("Id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity),
				new Column("TestId", typeof(int), ColumnProperties.ForeignKey)
			);
		}
    }
}

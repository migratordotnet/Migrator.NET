using System;
using System.Configuration;

using NUnit.Framework;

using Migrator.Loggers;

namespace Migrator.Providers.Tests
{
	[TestFixture, Category("Postgre")]
	public class PostgreSQLTransformationProviderTest : TransformationProviderBase
	{
		[SetUp]
		public void Init()
		{
#if DOTNET2
			string constr = ConfigurationManager.AppSettings["NpgsqlConnectionString"];
#else
			string constr = ConfigurationSettings.AppSettings["NpgsqlConnectionString"];
#endif
			if (constr == null)
				throw new ArgumentNullException("ConnectionString", "No config file");

			_provider = new PostgreSQLTransformationProvider(constr);
			_provider.Logger = Logger.ConsoleLogger();
			_provider.BeginTransaction();
			
			_provider.AddTable("Test2",
			                   new Column("Id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity),
								new Column("TestId", typeof(int))
									);
		}

	}
}

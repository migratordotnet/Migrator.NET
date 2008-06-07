using System.Configuration;
using Migrator.Framework;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ProviderFactoryTest {
        
        [Test, Category("SqlServer")]
        public void CanLoad_SqlServerProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SqlServer", "Data Source=.;Initial Catalog=Test;Integrated Security=SSPI;");
            Assert.IsNotNull(provider);
        }
        
        [Test, Category("MySql")]
        public void CanLoad_MySqlProvider() 
        {
            ITransformationProvider provider = ProviderFactory.Create("MySql", ConfigurationManager.AppSettings["MySqlConnectionString"]);
            Assert.IsNotNull(provider);
        }
    }
}
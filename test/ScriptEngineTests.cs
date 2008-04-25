using System.IO;
using System.Reflection;
using Migrator.Compile;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ScriptEngineTests
    {
        [Test,Ignore("Test fails when run from IDE, but works with NAnt from command line.")]
        public void CanCompileAssemblies() 
        {
            ScriptEngine engine = new ScriptEngine();

            // This should let it work on windows or mono/unix I hope
            string dataPath = Path.Combine(Path.Combine("..", "test"), "Data");

            Assembly asm = engine.Compile(dataPath);
            Assert.IsNotNull(asm);

            MigrationLoader loader = new MigrationLoader(null, asm, false);
            Assert.AreEqual(2, loader.LastVersion);

            Assert.AreEqual(2, MigrationLoader.GetMigrationTypes(asm).Count);
        }
    }
}
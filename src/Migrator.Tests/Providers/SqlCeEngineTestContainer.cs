using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Text.RegularExpressions;

namespace Migrator.Tests.Providers
{
  internal class SqlCeEngineTestContainer:IDisposable
  {
    private SqlCeConnection connection;
    private SqlCeEngine engine;
    private string constr;

    public SqlCeEngineTestContainer(string constr)
    {
      this.constr = constr;
    }

    public void EnsureTemporaryDatabase()
    {
      if (null == connection) connection = new SqlCeConnection(constr);
      if (null == engine) engine = new SqlCeEngine(constr);

      if (!File.Exists(connection.Database))
      {
        engine.CreateDatabase();
      }
    }

    public void Dispose()
    {
      engine.Dispose();
      connection.Dispose();
      engine = null;
      connection = null;
      RemoveDataSource(constr);
    }

    public static void RemoveDataSource(string constr)
    {
      string file = Regex.Match(constr, "Data Source=(.*)").Groups[1].Value;
      if (File.Exists(file)) File.Delete(file);
    }
  }
}
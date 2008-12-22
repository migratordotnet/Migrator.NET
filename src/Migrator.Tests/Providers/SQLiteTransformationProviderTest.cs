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
using Migrator.Providers.SQLite;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
     [TestFixture, Category("SQLite")]
     public class SQLiteTransformationProviderTest : TransformationProviderBase
     {
         [SetUp]
         public void SetUp()
         {
             string constr = ConfigurationManager.AppSettings["SQLiteConnectionString"];
             if (constr == null)
                 throw new ArgumentNullException("SQLiteConnectionString", "No config file");

             _provider = new SQLiteTransformationProvider(new SQLiteDialect(), constr);
             _provider.BeginTransaction();
            
             AddDefaultTable();
         }

         [Test]
         public void CanParseSqlDefinitions() 
         {
             const string testSql = "CREATE TABLE bar ( id INTEGER PRIMARY KEY AUTOINCREMENT, bar TEXT, baz INTEGER NOT NULL )";
             string[] columns = ((SQLiteTransformationProvider) _provider).ParseSqlColumnDefs(testSql);
             Assert.IsNotNull(columns);
             Assert.AreEqual(3, columns.Length);
             Assert.AreEqual("id INTEGER PRIMARY KEY AUTOINCREMENT", columns[0]);
             Assert.AreEqual("bar TEXT", columns[1]);
             Assert.AreEqual("baz INTEGER NOT NULL", columns[2]);
         }
         
         [Test]
         public void CanParseSqlDefinitionsForColumnNames() 
         {
             const string testSql = "CREATE TABLE bar ( id INTEGER PRIMARY KEY AUTOINCREMENT, bar TEXT, baz INTEGER NOT NULL )";
             string[] columns = ((SQLiteTransformationProvider) _provider).ParseSqlForColumnNames(testSql);
             Assert.IsNotNull(columns);
             Assert.AreEqual(3, columns.Length);
             Assert.AreEqual("id", columns[0]);
             Assert.AreEqual("bar", columns[1]);
             Assert.AreEqual("baz", columns[2]);
         }

         [Test]
         public void CanParseColumnDefForNotNull()
         {
             const string nullString = "bar TEXT";
             const string notNullString = "baz INTEGER NOT NULL";
             Assert.IsTrue(((SQLiteTransformationProvider)_provider).IsNullable(nullString));
             Assert.IsFalse(((SQLiteTransformationProvider)_provider).IsNullable(notNullString));
         }

         [Test]
         public void CanParseColumnDefForName()
         {
             const string nullString = "bar TEXT";
             const string notNullString = "baz INTEGER NOT NULL";
             Assert.AreEqual("bar", ((SQLiteTransformationProvider)_provider).ExtractNameFromColumnDef(nullString));
             Assert.AreEqual("baz", ((SQLiteTransformationProvider)_provider).ExtractNameFromColumnDef(notNullString));
         }
     }
}
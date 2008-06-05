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

using System.Data;
using Migrator.Framework;
using Migrator.Providers;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
    /// <summary>
    /// Cette classe délègue les appels au gestionnaire de tranformations
    /// spécifié dans le constructeur et vérifie que chaque tranformation
    /// est réellement appliquée à la base de données.
    /// <para>
    /// Par exemple, lors de l'appel à la méthode AddColumn, l'ajout sera
    /// automatiquement vérifiée par un appel à la méthode ColumnExists.
    /// </para>
    /// <para>
    /// Cette classe est utilisée pour tester automatiquement les migrations.
    /// </para>
    /// </summary>
    public class TransformationProviderTestAdapter : TransformationProvider
    {
        readonly TransformationProvider _realProvider;

        public TransformationProviderTestAdapter(TransformationProvider realProvider) : base("")
        {
            _realProvider = realProvider;
        }
		
        public override void AddColumn(string table, string column, DbType type, int size, ColumnProperty property, object defaultValue)
        {
            _realProvider.AddColumn(table, column, type, size, property, defaultValue);
            Assert.IsTrue(_realProvider.ColumnExists(table, column),
                          string.Format("The column {0}.{1} failed to be created", table, column));
        }
		
        public override void RemoveColumn(string table, string column)
        {
            _realProvider.RemoveColumn(table, column);
            Assert.IsFalse(_realProvider.ColumnExists(table, column),
                           string.Format("The column {0}.{1} failed to be removed", table, column));
        }
		
        public override void RemoveTable(string name)
        {
            _realProvider.RemoveTable(name);
			
            Assert.IsFalse(_realProvider.TableExists(name));
        }
		
        public override void AddTable(string name, params Column[] columns)
        {
            _realProvider.AddTable(name, columns);

            Assert.IsTrue(_realProvider.TableExists(name));
            foreach (Column c in columns)
            {
                Assert.IsTrue(_realProvider.ColumnExists(name, c.Name),
                              string.Format("The column {0}.{1} failed to be created", name, c.Name));
            }
        }

        #region Simple delegate method

        public override int CurrentVersion {
            get {
                return _realProvider.CurrentVersion;
            }
            set {
                _realProvider.CurrentVersion = value;
            }
        }
		
        override public ILogger Logger
        {
            get { return _realProvider.Logger; }
            set { _realProvider.Logger = value; }
        }
		
        public override bool ColumnExists(string table, string column)
        {
            return _realProvider.ColumnExists(table, column);
        }
		
        public override bool TableExists(string table)
        {
            return _realProvider.TableExists(table);
        }
		
        public override void AddPrimaryKey(string name, string table, params string[] columns)
        {
            _realProvider.AddPrimaryKey(name, table, columns);
        }
		
        public override void AddForeignKey(string name, string primaryTable, string[] primaryColumns, string refTable, string[] refColumns)
        {
            _realProvider.AddForeignKey(name, primaryTable, primaryColumns, refTable, refColumns);
        }
		
        public override void RemoveForeignKey(string table, string name)
        {
            _realProvider.RemoveForeignKey(table, name);
        }
						
        public override bool ConstraintExists(string table, string name)
        {
            return _realProvider.ConstraintExists(table, name);
        }
		
        public override string[] GetTables()
        {
            return _realProvider.GetTables();
        }
		
        public override Column[] GetColumns(string table)
        {
            return _realProvider.GetColumns(table);
        }

        public override void AddTable(string name, string columns)
        {
            _realProvider.AddTable(name, columns);
        }

		#endregion
	}
}
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
using System.Collections.Generic;

namespace Migrator.Framework
{
    /// <summary>
    /// Describe a migration
    /// </summary>
    public class MigrationAttribute : Attribute
    {
        private string _scopeAssembly;
        private long _version;
        private bool _ignore = false;

        /// <summary>
        /// Describe the migration
        /// </summary>
        /// <param name="version">The unique version of the migration.</param>
        /// <param name="scopeAssembly">Scope assembly</param>
        public MigrationAttribute(long version)
            : this(version, null)
        {
        }

        public MigrationAttribute(long version, string scopeAssembly)
        {
            _scopeAssembly = scopeAssembly;
            Version = version;
        }

        /// <summary>
        /// The version reflected by the migration
        /// </summary>
        public long Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        private string GetScopeId(Type t)
        {
            if (_scopeAssembly != null && _scopeAssembly.Length == 0)
                return "00000000000000000000000000000000";
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bs = System.Text.Encoding.UTF8.GetBytes(_scopeAssembly ?? t.Assembly.GetName().Name);
            bs = x.ComputeHash(bs);
            var s = new System.Text.StringBuilder();
            foreach (var b in bs)
                s.Append(b.ToString("x2").ToUpper());
            return s.ToString();
        }

        private Type _vType;
        private KeyValuePair<string, long> _v;

        public KeyValuePair<string, long> GetVersion(Type t)
        {
            if (t != _vType)
            {
                _v = new KeyValuePair<string, long>(GetScopeId(t), Version);
                _vType = t;
            }
            return _v;
        }
 
        /// <summary>
        /// Set to <c>true</c> to ignore this migration.
        /// </summary>
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }
    }
}

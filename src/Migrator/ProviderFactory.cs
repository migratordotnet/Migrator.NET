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
using System.Reflection;
using Migrator.Framework;
using Migrator.Providers;

namespace Migrator
{
    /// <summary>
    /// Handles loading Provider implementations
    /// </summary>
    public class ProviderFactory
    {
        private static readonly Assembly providerAssembly;
        private static readonly Dictionary<string, Dialect> dialects = new Dictionary<string, Dialect>();

        static ProviderFactory()
        {
            providerAssembly = Assembly.GetAssembly(typeof(TransformationProvider));
            LoadDialects();
        }

        public static ITransformationProvider Create(string providerName, string connectionString)
        {
            var dialectInstance = DialectForProvider(providerName);

            return dialectInstance.NewProviderForDialect( connectionString );
        }

        public static Dialect DialectForProvider(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
                return null;

            foreach (string key in dialects.Keys)
            {
                if (0 < key.IndexOf(providerName, StringComparison.InvariantCultureIgnoreCase))
                    return dialects[key];
            }
            return null;
        }

        public static void LoadDialects()
        {
            Type dialectType = typeof (Dialect);
            foreach (Type t in providerAssembly.GetTypes())
            {
                if (t.IsSubclassOf(dialectType))
                {
                    dialects.Add(t.FullName, (Dialect) Activator.CreateInstance(t, null));
                }
            }
        }
    }
}

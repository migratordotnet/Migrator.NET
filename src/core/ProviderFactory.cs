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
using System.IO;
using System.Reflection;
using Migrator.Framework;

namespace Migrator
{
    /// <summary>
    /// Handles loading Provider implementations
    /// </summary>
    public class ProviderFactory
    {
        private static readonly Assembly providerAssembly;
        static ProviderFactory()
        {
            
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            string fullPath = Path.Combine(directory, "Migrator.Providers.dll");
            if (fullPath.StartsWith("file:\\"))
                fullPath = fullPath.Substring(6);
            else if (fullPath.StartsWith("file:"))
                fullPath = fullPath.Substring(5);
            providerAssembly = Assembly.LoadFrom(fullPath);
        }

        public static ITransformationProvider Create(string providerName, string connectionString)
        {
            return (ITransformationProvider)Activator.CreateInstance(
                                                providerAssembly.GetType(
                                                    String.Format(
                                                        "Migrator.Providers.{0}.{0}TransformationProvider", providerName), true, true),
                                                new object[] {connectionString});
        }
    }
}

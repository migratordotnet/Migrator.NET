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
using Migrator.Framework.Loggers;

namespace Migrator
{
    /// <summary>
    /// Migrations mediator.
    /// </summary>
    public class Migrator
    {
        private readonly ITransformationProvider _provider;

        private readonly MigrationLoader _migrationLoader;

        private readonly bool _trace; // show trace for debugging
        private ILogger _logger = new Logger(false);
        private string[] _args;

        public string[] args
        {
            get { return _args; }
            set { _args = value; }
        }

        public Migrator(string provider, string connectionString, Assembly migrationAssembly)
            : this(provider, connectionString, migrationAssembly, false)
        {
        }

        public Migrator(string provider, string connectionString, Assembly migrationAssembly, bool trace)
            : this(ProviderFactory.Create(provider, connectionString), migrationAssembly, trace)
        {
        }

        public Migrator(string provider, string connectionString, Assembly migrationAssembly, bool trace, ILogger logger)
            : this(ProviderFactory.Create(provider, connectionString), migrationAssembly, trace, logger)
        {
        }

        public Migrator(ITransformationProvider provider, Assembly migrationAssembly, bool trace)
            : this(provider, migrationAssembly, trace, new Logger(trace, new ConsoleWriter()))
        {
        }

        public Migrator(ITransformationProvider provider, Assembly migrationAssembly, bool trace, ILogger logger)
        {
            _provider = provider;
            _trace = trace;
            _logger = logger;
            
            _provider.Logger = logger;

            _migrationLoader = new MigrationLoader(provider, migrationAssembly, trace);

            _migrationLoader.CheckForDuplicatedVersion();
        }


        /// <summary>
        /// Returns registered migration <see cref="System.Type">types</see>.
        /// </summary>
        public List<Type> MigrationsTypes
        {
            get { return _migrationLoader.MigrationsTypes; }
        }

        /// <summary>
        /// Run all migrations up to the latest.
        /// </summary>
        public void MigrateToLastVersion()
        {
            MigrateTo(_migrationLoader.LastVersion);
        }

        /// <summary>
        /// Returns the current version of the database.
        /// </summary>
        public int CurrentVersion
        {
            get { return _provider.CurrentVersion; }
        }

        /// <summary>
        /// Get or set the event logger.
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// Migrate the database to a specific version.
        /// Runs all migration between the actual version and the
        /// specified version.
        /// If <c>version</c> is greater then the current version,
        /// the <c>Up()</c> method will be invoked.
        /// If <c>version</c> lower then the current version,
        /// the <c>Down()</c> method of previous migration will be invoked.
        /// </summary>
        /// <param name="version">The version that must became the current one</param>
        public void MigrateTo(int version)
        {

            if (_migrationLoader.MigrationsTypes.Count == 0)
            {
                _logger.Warn("No public classes with the Migration attribute were found.");
                return;
            }

            if (CurrentVersion == version) return;
            bool goingUp = CurrentVersion < version;

            bool firstRun = true;

            BaseMigrate migrate = BaseMigrate.GetInstance(goingUp, CurrentVersion, _provider, _logger);
            _logger.Started(migrate.Original, version);

            while (migrate.Continue(version))
            {
                Migration migration = _migrationLoader.GetMigration(migrate.Current);
                if (null == migration)
                {
                    _logger.Skipping(migrate.Current);
                    migrate.Iterate();
                    continue;
                }

                try
                {
                    if (firstRun)
                    {
                        migration.InitializeOnce(_args);
                        firstRun = false;
                    }

                    migrate.Migrate(migration);
                }
                catch (Exception ex)
                {
                    _logger.Exception(migrate.Current, migration.Name, ex);

                    // Oho! error! We rollback changes.
                    _logger.RollingBack(migrate.Previous);
                    _provider.CurrentVersion = migrate.Previous;
                    _provider.Rollback();

                    throw;
                }

                migrate.Iterate();
            }

            _logger.Finished(migrate.Original, version);
        }
    }
}

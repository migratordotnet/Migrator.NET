using System;
using System.Collections.Generic;
using Migrator.Framework;

namespace Migrator
{
    /// <summary>
    /// Description of MigrateAnywhere.
    /// </summary>
    public class MigrateAnywhere : BaseMigrate
    {
        private bool _goForward;

        public MigrateAnywhere(string scope, List<long> availableMigrations, ITransformationProvider provider, ILogger logger)
            : base(scope, availableMigrations, provider, logger)
        {
			_current = 0;
			if (provider.AppliedMigrations.Count > 0) {
			    foreach (var appliedMigration in provider.AppliedMigrations)
			    {
                    if (appliedMigration.Key != scope)
                        continue;
                    if (_current < appliedMigration.Value)
                        _current = appliedMigration.Value;
			    }
			}
			_goForward = false;
        }

        public override long Next
        {
            get
            {
                return _goForward
                           ? NextMigration()
                           : PreviousMigration();
            }
        }

        public override long Previous
        {
            get
            {
                return _goForward
                           ? PreviousMigration()
                           : NextMigration();
            }
        }

        public override bool Continue(long version)
        {
            // If we're going backwards and our current is less than the target, 
            // reverse direction.  Also, start over at zero to make sure we catch
            // any merged migrations that are less than the current target.
            if (!_goForward && version >= Current)
            {
                _goForward = true;
                Current = 0;
                Iterate();
            }

            // We always finish on going forward. So continue if we're still 
            // going backwards, or if there are no migrations left in the forward direction.
            return !_goForward || Current <= version;
        }

        public override void Migrate(IMigration migration)
        {
            _provider.BeginTransaction();
            MigrationAttribute attr = (MigrationAttribute)Attribute.GetCustomAttribute(migration.GetType(), typeof(MigrationAttribute));

            var v = attr.GetVersion(migration.GetType());
            var contains = false;

            foreach (var appliedMigration in _provider.AppliedMigrations)
            {
                if (appliedMigration.Key != v.Key || appliedMigration.Value != v.Value) continue;
                contains = true;
                break;
            }

            if (contains)
            {
            	RemoveMigration(migration, attr);
            } else {
            	ApplyMigration(migration, attr);
            }
        }

        private void ApplyMigration(IMigration migration, MigrationAttribute attr)
        {
            // we're adding this one
            _logger.MigrateUp(Current, migration.Name);
            if(! DryRun)
            {
                migration.Up();
                var v = attr.GetVersion(migration.GetType());
                _provider.MigrationApplied(v.Value, v.Key);
                _provider.Commit();
                migration.AfterUp();
            }
        }

        private void RemoveMigration(IMigration migration, MigrationAttribute attr)
        {
            // we're removing this one
            _logger.MigrateDown(Current, migration.Name);
            if (! DryRun)
            {
                migration.Down();
                var v = attr.GetVersion(migration.GetType());
                _provider.MigrationUnApplied(v.Value, v.Key);
                _provider.Commit();
                migration.AfterDown();
            }
        }
    }
}

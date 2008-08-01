using Migrator.Framework;

namespace Migrator
{
    
    /// <summary>
    /// Runs migrations from low to high. This is for applying newer changes and is the
    /// normal mode of operation.
    /// </summary>
    public class MigrateUp : BaseMigrate
    {
        public MigrateUp(long current, ITransformationProvider provider, ILogger logger)
            : base(current, provider, logger)
        {
        }

        public override long Previous
        {
            get { return Current - 1; }
        }

        public override long Next
        {
            get { return Current + 1; }
        }

        public override bool Continue(long targetVersion)
        {
            return Current <= targetVersion;
        }

        public override void Migrate(IMigration migration)
        {
            _provider.BeginTransaction();

            _logger.MigrateUp(Current, migration.Name);
            migration.Up();
            _provider.CurrentVersion = Current;
            _provider.Commit();
            migration.AfterUp();
        }
    }
}

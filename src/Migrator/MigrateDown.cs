using Migrator.Framework;

namespace Migrator
{
    /// <summary>
    /// Runs migrations from high to low. This is for rolling back to an older version.
    /// </summary>
    public class MigrateDown : BaseMigrate
    {
        public MigrateDown(long current, ITransformationProvider provider, ILogger logger)
            : base(current, provider, logger)
        {
        }

        public override long Previous
        {
            get { return Current + 1; }
        }

        public override long Next
        {
            get { return Current - 1; }
        }

        public override bool Continue(long targetVersion)
        {
            return targetVersion < Current;
        }

        public override void Migrate(IMigration migration)
        {
            _provider.BeginTransaction();

            _logger.MigrateDown(Current, migration.Name);
            migration.Down();
            _provider.CurrentVersion = Next;
            _provider.Commit();
            migration.AfterDown();
        }
    }
}

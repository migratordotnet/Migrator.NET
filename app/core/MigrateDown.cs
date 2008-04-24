using Migrator.Framework;

namespace Migrator
{
    /// <summary>
    /// Runs migrations from high to low. This is for rolling back to an older version.
    /// </summary>
    public class MigrateDown : BaseMigrate
    {
        public MigrateDown(int current, ITransformationProvider provider, ILogger logger)
            : base(current, provider, logger)
        {
        }

        public override int Previous
        {
            get { return Current + 1; }
        }

        public override int Next
        {
            get { return Current - 1; }
        }

        public override bool Continue(int targetVersion)
        {
            return targetVersion < Current;
        }

        public override void Migrate(Migration migration)
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

using Migrator.Framework;

namespace Migrator
{
    public abstract class BaseMigrate
    {
        protected readonly ITransformationProvider _provider;
        protected ILogger _logger;
        protected long _current;
        protected long _original;

        protected BaseMigrate(long current, ITransformationProvider provider, ILogger logger)
        {
            _provider = provider;
            _current = current;
            _original = current;
            _logger = logger;
        }

        public static BaseMigrate GetInstance(bool up, long current, ITransformationProvider provider, ILogger logger)
        {
            if (up)
            {
                return new MigrateUp(current + 1, provider, logger);
            }
            else
            {
                return new MigrateDown(current, provider, logger);
            }
        }

        public long Original
        {
            get { return _original; }
        }

        public virtual long Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        public abstract long Previous { get; }
        public abstract long Next { get; }

        public void Iterate()
        {
            Current = Next;
        }

        public abstract bool Continue(long targetVersion);

        public abstract void Migrate(IMigration migration);
    }
}

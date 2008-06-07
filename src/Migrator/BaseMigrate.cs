using Migrator.Framework;

namespace Migrator
{
    public abstract class BaseMigrate
    {
        protected readonly ITransformationProvider _provider;
        protected ILogger _logger;
        protected int _current;
        protected int _original;

        protected BaseMigrate(int current, ITransformationProvider provider, ILogger logger)
        {
            _provider = provider;
            _current = current;
            _original = current;
            _logger = logger;
        }

        public static BaseMigrate GetInstance(bool up, int current, ITransformationProvider provider, ILogger logger)
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

        public int Original
        {
            get { return _original; }
        }

        public virtual int Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        public abstract int Previous { get; }
        public abstract int Next { get; }

        public void Iterate()
        {
            Current = Next;
        }

        public abstract bool Continue(int targetVersion);

        public abstract void Migrate(Migration migration);
    }
}

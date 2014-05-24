using Migrator.Framework;
using System.Collections.Generic;

namespace Migrator
{
    public abstract class BaseMigrate
    {
        protected readonly ITransformationProvider _provider;
        protected ILogger _logger;
        protected List<long> _availableMigrations;
        protected List<long> _original;
        protected string _scope;
        protected long _current;
        protected bool _dryrun;

        protected BaseMigrate(string scope, List<long> availableMigrations, ITransformationProvider provider, ILogger logger)
        {
            _provider = provider;
            _availableMigrations = availableMigrations;
            _scope = scope;
            _original = new List<long>(); //clone
            foreach (var appliedMigration in _provider.AppliedMigrations)
            {
                if (scope == appliedMigration.Key)
                    _original.Add(appliedMigration.Value);
            }
            _original.Sort();
            _logger = logger;
        }

        public static BaseMigrate GetInstance(string scope, List<long> availableMigrations, ITransformationProvider provider, ILogger logger)
        {
        	return new MigrateAnywhere(scope, availableMigrations, provider, logger);
        }

        public List<long> AppliedVersions
        {
        	get { return _original; }
        }

        public virtual long Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        public virtual bool DryRun
        {
            get { return _dryrun; }
            set { _dryrun = value; }
        }

        public abstract long Previous { get; }
        public abstract long Next { get; }

        public void Iterate()
        {
            Current = Next;
        }

        public abstract bool Continue(long targetVersion);

        public abstract void Migrate(IMigration migration);
        
        /// <summary>
        /// Finds the next migration available to be applied.  Only returns
        /// migrations that have NOT already been applied.
        /// </summary>
        /// <returns>The migration number of the next available Migration.</returns>
        protected long NextMigration()
        {
        	// Start searching at the current index
        	int migrationSearch = _availableMigrations.IndexOf(Current)+1;
        	
        	// See if we can find a migration that matches the requirement
        	while(migrationSearch < _availableMigrations.Count
                  && AppliedMigrationsContains(_availableMigrations[migrationSearch]))
        	{
        		migrationSearch++;
        	}
        	
        	// did we exhaust the list?
        	if(migrationSearch == _availableMigrations.Count){
        		// we're at the last one.  Done!
        		return _availableMigrations[migrationSearch-1]+1;
        	}
        	// found one.
        	return _availableMigrations[migrationSearch];
        }
        
        /// <summary>
        /// Finds the previous migration that has been applied.  Only returns
        /// migrations that HAVE already been applied.
        /// </summary>
        /// <returns>The most recently applied Migration.</returns>
        protected long PreviousMigration()
        {
        	// Start searching at the current index
        	int migrationSearch = _availableMigrations.IndexOf(Current)-1;
        	
        	// See if we can find a migration that matches the requirement
        	while(migrationSearch > -1
                  && !AppliedMigrationsContains(_availableMigrations[migrationSearch]))
        	{
        		migrationSearch--;
        	}
        	
        	// did we exhaust the list?
        	if(migrationSearch < 0){
        		// we're at the first one.  Done!
        		return 0;
        	}
        	
        	// found one.
        	return _availableMigrations[migrationSearch];
        }

        private bool AppliedMigrationsContains(long version)
        {
            foreach (var appliedMigration in _provider.AppliedMigrations)
            {
                if (appliedMigration.Key == _scope && appliedMigration.Value == version)
                    return true;
            }
            return false;
        }
    }
}

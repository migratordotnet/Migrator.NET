/*
 * Created by SharpDevelop.
 * User: evonz
 * Date: 7/25/2008
 * Time: 10:06 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Migrator.Framework;
using System.Collections.Generic;

namespace Migrator
{
	/// <summary>
	/// Description of MigrateAnywhere.
	/// </summary>
	public class MigrateAnywhere : BaseMigrate
	{
		
		private bool _goForward;
		
		public MigrateAnywhere(List<long> availableMigrations, ITransformationProvider provider, ILogger logger)
            : base(availableMigrations, provider, logger)
        {
			_current = 0;
			if(_availableMigrations.Count > 0) {
				_current = provider.AppliedMigrations[provider.AppliedMigrations.Count - 1];
			}
			_goForward = false;
        }
		
		public override long Next{
			get{
				if(_goForward){
					return NextMigration();
				} else {
					return PreviousMigration();
				}
			}
		}
		
		public override long Previous{
			get{
				if(_goForward){
					return PreviousMigration();
				} else {
					return NextMigration();
				}
			}
		}
		
		public override bool Continue(long version){
			// If we're going backwards and our current is less than the target, 
			// reverse direction.  Also, start over at zero to make sure we catch
			// any merged migrations that are less than the current target.
			if(!_goForward && version >= Current) {
				_goForward = true;
				Current = 0;
				this.Iterate();
			}
			   
			// We always finish on going forward. So continue if we're still 
			// going backwards, or if there are no migrations left in the forward direction.
			return !_goForward || Current <= version;
		}

        public override void Migrate(IMigration migration)
        {
            _provider.BeginTransaction();
            MigrationAttribute attr = (MigrationAttribute)Attribute.GetCustomAttribute(migration.GetType(), typeof(MigrationAttribute));
            
            if(_provider.AppliedMigrations.Contains(attr.Version)) {
            	// we're removing this one
	            _logger.MigrateDown(Current, migration.Name);
	            migration.Down();
	            _provider.MigrationUnApplied(attr.Version);
	            _provider.Commit();
	            migration.AfterDown();
            } else {
            	// we're adding this one
	            _logger.MigrateUp(Current, migration.Name);
	            migration.Up();
	            _provider.MigrationApplied(attr.Version);
	            _provider.Commit();
	            migration.AfterUp();
            	
            }
        }
	}
}

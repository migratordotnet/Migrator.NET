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
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

using Migrator.Providers;
using Migrator.Loggers;

namespace Migrator
{	
	/// <summary>
	/// Migrations mediator.
	/// </summary>
	public class Migrator
	{
		private TransformationProvider _provider;
		private ArrayList _migrationsTypes = new ArrayList();
		private bool _trace;  // show trace for debugging
		private ILogger _logger = new NullLogger();
		private string[] _args;

		public string[] args
		{
			get { return _args; }
			set { _args = value; }
		}
	
		public Migrator(string provider, string connectionString, Assembly migrationAssembly, bool trace)
			: this(CreateProvider(provider, connectionString), migrationAssembly, trace)
		{}
		
		public Migrator(string provider, string connectionString, Assembly migrationAssembly)
			: this(CreateProvider(provider, connectionString), migrationAssembly, false)
		{}
		
		public Migrator(TransformationProvider provider, Assembly migrationAssembly, bool trace)
		{
			_provider = provider;
			_trace = trace;
			_logger = new ConsoleLogger(_trace);
			
			_migrationsTypes.AddRange(GetMigrationTypes(Assembly.GetExecutingAssembly()));
			if (migrationAssembly != null)
				_migrationsTypes.AddRange(GetMigrationTypes(migrationAssembly));
			
			if (_trace)
			{
				_logger.Trace("Loaded migrations:");
				foreach(Type t in _migrationsTypes)
				{
					_logger.Trace("{0} {1}", GetMigrationVersion(t).ToString().PadLeft(5), ToHumanName(t.Name));
				}
			}
			
			CheckForDuplicatedVersion();
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
			_provider.Logger = _logger;
			_provider.BeginTransaction();
			
			if (CurrentVersion == version) return;
			int originalVersion = CurrentVersion;
			bool goingUp = originalVersion < version;
			Migration migration;
			int v;	// the currently running migration number
			bool firstRun = true;

			if (goingUp)
			{
				// When we migrate to an upper version,
				// tranformations of the current version are
				// already applied, so we started at the next version.
				v = CurrentVersion+1;
			}
			else
			{
				v = CurrentVersion;
			}
			
			_logger.Started(originalVersion, version);
			
			while (true)
			{
				migration = GetMigration(v);

				if (firstRun)
				{
					migration.InitializeOnce(_args);
					firstRun = false;
				}

				if (migration != null)
				{
					string migrationName = ToHumanName(migration.GetType().Name);
					
					migration.TransformationProvider = _provider;
					
					try
					{
						if (goingUp)
						{
							_logger.MigrateUp(v, migrationName);
							migration.Up();
						}
						else
						{
							_logger.MigrateDown(v, migrationName);
							migration.Down();
						}
					}
					catch (Exception ex)
					{
						_logger.Exception(v, migrationName, ex);
						
						// Oho! error! We rollback changes.
						_logger.RollingBack(originalVersion);
						_provider.Rollback();
						
						throw ex;
					}
					
				}
				else
				{
					// The migration number is not found
					_logger.Skipping(v);
				}
				
				if (goingUp)
				{
					if (v == version)
						break;
					v++;
				}
				else
				{
					v--;
					// When we go back to previous versions
					// we don't invoke Down() of the current
					// version.
					if (v == version)
						break;
				}
			}
			
			// Update and commit all changes
			_provider.CurrentVersion = version;
			
			_provider.Commit();
			_logger.Finished(originalVersion, version);
		}
		
		/// <summary>
		/// Run all migrations up to the latest.
		/// </summary>
		public void MigrateToLastVersion()
		{
			MigrateTo(LastVersion);
		}
		
		/// <summary>
		/// Returns the last version of the migrations.
		/// </summary>
		public int LastVersion
		{
			get
			{
				if (_migrationsTypes.Count == 0)
					return 0;
				return GetMigrationVersion((Type) _migrationsTypes[_migrationsTypes.Count-1]);
			}
		}
		
		/// <summary>
		/// Returns the current version of the database.
		/// </summary>
		public int CurrentVersion
		{
			get
			{
				return _provider.CurrentVersion;
			}
		}
		
		/// <summary>
		/// Returns registered migration <see cref="System.Type">types</see>.
		/// </summary>
		public ArrayList MigrationsTypes
		{
			get
			{
				return _migrationsTypes;
			}
		}
		
		/// <summary>
		/// Get or set the event logger.
		/// </summary>
		public ILogger Logger {
			get {
				return _logger;
			}
			set {
				_logger = value;
			}
		}
		
		
		/// <summary>
		/// Returns the version of the migration
		/// <see cref="MigrationAttribute">MigrationAttribute</see>.
		/// </summary>
		/// <param name="t">Migration type.</param>
		/// <returns>Version number sepcified in the attribute</returns>
		public static int GetMigrationVersion(Type t)
		{
			MigrationAttribute attrib = (MigrationAttribute) 
					Attribute.GetCustomAttribute(t, typeof(MigrationAttribute));
			
			return attrib.Version;
		}
		
		/// <summary>
		/// Check for duplicated version in migrations.
		/// </summary>
		/// <exception cref="CheckForDuplicatedVersion">CheckForDuplicatedVersion</exception>
		public void CheckForDuplicatedVersion()
		{
			ArrayList versions = new ArrayList();
			
			foreach (Type t in _migrationsTypes)
			{
				int version = GetMigrationVersion(t);
				
				if (versions.Contains(version))
					throw new DuplicatedVersionException(version);
				
				versions.Add(version);
			}
		}
						
		/// <summary>
		/// Collect migrations in one <c>Assembly</c>.
		/// </summary>
		/// <param name="asm">The <c>Assembly</c> to browse.</param>
		/// <returns>The migrations collection</returns>
		public static ArrayList GetMigrationTypes(Assembly asm)
		{
			ArrayList migrations = new ArrayList();
			
			foreach (Type t in asm.GetTypes())
			{
				MigrationAttribute attrib = (MigrationAttribute) 
					Attribute.GetCustomAttribute(t, typeof(MigrationAttribute));
				if (attrib != null && typeof(Migration).IsAssignableFrom(t))
			    {
					if (!attrib.Ignore)
					{
						migrations.Add(t);
					}
			    }
			}
			
			migrations.Sort(new MigrationTypeComparer(true));
			
			return migrations;
		}
		
		/// <summary>
		/// Convert a classname to something more readable.
		/// ex.: CreateATable => Create a table
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static string ToHumanName(string className)
		{
			string name = Regex.Replace(className, "([A-Z])", " $1").Substring(1);
			return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
		}
		
		
		#region Helper methods
		private static TransformationProvider CreateProvider(string name, string constr)
		{
			return new ProviderFactory().Create(name, constr);
		}
				
		private Migration GetMigration(int version)
		{
			foreach (Type t in _migrationsTypes)
			{
				if (GetMigrationVersion(t) == version)
					return (Migration) Activator.CreateInstance(t);
			}
			
			return null;
		}
		
		#endregion
		
	}
}

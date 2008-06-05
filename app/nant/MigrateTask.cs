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
using System.IO;
using System.Reflection;
using Migrator.Compile;
using NAnt.Core;
using NAnt.Core.Attributes;
using Migrator.NAnt.Loggers;

namespace Migrator.NAnt
{
	/// <summary>
	/// Runs migrations on a database
	/// </summary>
	/// <example>
	/// <loadtasks assembly="…/Migrator.NAnt.dll" />
    /// <target name="migrate" description="Migrate the database" depends="build">
    ///  <property name="version" value="-1" overwrite="false" />
    ///  <migrate
    ///    provider="MySql"
    ///    connectionstring="Database=MyDB;Data Source=localhost;User Id=;Password=;"
    ///    migrations="bin/MyProject.dll"
    ///    to="${version}" />
    /// </target>
	/// </example>
	[TaskName("migrate")]
	public class MigrateTask : Task
	{
		private int _to = -1; // To last revision
		private string _provider;
		private string _connectionString;
		private FileInfo _migrationsAssembly;
		private bool _trace;

        private string _directory;
        private string _language;

		[TaskAttribute("provider", Required=true)]
		public string Provider
		{
			set { _provider = value; }
			get { return _provider; }
		}
		
		[TaskAttribute("connectionstring", Required=true)]
		public string ConnectionString
		{
			set { _connectionString = value; }
			get { return _connectionString; }
		}
		
		[TaskAttribute("migrations")]
		public FileInfo MigrationsAssembly
		{
			set { _migrationsAssembly = value; }
			get { return _migrationsAssembly; }
		}

        /// <summary>
        /// The paths to the directory that contains your migrations. 
        /// This will generally just be a single item.
        /// </summary>
        [TaskAttribute("directory")]
        public string Directory
        {
            set { _directory = value; }
            get { return _directory; }
        }

        [TaskAttribute("language")]
        public string Language
        {
            set { _language = value; }
            get { return _language; }
        }

		
		[TaskAttribute("to")]
		public int To
		{
			set { _to = value; }
			get { return _to; }
		}
		
		[TaskAttribute("trace")]
		public bool Trace
		{
			set { _trace = value; }
			get { return _trace; }
		}
		
		protected override void ExecuteTask()
		{
            if (! String.IsNullOrEmpty(Directory))
            {
                ScriptEngine engine = new ScriptEngine(Language, null);
                Execute(engine.Compile(Directory));
            }

            if (null != MigrationsAssembly)
            {
                Assembly asm = Assembly.LoadFrom(MigrationsAssembly.FullName);
                Execute(asm);
            }
		}

        private void Execute(Assembly asm)
        {
            Migrator mig = new Migrator(Provider, ConnectionString, asm, Trace, new TaskLogger(this));

            if (_to == -1)
                mig.MigrateToLastVersion();
            else
                mig.MigrateTo(_to);
        }
	}
}

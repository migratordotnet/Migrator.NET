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

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

using Migrator.NAnt.Loggers;

namespace Migrator.NAnt
{
	/// <summary>
	/// Runs migrations on a database
	/// </summary>
	[TaskName("migrate")]
	public class MigrateTask : Task
	{
		private int _to = -1; // To last revision
		private string _provider;
		private string _connectionString;
		private FileInfo _migrationsAssembly;
		private bool _trace;
		
		#region Attribute properties
		[TaskAttribute("provider", Required=true)]
		public string Provider
		{
			set
			{
				_provider = value;
			}
			get
			{
				return _provider;
			}
		}
		
		[TaskAttribute("connectionstring", Required=true)]
		public string ConnectionString
		{
			set
			{
				_connectionString = value;
			}
			get
			{
				return _connectionString;
			}
		}
		
		[TaskAttribute("migrations", Required=true)]
		public FileInfo MigrationsAssembly
		{
			set
			{
				_migrationsAssembly = value;
			}
			get
			{
				return _migrationsAssembly;
			}
		}
		
		[TaskAttribute("to")]
		public int To
		{
			set
			{
				_to = value;
			}
			get
			{
				return _to;
			}
		}
		
		[TaskAttribute("trace")]
		public bool Trace
		{
			set
			{
				_trace = value;
			}
			get
			{
				return _trace;
			}
		}
		#endregion
		
		protected override void ExecuteTask()
		{
			Assembly asm = Assembly.LoadFrom(_migrationsAssembly.FullName);
			
			Migrator mig = new Migrator(_provider, _connectionString, asm, _trace);
			mig.Logger = new TaskLogger(this);
			
			if (_to == -1)
				mig.MigrateToLastVersion();
			else
				mig.MigrateTo(_to);
		}
	}
}
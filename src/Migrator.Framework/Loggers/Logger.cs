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
using System.Collections.Generic;
using Migrator.Framework;

namespace Migrator.Framework.Loggers
{
	/// <summary>
	/// Text logger for the migration mediator
	/// </summary>
	public class Logger : IAttachableLogger
	{
		private readonly int _widthFirstColumn = 5;
		private readonly bool _trace = false;
		private readonly List<ILogWriter> _writers = new List<ILogWriter>();

		public Logger(bool trace)
		{
			_trace = trace;
		}

		public Logger(bool trace, params ILogWriter[] writers)
			: this(trace)
		{
			_writers.AddRange(writers);
		}

		public void Attach(ILogWriter writer)
		{
			_writers.Add(writer);
		}

		public void Detach(ILogWriter writer)
		{
			_writers.Remove(writer);
		}

		public void Started(long currentVersion, long finalVersion)
		{
			WriteLine("Current version : {0}.  Target version : {1}", currentVersion, finalVersion);
		}

		public void Started(List<long> currentVersions, long finalVersion)
		{
			WriteLine("Latest version applied : {0}.  Target version : {1}", LatestVersion(currentVersions), finalVersion);
		}

		public void MigrateUp(long version, string migrationName)
		{
			WriteLine("Applying {0}: {1}", version.ToString().PadLeft(_widthFirstColumn), migrationName);
		}

		public void MigrateDown(long version, string migrationName)
		{
			WriteLine("Removing {0}: {1}", version.ToString().PadLeft(_widthFirstColumn), migrationName);
		}

		public void Skipping(long version)
		{
			WriteLine("{0} {1}", version.ToString().PadLeft(_widthFirstColumn), "<Migration not found>");
		}

		public void RollingBack(long originalVersion)
		{
			WriteLine("Rolling back to migration {0}", originalVersion);
		}

        public void ApplyingDBChange(string sql)
	    {
	        Log(sql);
	    }

		public void Exception(long version, string migrationName, Exception ex)
		{
			WriteLine("{0} Error in migration {1} : {2}", "".PadLeft(_widthFirstColumn), version, ex.Message);
			if (_trace)
			{
				WriteLine("========= Error detail =========");
				WriteLine(ex.ToString());
				WriteLine(ex.StackTrace);
				Exception iex = ex.InnerException;
				while (iex != null)
				{
					WriteLine("Caused by: {0}", iex);
					WriteLine(ex.StackTrace);
					iex = iex.InnerException;
				}
				WriteLine("======================================");
			}
		}

		public void Finished(long originalVersion, long currentVersion)
		{
			WriteLine("Migrated to version {0}", currentVersion);
		}

		public void Finished(List<long> originalVersions, long currentVersion)
		{
			WriteLine("Migrated to version {0}", currentVersion);
		}

		public void Log(string format, params object[] args)
		{
			Write("{0} ", "".PadLeft(_widthFirstColumn));
			WriteLine(format, args);
		}

		public void Warn(string format, params object[] args)
		{
			Write("{0} Warning! : ", "".PadLeft(_widthFirstColumn));
			WriteLine(format, args);
		}

		public void Trace(string format, params object[] args)
		{
			if (_trace)
			{
				Log(format, args);
			}
		}

		private void Write(string message, params object[] args)
		{
			foreach (ILogWriter writer in _writers)
			{
				writer.Write(message, args);
			}
		}

		private void WriteLine(string message, params object[] args)
		{
			foreach (ILogWriter writer in _writers)
			{
				writer.WriteLine(message, args);
			}
		}

		public static ILogger ConsoleLogger()
		{
			return new Logger(false, new ConsoleWriter());
		}
		
		private string LatestVersion(List<long> versions){
			if(versions.Count > 0)
			{
				return versions[versions.Count - 1].ToString();
			}
			return "No migrations applied yet!";
		}
	}
}

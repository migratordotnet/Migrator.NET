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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger=Migrator.Framework.ILogger;

namespace Migrator.MSBuild.Logger
{
    /// <summary>
    /// MSBuild task logger for the migration mediator
    /// </summary>
    public class TaskLogger : ILogger
    {
        private readonly Task _task;

        public TaskLogger(Task task)
        {
            _task = task;
        }

        protected void LogInfo(string format, params object[] args)
        {
            _task.Log.LogMessage(format, args);
        }

        protected void LogError(string format, params object[] args)
        {
            _task.Log.LogError(format, args);
        }

        public void Started(long currentVersion, long finalVersion)
        {
            LogInfo("Current version : {0}", currentVersion);
        }

		public void Started(List<long> currentVersions, long finalVersion)
		{
			LogInfo("Latest version applied : {0}.  Target version : {1}", LatestVersion(currentVersions), finalVersion);
		}

		public void MigrateUp(long version, string migrationName)
		{
			LogInfo("Applying {0}: {1}", version.ToString(), migrationName);
		}

		public void MigrateDown(long version, string migrationName)
		{
			LogInfo("Removing {0}: {1}", version.ToString(), migrationName);
		}

        public void Skipping(long version)
        {
            MigrateUp(version, "<Migration not found>");
        }

        public void RollingBack(long originalVersion)
        {
            LogInfo("Rolling back to migration {0}", originalVersion);
        }

        public void ApplyingDBChange(string sql)
        {
            Log(sql);
        }

        public void Exception(long version, string migrationName, Exception ex)
        {
            LogInfo("============ Error Detail ============");
            LogInfo("Error in migration: {0}", version);
            _task.Log.LogErrorFromException(ex, true);
            LogInfo("======================================");
        }

        public void Exception(string message, Exception ex)
        {
            LogInfo("============ Error Detail ============");
            LogInfo("Error: {0}", message);
            _task.Log.LogErrorFromException(ex, true);
            LogInfo("======================================");
        }

        public void Finished(long originalVersion, long currentVersion)
        {
            LogInfo("Migrated to version {0}", currentVersion);
        }

        public void Finished(List<long> originalVersion, long currentVersion)
        {
            LogInfo("Migrated to version {0}", currentVersion);
        }

        public void Log(string format, params object[] args)
        {
            LogInfo(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _task.Log.LogWarning("[Warning] {0}", String.Format(format, args));
        }

        public void Trace(string format, params object[] args)
        {
            _task.Log.LogMessage(MessageImportance.Low, format, args);
        }
		
		private string LatestVersion(List<long> versions)
        {
			if(versions.Count > 0)
			{
				return versions[versions.Count - 1].ToString();
			}
			return "No migrations applied yet!";
		}
    }
}

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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger=Migrator.Framework.ILogger;

namespace Migrator.MSBuild.Logger
{
    /// <summary>
    /// NAnt task logger for the migration mediator
    /// </summary>
    public class TaskLogger : ILogger
    {
        private int _widthFirstColumn = 5;
        private Task _task;

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

        public void Started(int currentVersion, int finalVersion)
        {
            LogInfo("Current version : {0}", currentVersion);
        }

        public void MigrateUp(int version, string migrationName)
        {
            LogInfo("{0} {1}", version.ToString().PadLeft(_widthFirstColumn), migrationName);
        }

        public void MigrateDown(int version, string migrationName)
        {
            MigrateUp(version, migrationName);
        }

        public void Skipping(int version)
        {
            MigrateUp(version, "<Migration not found>");
        }

        public void RollingBack(int originalVersion)
        {
            LogInfo("Rolling back to migration {0}", originalVersion);
        }

        public void Exception(int version, string migrationName, Exception ex)
        {
            LogInfo("{0} Error in migration {1} : {2}", "".PadLeft(_widthFirstColumn), version, ex.Message);
            _task.Log.LogErrorFromException(ex, true);
        }

        public void Finished(int originalVersion, int currentVersion)
        {
            LogInfo("Migrated to version {0}", currentVersion);
        }

        public void Log(string format, params object[] args)
        {
            LogInfo("{0} {1}", "".PadLeft(_widthFirstColumn), String.Format(format, args));
        }

        public void Warn(string format, params object[] args)
        {
            _task.Log.LogWarning("{0} [Warning] {1}", "".PadLeft(_widthFirstColumn), String.Format(format, args));
        }

        public void Trace(string format, params object[] args)
        {
            _task.Log.LogMessage(MessageImportance.Low, "{0} {1}", "".PadLeft(_widthFirstColumn), String.Format(format, args));
        }
    }
}

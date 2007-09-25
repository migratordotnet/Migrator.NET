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

namespace Migrator.Loggers
{
	public class NullLogger : ILogger
	{
		public void Started(int currentVersion, int finalVersion) {}
		public void MigrateUp(int version, string migrationName) {}
		public void MigrateDown(int version, string migrationName) {}
		public void Skipping(int version) {}
		public void RollingBack(int originalVersion) {}
		public void Exception(int version, string migrationName, Exception ex) {}
		public void Finished(int originalVersion, int currentVersion) {}
		public void Log(string format, params object[] args) {}
		public void Warn(string format, params object[] args) {}
		public void Trace(string format, params object[] args) {}
	}
}
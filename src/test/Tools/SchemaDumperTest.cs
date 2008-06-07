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
using System.Configuration;
using NUnit.Framework;
using Migrator.Tools;

namespace Migrator.Tests.Tools
{
	[TestFixture, Category("MySql")]
	public class SchemaDumperTest
	{
		[Test]
		public void Dump()
		{

			string constr = ConfigurationManager.AppSettings["MySqlConnectionString"];

			if (constr == null)
				throw new ArgumentNullException("MySqlConnectionString", "No config file");
			
			SchemaDumper dumper = new SchemaDumper("MySql", constr);
			string output = dumper.Dump();
			
			Assert.IsNotNull(output);
		}
	}
}
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
using NUnit.Framework;

namespace Migrator.Tests
{
	[TestFixture]
	public class MigrationTypeComparerTest
	{
		private readonly Type[] _types = {
			typeof(Migration1),
			typeof(Migration2),
			typeof(Migration3)
		};
		
		[Test]
		public void SortAscending()
		{
            List<Type> list = new List<Type>();
			
			list.Add(_types[1]);
			list.Add(_types[0]);
			list.Add(_types[2]);
			
			list.Sort(new MigrationTypeComparer(true));
			
			for (int i = 0; i < 3; i++) {
				Assert.AreSame(_types[i], list[i]);
			}			
		}
		
		[Test]
		public void SortDescending()
		{
            List<Type> list = new List<Type>();
			
			list.Add(_types[1]);
			list.Add(_types[0]);
			list.Add(_types[2]);
			
			list.Sort(new MigrationTypeComparer(false));
			
			for (int i = 0; i < 3; i++) {
				Assert.AreSame(_types[2-i], list[i]);
			}			
		}
				
		[Migration(1, Ignore=true)]
		internal class Migration1 : Migration {
			override public void Up() {}
			override public void Down() {}
		}
		
        [Migration(2, Ignore=true)]
		internal class Migration2 : Migration {
			override public void Up() {}
			override public void Down() {}
		}

		[Migration(3, Ignore=true)]
		internal class Migration3 : Migration {
			override public void Up() {}
			override public void Down() {}
		}
	}
}

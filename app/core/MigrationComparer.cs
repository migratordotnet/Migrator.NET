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
using System.Collections;

namespace Migrator
{
	/// <summary>
	/// Comparer of Migration by their version attribute.
	/// </summary>
	public class MigrationTypeComparer : IComparer
	{
		private bool _ascending = true;
		
		public MigrationTypeComparer(bool ascending)
		{
			_ascending = ascending;
		}
				
		public int Compare(object x, object y)
		{
			MigrationAttribute attribOfX = (MigrationAttribute) Attribute.GetCustomAttribute((Type) x, typeof(MigrationAttribute));
			MigrationAttribute attribOfY = (MigrationAttribute) Attribute.GetCustomAttribute((Type) y, typeof(MigrationAttribute));
			
			if (_ascending)
				return attribOfX.Version - attribOfY.Version;
			else
				return attribOfY.Version - attribOfX.Version;
		}
	}
}

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

namespace Migrator
{
	/// <summary>
	/// Comparer of Migration by their version attribute.
	/// </summary>
	public class MigrationTypeComparer : IComparer<Type>
	{
		private readonly bool _ascending = true;
		
		public MigrationTypeComparer(bool ascending)
		{
			_ascending = ascending;
		}

        public int Compare(Type x, Type y)
		{
			MigrationAttribute attribOfX = (MigrationAttribute) Attribute.GetCustomAttribute(x, typeof(MigrationAttribute));
			MigrationAttribute attribOfY = (MigrationAttribute) Attribute.GetCustomAttribute(y, typeof(MigrationAttribute));

            var vX = attribOfX.GetVersion(x);
            var vY = attribOfY.GetVersion(y);

            var c = vX.Key.CompareTo(vY.Key);

            if (_ascending)
            {
                if (c != 0)
                    return c;
            }
            else
            {
                if (c != 0)
                    return -c;
            }

            return _ascending
                       ? attribOfX.Version.CompareTo(attribOfY.Version)
                       : attribOfY.Version.CompareTo(attribOfX.Version);


		}
	}
}

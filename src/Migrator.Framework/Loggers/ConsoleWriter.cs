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

namespace Migrator.Framework.Loggers
{
	public class ConsoleWriter : ILogWriter
	{
		public void Write(string message, params object[] args)
		{
            if (args == null || args.Length == 0)
            {
                Console.Write(message);
            }
            else
            {
                Console.Write(message, args);
            }
		}

		public void WriteLine(string message, params object[] args)
		{
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message, args);
            }
		}
	}
}

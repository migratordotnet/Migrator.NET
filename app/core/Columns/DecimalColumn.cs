using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Columns
{
	public class DecimalColumn: Column
	{
		private int _remainder;

		public DecimalColumn(string name, int size, int remainder)
			: base(name, typeof(decimal), size)
		{
			_remainder = remainder;
		}

		public DecimalColumn(string name, int size, int remainder, ColumnProperties property)
			:base(name, typeof(decimal), property)
		{
			_remainder = remainder;
		}

		public int Remainder
		{
			get { return _remainder; }
			set { _remainder = value; }
		}
	
	}
}

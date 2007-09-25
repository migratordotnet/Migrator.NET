using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ForeignKeys
{
	public abstract class ForeignKeyConstraintMapper: IForeignKeyConstraintMapper
	{
		#region IForeignKeyConstraintMapper Members

		public string Resolve(ForeignKeyConstraint constraint)
		{
			switch(constraint)
			{
				case ForeignKeyConstraint.Cascade:
					return Cascade;
				case ForeignKeyConstraint.Restrict:
					return Restrict;
				case ForeignKeyConstraint.SetDefault:
					return SetDefault;
				case ForeignKeyConstraint.SetNull:
					return SetNull;
				default:
					return NoAction;
			}
		}

		public abstract string Cascade { get;}

		public abstract string SetNull { get;}

		public abstract string NoAction { get;}

		public abstract string Restrict { get;}

		public abstract string SetDefault { get;}
		
		#endregion
	}
}
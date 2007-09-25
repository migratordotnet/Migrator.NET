using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ForeignKeys
{
	class SQLServerForeignKeyConstraintMapper: ForeignKeyConstraintMapper
	{

		//from MYSQL driver, delete if they are not correct
		public override string Cascade
		{
		    get { return "CASCADE"; }
		}

		public override string SetNull
		{
		    get { return "SET NULL"; }
		}

		public override string NoAction
		{
		    get { return "NO ACTION"; }
		}

		public override string Restrict
		{
		    get { return "RESTRICT"; }
		}

		public override string SetDefault
		{
		    get { return "SET DEFAULT"; }
		}
	}
}
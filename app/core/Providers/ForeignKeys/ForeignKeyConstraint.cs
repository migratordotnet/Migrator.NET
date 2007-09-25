using System;
using System.Collections.Generic;
using System.Text;

namespace Migrator.Providers.ForeignKeys
{
	public enum ForeignKeyConstraint
	{
		Cascade,
		SetNull,
		NoAction,
		Restrict,
		SetDefault
	}
}
namespace Migrator.Providers.ForeignKeys
{
	public interface IForeignKeyConstraintMapper
	{
		string Cascade { get;}
		string SetNull { get;}
		string NoAction { get;}
		string Restrict { get;}
		string SetDefault{ get;}

		string Resolve(ForeignKeyConstraint constraint);
	}
}
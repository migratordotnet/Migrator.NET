using Migrator.Framework;
using System.Data;

[Migration(2)]
public class AddAddressColumns : Migration
{
    override public void Up()
    {
        Database.AddColumn("Address", new Column("street2", DbType.String, 50));
        Database.AddColumn("Address", new Column("street3", DbType.String, 50));
    }

    override public void Down()
    {
        Database.RemoveColumn("Address", "street2");
        Database.RemoveColumn("Address", "street3");
    }
}
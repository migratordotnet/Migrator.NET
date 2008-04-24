using Migrator.Framework;
using System.Data;

[Migration(1)]
public class AddAddressTable : Migration
{
    override public void Up()
    {
        Database.AddTable("Address", 
            new Column("id", DbType.Int32, ColumnProperty.PrimaryKey),
            new Column("street", DbType.String, 50),
            new Column("city", DbType.String, 50),
            new Column("state", DbType.StringFixedLength, 2),
            new Column("postal_code", DbType.String, 10)
        );
    }
    override public void Down()
    {
        Database.RemoveTable("Address");
    }
}

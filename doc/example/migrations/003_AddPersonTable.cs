using Migrator.Framework;
using System.Data;

[Migration(3)]
public class AddPersonTable : Migration
{
    override public void Up()
    {
        Database.AddTable("Person", 
            new Column("id", DbType.Int32, ColumnProperty.PrimaryKey),
            new Column("first_name", DbType.String, 50),
            new Column("last_name", DbType.String, 50),
            new Column("address_id", DbType.Int32, ColumnProperty.Unsigned)
        );
        Database.AddForeignKey("FK_PERSON_ADDRESS", "Person", "address_id", "Address", "id");
    }
    override public void Down()
    {
        Database.RemoveTable("Person");
    }
}

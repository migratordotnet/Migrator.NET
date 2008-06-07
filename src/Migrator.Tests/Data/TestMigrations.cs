using Migrator.Framework;

namespace Migrator.Tests.Data
{
    [Migration(1)]
    public class FirstTestMigration : Migration
    {
        override public void Up()
        {
        }
        override public void Down()
        {
        }
    }

    [Migration(2)]
    public class SecondTestMigration : Migration
    {
        override public void Up()
        {
        }

        override public void Down()
        {
        }
    }
}
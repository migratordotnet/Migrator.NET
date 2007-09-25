# Migrator Readme file

See http://code.macournoyer.com/migrator for further infos

## Compiling
To build from source:
  nant build

To run tests:
  nant test

You should have mysql 4.0 or upper and MSDE (on win only) installed and setup
the following databases: Test on each engine.
To change these configuration see config\test.config.

## Usage
To use it:

1. add bin/Migrator.dll to you project references
2. create some class looking like this:

    using Migrator;
    [Migration(1)]
    public class MyMigration : Migration
    {
      public override void Up()
      {
        // Create stuff
      }
      public override void Down()
      {
        // Remove the same stuff
      }
    }

3. compile and run the console (Migrator.Console.exe) or better, use the migrator NAnt task:

    <loadtasks assembly=".../Migrator.NAnt.dll" />
    <target name="migrate" description="Migrate the database" depends="build">
      <property name="version" value="-1" overwrite="false" />
    	<migrate
    	  provider="MySql|PostgreSQL|SqlServer"
    	  connectionstring="Database=MyDB;Data Source=localhost;User Id=;Password=;"
    	  migrations="bin/MyProject.dll"
    	  to="${version}" />
    </target>

##  Other stuff...
Have fun!

[Web site](http://sourceforge.net/projects/nproject)
[Marc-André Cournoyer](macournoyer@yahoo.ca)

Released under MPL 1.1
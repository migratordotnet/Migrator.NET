= Migrator DotNet
Database Migrations implemented in .NET.
Supports rolling up and rolling back of migrations.

A way to integrate database change management into your regular development and automation processes.
The migrations themselves are implemented in code and can be mostly done in a database independent way.

Licensed under MPL 1.1 : http://www.mozilla.org/MPL/

== Supported Database
* MySQL (5.0, 5.1)
* PostgreSQL
* SQLite (tested on Mono)
* SQL Server (2000, 2005)
* SQL Server CE (3.5)

== Untested Databases but in there
* Oracle

== Supported Modes
* MSBuild Task
* NAnt Task
* Console Application


= Development

== Compiling
To build from source:
  nant build

== Testing
To run tests:
  nant test

You should have a database installed and setup:
* MySql
* SQL Server
* Oracle
* PostgreSQL
* or you can use SQLite with no setup
You can Test on each engine or change those by changing the 'exclude' properties in a nant build
file called 'local.properties'. To change the database connection strings see config\app.config. You
can make your own local version called 'local.config' to override these

== SQL Server CE
To use SQL Server CE, you will need the proper tools installed. The current DLL that we are testing
against is the 3.5 version.
As of this writing you can download the installer for the SQL CE Runtime at:
http://www.microsoft.com/downloads/details.aspx?&FamilyID=7849b34f-67ab-481f-a5a5-4990597b0297&DisplayLang=en

We have not confirmed if this will build on Mono yet. But it almost definitely won't run because SQL CE uses PInvoke
internally.

= Usage

1. Add bin/Migrator.Framework.dll to you project references
    - All of the other DLLs are only needed for actually running the migrations.
2. Create a class for your migration like:
    using Migrator.Framework;
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

3. Compile your migrations and run the console (Migrator.Console.exe) or use the migrator 
   NAnt or MSBuild tasks:

   NAnt:
    <loadtasks assembly=".../Migrator.NAnt.dll" />
    <target name="migrate" description="Migrate the database" depends="build">
      <property name="version" value="-1" overwrite="false" />
        <migrate
          provider="MySql|PostgreSQL|SqlServer"
          connectionstring="Database=MyDB;Data Source=localhost;User Id=;Password=;"
          migrations="bin/MyProject.dll"
          to="${version}" />
    </target>

    MSBuild:
    <PropertyGroup>
        <MigratorTasksPath>$(MSBuildProjectDirectory)\migrator</MigratorTasksPath>
    </PropertyGroup>

    <Import Project="$(MigratorTasksPath)\Migrator.Targets" />

    <Target name="Migrate" DependsOnTargets="Build">
        <Migrate Provider="SqlServer" 
            Connectionstring="Database=MyDB;Data Source=localhost;User Id=;Password=;" 
            Migrations="bin/MyProject.dll"/>
    </Target>

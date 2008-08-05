using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Migrator.Framework;

namespace Migrator.Web
{
	/// <summary>
	/// Web form that can be used to run migrations in a web project.
    /// It's recommended that you have some security in place.
	/// </summary>
	public class Default : Page
	{	

		protected Label _LatestVersion;
		protected DropDownList _availableVersions;
		protected Button _runMigration;

		protected void PageInit(object sender, EventArgs e)
		{
		}

		protected void PageExit(object sender, EventArgs e)
		{
		}


		private void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				this.BindForm();
			}
		}
		
		private void RunMigration(object sender, EventArgs e)
		{
			Migrator mig = GetMigrator();
			mig.MigrateTo(int.Parse(this._availableVersions.SelectedValue));
			this.BindForm();
		}
		
		
		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.Load	+= new System.EventHandler(Page_Load);
			this.Init   += new System.EventHandler(PageInit);
			this.Unload += new System.EventHandler(PageExit);
			
			this._runMigration.Click += new EventHandler(RunMigration);
		}
		
		private void BindForm(){
			Migrator mig = GetMigrator();
			List<long> appliedMigrations = mig.AppliedMigrations;
			long latestMigration = 0;
			if(appliedMigrations.Count > 0) {
				latestMigration = appliedMigrations[appliedMigrations.Count - 1];
			} 
			this._LatestVersion.Text = latestMigration.ToString();
			
			List<MigrationInfo> availableMigrations = GetMigrationsList(mig);
			this._availableVersions.DataSource = availableMigrations;
			this._availableVersions.DataValueField = "ID";
			this._availableVersions.DataTextField = "ClassName";
			this._availableVersions.DataBind();
		}
		
		private Migrator GetMigrator()
		{
			Assembly asm = Assembly.LoadFrom(ConfigurationManager.AppSettings["MigrationAsembly"]);
			string provider = ConfigurationManager.AppSettings["MigrationProvider"];
			string connectString = ConfigurationManager.AppSettings["ConnectionString"];
			
			Migrator migrator = new Migrator(provider, connectString, asm, false);
			return migrator;
		}
		
		private List<MigrationInfo> GetMigrationsList(Migrator mig)
        {
			List<System.Type> migrations = mig.MigrationsTypes;
			migrations.Reverse();
			List<MigrationInfo> list = new List<MigrationInfo>();
			List<System.Type>.Enumerator en = migrations.GetEnumerator();
			while(en.MoveNext()){
				MigrationInfo info = new MigrationInfo(en.Current);
				list.Add(info);
			}
			return list;
		}
		
		public class MigrationInfo
		{
			private Type _type;
			
			public MigrationInfo(Type type)
			{
				this._type = type;
			}
			
			public Type MigrationType
			{
				get{ return _type; }
			}
			
			public long ID
			{
				get{ return MigrationLoader.GetMigrationVersion(_type); }
			}
			
			public string ClassName
			{
				get{ return _type.ToString() + " (" + ID + ")"; }
			}
		}
	}
}

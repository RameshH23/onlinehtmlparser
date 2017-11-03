namespace OnlineHTMLParser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Intiial : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserProfile", "FirstName");
            DropColumn("dbo.UserProfile", "LastName");
            DropColumn("dbo.UserProfile", "Dob");
            DropColumn("dbo.UserProfile", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "Phone", c => c.String(maxLength: 100));
            AddColumn("dbo.UserProfile", "Dob", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserProfile", "LastName", c => c.String(maxLength: 100));
            AddColumn("dbo.UserProfile", "FirstName", c => c.String(maxLength: 100));
        }
    }
}

namespace PhotoShare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBackgroundPhoto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "BackgroundPhoto", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "BackgroundPhoto");
        }
    }
}

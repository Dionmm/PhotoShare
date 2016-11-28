namespace PhotoShare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photographerConfirmation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AwaitingAdminConfirmation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AwaitingAdminConfirmation");
        }
    }
}

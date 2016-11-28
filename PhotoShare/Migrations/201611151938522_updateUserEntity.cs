namespace PhotoShare.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class updateUserEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Name", c => c.String());
            AddColumn("dbo.Users", "ProfileDescription", c => c.String());
            AddColumn("dbo.Users", "ProfilePhoto", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ProfilePhoto");
            DropColumn("dbo.Users", "ProfileDescription");
            DropColumn("dbo.Users", "Name");
        }
    }
}

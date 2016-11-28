namespace PhotoShare.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PhotoRequiresUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "User_Id", "dbo.Users");
            DropIndex("dbo.Photos", new[] { "User_Id" });
            AlterColumn("dbo.Photos", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Photos", "User_Id");
            AddForeignKey("dbo.Photos", "User_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "User_Id", "dbo.Users");
            DropIndex("dbo.Photos", new[] { "User_Id" });
            AlterColumn("dbo.Photos", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Photos", "User_Id");
            AddForeignKey("dbo.Photos", "User_Id", "dbo.Users", "Id");
        }
    }
}

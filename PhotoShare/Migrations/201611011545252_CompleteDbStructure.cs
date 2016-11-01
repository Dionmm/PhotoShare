namespace PhotoShare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompleteDbStructure : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Role", newName: "Roles");
            RenameTable(name: "dbo.UserRole", newName: "UserRoles");
            RenameTable(name: "dbo.User", newName: "Users");
            RenameTable(name: "dbo.UserClaim", newName: "UserClaims");
            RenameTable(name: "dbo.UserLogin", newName: "UserLogins");
            CreateTable(
                "dbo.BannedWords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        UpdatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExifData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExifName = c.String(),
                        ExifValue = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        UpdatedDateTime = c.DateTime(nullable: false),
                        Photo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.Photo_Id)
                .Index(t => t.Photo_Id);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        OptimisedVersionAddress = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDateTime = c.DateTime(nullable: false),
                        UpdatedDateTime = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDateTime = c.DateTime(nullable: false),
                        UpdatedDateTime = c.DateTime(nullable: false),
                        Photo_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.Photo_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Photo_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Hidden = c.Boolean(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        UpdatedDateTime = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Purchases", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Messages", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Purchases", "Photo_Id", "dbo.Photos");
            DropForeignKey("dbo.ExifData", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.Messages", new[] { "User_Id" });
            DropIndex("dbo.Purchases", new[] { "User_Id" });
            DropIndex("dbo.Purchases", new[] { "Photo_Id" });
            DropIndex("dbo.Photos", new[] { "User_Id" });
            DropIndex("dbo.ExifData", new[] { "Photo_Id" });
            DropTable("dbo.Messages");
            DropTable("dbo.Purchases");
            DropTable("dbo.Photos");
            DropTable("dbo.ExifData");
            DropTable("dbo.BannedWords");
            RenameTable(name: "dbo.UserLogins", newName: "UserLogin");
            RenameTable(name: "dbo.UserClaims", newName: "UserClaim");
            RenameTable(name: "dbo.Users", newName: "User");
            RenameTable(name: "dbo.UserRoles", newName: "UserRole");
            RenameTable(name: "dbo.Roles", newName: "Role");
        }
    }
}

namespace PhotoShare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingExifDataTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ExifDatas", newName: "ExifData");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ExifData", newName: "ExifDatas");
        }
    }
}

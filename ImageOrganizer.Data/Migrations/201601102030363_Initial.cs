namespace ImageOrganizer.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaFiles",
                c => new
                    {
                        MediaFileId = c.Guid(nullable: false, identity: true),
                        OriginalFileName = c.String(nullable: false, maxLength: 250),
                        OriginalFilePath = c.String(nullable: false, maxLength: 1024),
                        FileName = c.String(nullable: false, maxLength: 250),
                        TargetFilePath = c.String(nullable: false, maxLength: 1024),
                        MarkForDelete = c.Boolean(nullable: false),
                        ArchiveDateTime = c.DateTime(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MediaFileId)
                .Index(t => t.OriginalFileName)
                .Index(t => t.OriginalFilePath)
                .Index(t => t.TargetFilePath);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.MediaFiles", new[] { "TargetFilePath" });
            DropIndex("dbo.MediaFiles", new[] { "OriginalFilePath" });
            DropIndex("dbo.MediaFiles", new[] { "OriginalFileName" });
            DropTable("dbo.MediaFiles");
        }
    }
}

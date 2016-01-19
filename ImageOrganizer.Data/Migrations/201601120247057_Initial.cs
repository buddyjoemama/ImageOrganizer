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
                        TargetFileName = c.String(nullable: false, maxLength: 250),
                        ContentHash = c.String(maxLength: 50),
                        MarkForDelete = c.Boolean(nullable: false),
                        OriginalDeleteDateTime = c.DateTime(),
                        ArchiveDateTime = c.DateTime(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MediaFileId)
                .Index(t => t.OriginalFileName)
                .Index(t => t.TargetFileName)
                .Index(t => t.ContentHash, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.MediaFiles", new[] { "ContentHash" });
            DropIndex("dbo.MediaFiles", new[] { "TargetFileName" });
            DropIndex("dbo.MediaFiles", new[] { "OriginalFileName" });
            DropTable("dbo.MediaFiles");
        }
    }
}

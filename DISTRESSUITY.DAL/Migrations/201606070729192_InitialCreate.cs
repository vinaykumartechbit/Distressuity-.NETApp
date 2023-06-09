namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        MessageBody = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.User", t => t.CreatedBy)
                .Index(t => t.ProjectId)
                .Index(t => t.CreatedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Message", "CreatedBy", "dbo.User");
            DropForeignKey("dbo.Message", "ProjectId", "dbo.Project");
            DropIndex("dbo.Message", new[] { "CreatedBy" });
            DropIndex("dbo.Message", new[] { "ProjectId" });
            DropTable("dbo.Message");
        }
    }
}

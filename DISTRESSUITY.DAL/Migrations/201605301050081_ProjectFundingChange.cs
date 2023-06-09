namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectFundingChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectFunding", "StatusId", "dbo.Status");
            DropIndex("dbo.ProjectFunding", new[] { "StatusId" });
            AddColumn("dbo.ProjectFunding", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.ProjectFunding", "StatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectFunding", "StatusId", c => c.Int(nullable: false));
            DropColumn("dbo.ProjectFunding", "Status");
            CreateIndex("dbo.ProjectFunding", "StatusId");
            AddForeignKey("dbo.ProjectFunding", "StatusId", "dbo.Status", "StatusId");
        }
    }
}

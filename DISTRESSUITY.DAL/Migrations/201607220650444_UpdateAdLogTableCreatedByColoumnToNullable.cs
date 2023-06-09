namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAdLogTableCreatedByColoumnToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdLog", "CreatedBy", c => c.Int());
            DropColumn("dbo.AdLog", "UpdatedDate");
            DropColumn("dbo.AdLog", "UpdatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdLog", "UpdatedBy", c => c.Int());
            AddColumn("dbo.AdLog", "UpdatedDate", c => c.DateTime());
            AlterColumn("dbo.AdLog", "CreatedBy", c => c.Int(nullable: false));
        }
    }
}

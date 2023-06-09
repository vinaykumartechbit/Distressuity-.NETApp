namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeaturedIdea_And_AdLog_Table_Changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeaturedIdea", "TotalClicks", c => c.Int(nullable: false));
            AddColumn("dbo.FeaturedIdea", "ClciksLeft", c => c.Int(nullable: false));
            AddColumn("dbo.FeaturedIdea", "Amount", c => c.Decimal(nullable: false));
            AddColumn("dbo.FeaturedIdea", "CardDetailId", c => c.Int(nullable: false));
            DropColumn("dbo.AdLog", "AmountPerClick");
            DropColumn("dbo.FeaturedIdea", "StartDate");
            DropColumn("dbo.FeaturedIdea", "EndDate");
            DropColumn("dbo.FeaturedIdea", "Amount");
            DropColumn("dbo.FeaturedIdea", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FeaturedIdea", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.FeaturedIdea", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FeaturedIdea", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.FeaturedIdea", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AdLog", "AmountPerClick", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.FeaturedIdea", "ClciksLeft");
            DropColumn("dbo.FeaturedIdea", "TotalClicks");
        }
    }
}

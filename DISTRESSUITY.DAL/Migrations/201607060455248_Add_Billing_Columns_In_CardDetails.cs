namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Billing_Columns_In_CardDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardDetail", "BillingName", c => c.String());
            AddColumn("dbo.CardDetail", "Address1", c => c.String());
            AddColumn("dbo.CardDetail", "Address2", c => c.String());
            AddColumn("dbo.CardDetail", "City", c => c.String());
            AddColumn("dbo.CardDetail", "Region", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardDetail", "Region");
            DropColumn("dbo.CardDetail", "City");
            DropColumn("dbo.CardDetail", "Address2");
            DropColumn("dbo.CardDetail", "Address1");
            DropColumn("dbo.CardDetail", "BillingName");
        }
    }
}

namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConversationReplyChangesRemoveToColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ConversationReply", "To");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ConversationReply", "To", c => c.Int(nullable: false));
        }
    }
}

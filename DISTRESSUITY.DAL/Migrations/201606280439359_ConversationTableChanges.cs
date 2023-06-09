namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConversationTableChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Conversation", "CreatedBy");
            DropColumn("dbo.Conversation", "IsPrivate");
            RenameColumn(table: "dbo.Conversation", name: "User1", newName: "To");
            RenameColumn(table: "dbo.Conversation", name: "User2", newName: "CreatedBy");
            //RenameIndex(table: "dbo.Conversation", name: "IX_User1", newName: "IX_To");
            //RenameIndex(table: "dbo.Conversation", name: "IX_User2", newName: "IX_CreatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conversation", "IsPrivate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Conversation", "CreatedBy", c => c.Int(nullable: false));
            //RenameIndex(table: "dbo.Conversation", name: "IX_CreatedBy", newName: "IX_User2");
            //RenameIndex(table: "dbo.Conversation", name: "IX_To", newName: "IX_User1");
            RenameColumn(table: "dbo.Conversation", name: "CreatedBy", newName: "User2");
            RenameColumn(table: "dbo.Conversation", name: "To", newName: "User1");
        }
    }
}

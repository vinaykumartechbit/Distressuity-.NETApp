namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Message");
            DropColumn("dbo.Message", "CommentId");
            AddColumn("dbo.Message", "MessageId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Message", "MessageId");
        }
        
        //public override void Down()
        //{
        //    AddColumn("dbo.Message", "CommentId", c => c.Int(nullable: false, identity: true));
        //    DropPrimaryKey("dbo.Message");
        //    DropColumn("dbo.Message", "MessageId");
        //    AddPrimaryKey("dbo.Message", "CommentId");            
        //}
    }
}

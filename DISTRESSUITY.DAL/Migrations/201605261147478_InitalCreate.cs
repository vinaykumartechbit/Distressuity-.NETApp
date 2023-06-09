namespace DISTRESSUITY.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitalCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdLog",
                c => new
                    {
                        AdLogsId = c.Int(nullable: false, identity: true),
                        FeatureIdeaId = c.Int(nullable: false),
                        DateTimeOfClick = c.DateTime(nullable: false),
                        IpAddress = c.String(),
                        AmountPerClick = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AdLogsId)
                .ForeignKey("dbo.FeaturedIdea", t => t.FeatureIdeaId)
                .Index(t => t.FeatureIdeaId);
            
            CreateTable(
                "dbo.FeaturedIdea",
                c => new
                    {
                        FeaturedIdeaId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FeaturedIdeaId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        IndustryId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                        VideoPath = c.String(),
                        VideoName = c.String(),
                        ImagePath = c.String(),
                        ImageName = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        CompanyName = c.String(),
                        Location = c.String(),
                        AnnualSales = c.Decimal(precision: 18, scale: 2),
                        EquityOffered = c.Int(nullable: false),
                        InvestmentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFundingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FundingDuration = c.DateTime(nullable: false),
                        RiskAndChallenges = c.String(),
                        OwnerName = c.String(),
                        Biography = c.String(),
                        Address = c.String(),
                        WebsiteLink = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        StatusId = c.Int(nullable: false),
                        MinPledge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.Industry", t => t.IndustryId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.User", t => t.OwnerId)
                .Index(t => t.IndustryId)
                .Index(t => t.OwnerId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.FinancialBreakdown",
                c => new
                    {
                        FinancialBreakdownId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FinancialBreakdownId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Industry",
                c => new
                    {
                        IndustryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IndustryId);
            
            CreateTable(
                "dbo.ProjectDocument",
                c => new
                    {
                        ProjectDocumentId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        DocumentSize = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectDocumentId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.ProjectFunding",
                c => new
                    {
                        ProjectFundingId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        FundingUserId = c.Int(nullable: false),
                        FundingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FundingDate = c.DateTime(nullable: false),
                        IsDeducted = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectFundingId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.User", t => t.FundingUserId)
                .Index(t => t.ProjectId)
                .Index(t => t.FundingUserId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        IndustryId = c.Int(),
                        Summary = c.String(),
                        PictureUrl = c.String(),
                        Specialists = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Industry", t => t.IndustryId)
                .Index(t => t.IndustryId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserPosition",
                c => new
                    {
                        PosititonId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CompanyName = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        StartMonth = c.Int(nullable: false),
                        StartYear = c.Int(nullable: false),
                        EndMonth = c.Int(),
                        EndYear = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PosititonId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CardDetail",
                c => new
                    {
                        CardDetailId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        CardNumber = c.String(),
                        ExpirationMonth = c.Int(nullable: false),
                        ExpirationYear = c.Int(nullable: false),
                        CardCVN = c.Int(nullable: false),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.CardDetailId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Type = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.User_Project_CardDetail",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        CardDetailId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.CardDetailId, t.ProjectId })
                .ForeignKey("dbo.CardDetail", t => t.CardDetailId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CardDetailId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User_Project_CardDetail", "UserId", "dbo.User");
            DropForeignKey("dbo.User_Project_CardDetail", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.User_Project_CardDetail", "CardDetailId", "dbo.CardDetail");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.CardDetail", "UserId", "dbo.User");
            DropForeignKey("dbo.Project", "OwnerId", "dbo.User");
            DropForeignKey("dbo.Project", "StatusId", "dbo.Status");
            DropForeignKey("dbo.ProjectFunding", "FundingUserId", "dbo.User");
            DropForeignKey("dbo.UserPosition", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.User");
            DropForeignKey("dbo.User", "IndustryId", "dbo.Industry");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.User");
            DropForeignKey("dbo.ProjectFunding", "StatusId", "dbo.Status");
            DropForeignKey("dbo.ProjectFunding", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.ProjectDocument", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "IndustryId", "dbo.Industry");
            DropForeignKey("dbo.FinancialBreakdown", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.FeaturedIdea", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.AdLog", "FeatureIdeaId", "dbo.FeaturedIdea");
            DropIndex("dbo.User_Project_CardDetail", new[] { "ProjectId" });
            DropIndex("dbo.User_Project_CardDetail", new[] { "CardDetailId" });
            DropIndex("dbo.User_Project_CardDetail", new[] { "UserId" });
            DropIndex("dbo.Role", "RoleNameIndex");
            DropIndex("dbo.CardDetail", new[] { "UserId" });
            DropIndex("dbo.UserPosition", new[] { "UserId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.User", "UserNameIndex");
            DropIndex("dbo.User", new[] { "IndustryId" });
            DropIndex("dbo.ProjectFunding", new[] { "StatusId" });
            DropIndex("dbo.ProjectFunding", new[] { "FundingUserId" });
            DropIndex("dbo.ProjectFunding", new[] { "ProjectId" });
            DropIndex("dbo.ProjectDocument", new[] { "ProjectId" });
            DropIndex("dbo.FinancialBreakdown", new[] { "ProjectId" });
            DropIndex("dbo.Project", new[] { "StatusId" });
            DropIndex("dbo.Project", new[] { "OwnerId" });
            DropIndex("dbo.Project", new[] { "IndustryId" });
            DropIndex("dbo.FeaturedIdea", new[] { "ProjectId" });
            DropIndex("dbo.AdLog", new[] { "FeatureIdeaId" });
            DropTable("dbo.User_Project_CardDetail");
            DropTable("dbo.Role");
            DropTable("dbo.CardDetail");
            DropTable("dbo.UserPosition");
            DropTable("dbo.UserRole");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.User");
            DropTable("dbo.Status");
            DropTable("dbo.ProjectFunding");
            DropTable("dbo.ProjectDocument");
            DropTable("dbo.Industry");
            DropTable("dbo.FinancialBreakdown");
            DropTable("dbo.Project");
            DropTable("dbo.FeaturedIdea");
            DropTable("dbo.AdLog");
        }
    }
}

namespace DISTRESSUITY.DAL
{
    using Entities;
    using Interfaces;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class DISTRESSUITYContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole,
        ApplicationUserClaim>
    {
        private string[] excludeProperties = new string[] { "CreatedDate", "CreatedBy" };
        
        // Your context has been configured to use a 'DISTRESSUITYContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DISTRESSUITY.DAL.DISTRESSUITYContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DISTRESSUITYContext' 
        // connection string in the application configuration file.
        public DISTRESSUITYContext()
            : base("name=DefaultConnection")
        {
            //disable initializer
            //Database.SetInitializer<DISTRESSUITYContext>(null);
        }

        public static DISTRESSUITYContext Create()
        {
            return new DISTRESSUITYContext();
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                #region Set Log Info Details
                //Set the log info details
                if (entry.Entity is ILogInfo)
                {
                    if (entry.State == EntityState.Added)
                    {
                        ((ILogInfo)entry.Entity).CreatedDate = DateTime.UtcNow;
                        ((ILogInfo)entry.Entity).UpdatedDate = null;
                        ((ILogInfo)entry.Entity).UpdatedBy = null;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        foreach (string property in excludeProperties)
                        {
                            entry.Property(property).IsModified = false;
                        }
                        ((ILogInfo)entry.Entity).UpdatedDate = DateTime.UtcNow;
                    }
                }
                #endregion
            }
            return base.SaveChanges();
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<UserPosition> UserPositions { get; set; }
        public virtual DbSet<AdLog> AdLogs { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectFunding> ProjectFundings { get; set; }
        public virtual DbSet<FinancialBreakdown> FinancialBreakdowns { get; set; }
        public virtual DbSet<FeaturedIdea> FeaturedIdeas { get; set; }
        public virtual DbSet<Industry> Industries { get; set; }
        public virtual DbSet<ProjectDocument> ProjectDocuments { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<CardDetail> CardDetails { get; set; }
        public virtual DbSet<User_Project_CardDetail> User_Project_CardDetails { get; set; }
        public virtual DbSet<Conversation> Conversation { get; set; }
        public virtual DbSet<ConversationReply> ConversationReply { get; set; }
        public virtual DbSet<PublicMessage> PublicMessage { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            modelBuilder.Entity<ApplicationRole>().ToTable("Role");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRole");
        }
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            this.FinancialBreakdowns = new List<FinancialBreakdown>();
            this.FeaturedIdeas = new List<FeaturedIdea>();
            this.ProjectFundings = new List<ProjectFunding>();
            this.ProjectDocuments = new List<ProjectDocument>();
            this.Conversations = new List<Conversation>();
            this.PublicMessages = new List<PublicMessage>();
            this.Transaction = new List<Transaction>();
            //this.Industry = new Industry();
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectId { get; set; }
        public int IndustryId { get; set; }
        public int OwnerId { get; set; }
        public string VideoPath { get; set; }
        public string VideoName { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public decimal? AnnualSales { get; set; }

        [DisplayName("%OfEquityOffered")]
        public int EquityOffered { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal TotalFundingAmount { get; set; }
        public DateTime FundingDuration { get; set; }
        public string RiskAndChallenges { get; set; }
        public string OwnerName { get; set; }
        public string Biography { get; set; }
        public string Address { get; set; }
        public string WebsiteLink { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusId { get; set; }
        public decimal MinPledge { get; set; }

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("IndustryId")]
        public virtual Industry Industry { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        public string LogoPath { get; set; }
        public string LogoName { get; set; }
        public string ProjectTitle { get; set; }
        public string PaypalAccount { get; set; }
        public bool PaypalAccountStatus { get; set; }
        public bool IsFunded { get; set; }
        public bool IsTransfered { get; set; }
        public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; }
        public virtual ICollection<FinancialBreakdown> FinancialBreakdowns { get; set; }
        public virtual ICollection<FeaturedIdea> FeaturedIdeas { get; set; }
        public virtual ICollection<ProjectFunding> ProjectFundings { get; set; }
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<PublicMessage> PublicMessages { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DISTRESSUITY.Service.Model
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            this.FinancialBreakdowns = new List<FinancialBreakdownModel>();
            this.FeaturedIdeas = new List<FeaturedIdeaModel>();
            this.ProjectFundings = new List<ProjectFundingModel>();
            this.ProjectDocuments = new List<ProjectDocumentModel>();
            this.User = new UserModel();
            this.Status = new StatusModel();
            this.Conversations = new List<ConversationModel>();
            this.Industry = new IndustryModel();
            this.PublicMessages = new List<PublicMessageModel>();
        }
        public string ProjectId { get; set; }
        [Required]
        public string IndustryId { get; set; }
        public int OwnerId { get; set; }
        public string VideoPath { get; set; }
        public string VideoName { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        [Required]
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        [Required]
        public string Location { get; set; }
        public decimal? AnnualSales { get; set; }

        [DisplayName("%OfEquityOffered")]
        public int EquityOffered { get; set; }
        public decimal InvestmentAmount { get; set; }
        [Required]
        public decimal TotalFundingAmount { get; set; }
        [Required]
        public string FundingDuration { get; set; }
        public decimal TotalProcessingAmount { get; set; }
        public string RiskAndChallenges { get; set; }
        public string OwnerName { get; set; }
        public string Biography { get; set; }
        public string Address { get; set; }
        public string WebsiteLink { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusId { get; set; }
        public decimal MinPledge { get; set; }
        public int TabType { get; set; }
        public decimal FeaturedClickPrice { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserModel User { get; set; }
        public StatusModel Status { get; set; }
        public string LogoPath { get; set; }
        public string LogoName { get; set; }
        public string ProjectTitle { get; set; }
        public string PaypalAccount { get; set; }
        public bool PaypalAccountStatus { get; set; }
        public decimal SiteCommission { get; set; }
        public decimal TransactionCharges { get; set; }
        public decimal PaymentReceived { get; set; }
        public bool IsTransfered { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public List<FinancialBreakdownModel> FinancialBreakdowns { get; set; }
        public List<FeaturedIdeaModel> FeaturedIdeas { get; set; }
        public List<ProjectFundingModel> ProjectFundings { get; set; }
        public List<ProjectDocumentModel> ProjectDocuments { get; set; }
        public List<ConversationModel> Conversations { get; set; }
        public IndustryModel Industry { get; set; }
        public List<PublicMessageModel> PublicMessages { get; set; }
    }
}

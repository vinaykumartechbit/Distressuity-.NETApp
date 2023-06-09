using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class ProjectFundingModel
    {
        public ProjectFundingModel()
        {
            this.UserModel = new UserModel();
            this.Status = new StatusModel();
        }
        public string ProjectFundingId { get; set; }
        public string ProjectId { get; set; }
        public int FundingUserId { get; set; }
        public decimal FundingAmount { get; set; }
        public DateTime FundingDate { get; set; }
        public bool IsDeducted { get; set; }
        public bool IsDeleted { get; set; }
        public int StatusId { get; set; }
        public int EquityDemanded { get; set; }
        public decimal ProcessingFees { get; set; }
        public decimal TotalAmount { get; set; }
        public UserModel UserModel { get; set; }
        public StatusModel Status { get; set; }
        public string ProjectTitle {get;set;}
        public string ProjectFundingDuration { get; set; }
        public string InvestedBy { get; set; }
        public decimal SiteCommission { get; set; }
        public decimal TransactionCharges { get; set; }
        public decimal PaymentReceived { get; set; }

    }
}

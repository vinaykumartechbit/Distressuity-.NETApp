using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class PaymentModel
    {
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public int CardCVN { get; set; }
        public string PostalCode { get; set; }
        public string FundingAmount { get; set; }
        public string ProjectId { get; set; }
        public int ProjectFundingId { get; set; }
        public int EquityDemanded { get; set; }
        public string BillingName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string CountryCode { get; set; }
        public string ProcessingFees { get; set; }
        public int FundingUserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class CardDetailModel
    {
        public int CardDetailId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public int CardCVN { get; set; }
        public string PostalCode { get; set; }
        public string BillingName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        
    }
}

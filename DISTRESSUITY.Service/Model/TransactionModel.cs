using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class TransactionModel
    {
        public TransactionModel() { }
        public int TransactionId { get; set; }
        public string PaymentId { get; set; }
        public string Amount { get; set; }
        public string Fees { get; set; }
        public string FinalAmount { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class Transaction : BaseEntity
    {
        public Transaction() { }

        public int TransactionId { get; set; }
        public string PaymentId { get; set; }
        public string Amount { get; set; }
        public string Fees { get; set; }
        public string FinalAmount { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}

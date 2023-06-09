using DISTRESSUITY.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class ProjectFunding : IFlagRemove
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectFundingId { get; set; }
        public int ProjectId { get; set; }
        public int FundingUserId { get; set; }
        public int? TransactionId { get; set; }
        public decimal FundingAmount { get; set; }
        public DateTime FundingDate { get; set; }
        public bool IsDeducted { get; set; }
        public bool IsDeleted { get; set; }
        public int StatusId { get; set; }
        public int EquityDemanded { get; set; }
        public decimal ProcessingFees { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [ForeignKey("FundingUserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }
    }
}

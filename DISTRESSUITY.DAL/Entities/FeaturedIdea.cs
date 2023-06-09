using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class FeaturedIdea : BaseEntity
    {
        public FeaturedIdea()
        {
            this.AdLogs = new List<AdLog>();
        }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int FeaturedIdeaId { get; set; }
        public int ProjectId { get; set; }
        public int TotalClicks { get; set; }
        public int ClciksLeft { get; set; }
        public decimal Amount { get; set; }
        public int CardDetailId { get; set; }
        
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public virtual ICollection<AdLog> AdLogs { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class AdLog
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AdLogsId { get; set; }
        public int FeatureIdeaId { get; set; }
        public DateTime DateTimeOfClick { get; set; }
        public string IpAddress { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("FeatureIdeaId")]
        public FeaturedIdea FeaturedIdea { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser User { get; set; }

    }
}

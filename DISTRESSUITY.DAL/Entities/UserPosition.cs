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
    public class UserPosition:IFlagRemove
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PosititonId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}

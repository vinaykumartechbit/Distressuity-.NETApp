using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class User_Project_CardDetail
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int CardDetailId { get; set; }

        [Key, Column(Order = 2)]
        public int ProjectId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("CardDetailId")]
        public CardDetail CardDetail { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}

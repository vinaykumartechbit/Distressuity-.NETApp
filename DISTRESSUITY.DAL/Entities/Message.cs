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
    public class PublicMessage : BaseEntity, IFlagRemove 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int MessageId { get; set; }
        public int ProjectId { get;set; }
        public string MessageBody { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("ProjectId")]
        public virtual  Project Project { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser User { get; set; }
    }
}

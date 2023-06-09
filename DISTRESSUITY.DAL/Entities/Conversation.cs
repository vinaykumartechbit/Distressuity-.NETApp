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
    public class Conversation : BaseEntity, IFlagRemove
    {
        public Conversation()
        {
            this.ConversationReply = new List<ConversationReply>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ConversationId { get; set; }
        public int ProjectID { get; set; }
        public int To { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("To")]
        public virtual ApplicationUser ToUser { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }
        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }
        //[NotMapped]
        public virtual ICollection<ConversationReply> ConversationReply { get; set; }
        
    }
}

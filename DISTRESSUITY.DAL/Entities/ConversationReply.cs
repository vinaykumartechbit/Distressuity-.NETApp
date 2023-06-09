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
    public class ConversationReply : BaseEntity, IFlagRemove
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ConversationReplyId { get; set; }
        public string Reply { get; set; }
        //public int To { get; set; }
        public int ConversationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool Read { get; set; }
        [ForeignKey("CreatedBy")]
        public ApplicationUser User { get; set; }

        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; }
    }
}

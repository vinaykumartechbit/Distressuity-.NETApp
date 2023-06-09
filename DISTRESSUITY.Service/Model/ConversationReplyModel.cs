using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class ConversationReplyModel
    {
        public ConversationReplyModel()
        {
            this.UserModel = new UserModel();
            this.ConversationModel = new ConversationModel();
        }
        public string ConversationReplyId { get; set; }
        public string Reply { get; set; }
        public string ConversationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual ConversationModel ConversationModel { get; set; }

    }
}

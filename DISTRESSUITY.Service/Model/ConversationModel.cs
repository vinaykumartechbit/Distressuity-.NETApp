using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class ConversationModel
    {
        public ConversationModel()
        {
            this.ToUserModel = new UserModel();
            this.CreatedByUserModel = new UserModel();
            this.ConversationReplyModel = new List<ConversationReplyModel>();
            this.ProjectModel = new ProjectModel();
        }

        public string ConversationId { get; set; }
        public string ProjectID { get; set; }
        public string To { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int UnreadMessages { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public UserModel ToUserModel { get; set; }
        public UserModel CreatedByUserModel { get; set; }
        public ProjectModel ProjectModel { get; set; }
        public List<ConversationReplyModel> ConversationReplyModel { get; set; }
        public string Message { get; set; }

    }
}

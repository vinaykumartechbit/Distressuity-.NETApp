using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class PublicMessageModel
    {
        public PublicMessageModel()
        {
            this.Project = new ProjectModel();
            this.UserModel = new UserModel();
        }

        public string MessageId { get; set; }
        public string ProjectId { get; set; }
        public string MessageBody { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ProjectModel Project { get; set; }
        public virtual UserModel UserModel { get; set; }
    }
}

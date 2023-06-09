using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class FeaturedIdeaModel
    {
         public FeaturedIdeaModel()
        {
            this.AdLogsModel = new List<AdLogModel>();
            this.ProjectModel = new ProjectModel();
        }
        public string FeaturedIdeaId { get; set; }
        public string ProjectId { get; set; }
        public int TotalClicks { get; set; }
        public int ClciksLeft { get; set; }
        public decimal Amount { get; set; }
        //public string CardDetailId { get; set; }
        public List<AdLogModel> AdLogsModel { get; set; }
        public ProjectModel ProjectModel { get; set; } 
    }
}

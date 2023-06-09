using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class IndustryModel
    {
        public IndustryModel()
        {
            this.ProjectsModel = new List<ProjectModel>();
        }
        public string IndustryId { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProjectModel> ProjectsModel { get; set; } 
    }
}

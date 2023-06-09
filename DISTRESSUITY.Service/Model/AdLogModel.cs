using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class AdLogModel
    {
        public AdLogModel()
        {
            this.FeaturedIdeaModel = new FeaturedIdeaModel();
        }
        public string AdLogsId { get; set; }
        public string FeatureIdeaId { get; set; }
        public DateTime DateTimeOfClick { get; set; }
        public string IpAddress { get; set; }
        public FeaturedIdeaModel FeaturedIdeaModel { get; set; }
    }
}

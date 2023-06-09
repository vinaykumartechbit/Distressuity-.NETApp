using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class UserPositionsModel
    {
        public int PosititonId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }

    }
}

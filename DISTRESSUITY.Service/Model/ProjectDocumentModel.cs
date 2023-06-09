using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class ProjectDocumentModel
    {
        public int ProjectDocumentId { get; set; }
        public string ProjectId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
    }
}

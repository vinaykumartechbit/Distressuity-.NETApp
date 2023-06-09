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
    public class ProjectDocument : BaseEntity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ProjectDocumentId { get; set; }
        public int ProjectId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }

        [ForeignKey("ProjectId")]
        public Project project { get; set; }
    }
}

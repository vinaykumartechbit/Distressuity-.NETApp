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
    public class Industry : IFlagRemove
    {
        public Industry()
        {
            this.Projects = new List<Project>();
        }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IndustryId { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Project> Projects { get; set; } 

    }
}

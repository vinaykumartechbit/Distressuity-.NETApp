using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DISTRESSUITY.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>, IRole<int>, IFlagRemove, ILogInfo
    {
        public ApplicationRole() { }
        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }
        public ApplicationRole(string name, string description)
            : this(name)
        {
            this.Description = description;
        }

        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

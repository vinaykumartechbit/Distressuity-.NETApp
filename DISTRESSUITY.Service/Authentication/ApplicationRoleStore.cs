using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DISTRESSUITY.DAL;
using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Authentication
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole, int, ApplicationUserRole>, IQueryableRoleStore<ApplicationRole, int>, IRoleStore<ApplicationRole, int>, IDisposable
    {
        public ApplicationRoleStore(DISTRESSUITYContext context)
            : base(context)
        {
        }
    }
}

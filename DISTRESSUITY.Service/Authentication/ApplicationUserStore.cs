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
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin,
        ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, int>, IDisposable
    {
        public ApplicationUserStore(DISTRESSUITYContext context)
            : base(context)
        {

        }

    }
}

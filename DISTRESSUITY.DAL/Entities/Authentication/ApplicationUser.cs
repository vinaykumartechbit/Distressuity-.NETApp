using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DISTRESSUITY.DAL.Interfaces;
using DISTRESSUITY.DAL.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Entities
{
    public partial class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
        , IUser<int>, IFlagRemove, ILogInfo
    {
        //public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? IndustryId { get; set; }
        public string Summary { get; set; }
        public string PictureUrl { get; set; }
        public string Specialists { get; set; }
        public string Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [ForeignKey("IndustryId")]
        public Industry Industry { get; set; }

        public virtual ICollection<UserPosition> UserPositions { get; set; }
      
        public Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager, string authenticationType, bool isThirdParty = false)
        {
            var userIdentity = manager.CreateIdentity(this, authenticationType);
            //var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("UserId", this.Id.ToString()));
            //userIdentity.AddClaim(new Claim("FullName", this.FirstName + " " + this.LastName));
            //userIdentity.AddClaim(new Claim("CompanyId", this.CompanyId.ToString()));
            //userIdentity.AddClaim(new Claim("RoleId", roleId.ToString()));

            return Task.FromResult(userIdentity);
        }
    }
}

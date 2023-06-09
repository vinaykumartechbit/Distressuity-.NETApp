using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Model
{
    public class UserModel
    {
        public UserModel()
        {
            this.UserPositions = new List<UserPositionsModel>();
        }
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string IndustryId { get; set; }
        public string Summary { get; set; }
        public string PictureUrl { get; set; }
        public string Specialists { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        
        public List<UserPositionsModel> UserPositions { get; set; }
        //public List<object> Months { get; set; }
    }
}

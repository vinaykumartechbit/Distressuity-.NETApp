using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.Service.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationServiceResponse> CreateUser(UserModel userModel, bool isExternalLogin = false);
        AuthenticationServiceResponse SignInUser(LoginModel userModel, string authenticationType, bool isPersistent = false);
        Task<ApplicationUser> FindByNameAsync(string name);
        void SignOut();
        Task<ApplicationUser> FindUser(string userName, string password);
        Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo);
        Task<AuthenticationServiceResponse> ConfirmAccount(int userId, string code);
        Task<AuthenticationServiceResponse> SetPassword(int userId, string password);
        Task<AuthenticationServiceResponse> ForgetUser(ForgetPasswordModel userModel);
        Task<AuthenticationServiceResponse> ResetPassword(int userId, string password);
        ClaimsIdentity ExternalLogin(string authenticationType, string userName, bool isPersistent = false);
        ExternalLoginInfo GetExternalLoginInfo();
        IndustryModel GetIndustryByName(string IndustryName);

        
    }
}

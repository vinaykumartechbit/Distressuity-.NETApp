using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.Service.Interfaces;
using DISTRESSUITY.Service.Model;
using DISTRESSUITY.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DISTRESSUITY.Common.Constants;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using DISTRESSUITY.DAL.Interfaces;
using System.Security.Claims;
using DISTRESSUITY.Common.Utility;
using System.Net;

namespace DISTRESSUITY.Service.Service
{
    public class AuthenticationService : AbstractService, IAuthenticationService
    {
        #region Constructors
        public AuthenticationService(IUnitofWork unitwork) :
            base(unitwork)
        {

        }
        public AuthenticationService()
        {

        }
        #endregion


        public Task<AuthenticationServiceResponse> CreateUser(UserModel model, bool isExternalLogin = false)
        {
            var userPosition = new List<UserPosition>();
            var user = new ApplicationUser();
            var result = new IdentityResult();

            try
            {
                var exstUsr = UserManager.FindByEmail(model.Email);
                if (exstUsr == null)
                {
                    Mapper.Map(model, user);
                    user.CreatedDate = DateTime.UtcNow;


                    if (isExternalLogin)
                    {
                        if (user.IndustryId.Equals(0))
                            user.IndustryId = null;
                        result = UserManager.Create(user);

                        result = UserManager.AddLogin(user.Id, new UserLoginInfo(model.LoginProvider, model.ProviderKey));

                        // Creating External User
                        // model.UserPositions.ForEach(t => t.UserId = user.Id);
                        //Mapper.Map(model.UserPositions, userPosition);
                        //_unitOfWork.userPositionsRepository.InsertAll(userPosition);
                        //_unitOfWork.Save();
                    }
                    else
                    {
                        if (user.IndustryId.Equals(0))
                            user.IndustryId = null;
                        result = UserManager.Create(user);// Creating Signup user
                        SendEmail(user, MailSubject.CONFIRM_ACCOUNT);
                    }

                    Mapper.Map(user, model);
                    if (result.Succeeded)
                    {
                        return Task.FromResult(new AuthenticationServiceResponse() { Data = model, Success = result.Succeeded, Message = CommonConstants.EmailVerification });
                    }
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = model, Success = result.Succeeded, Message = result.Errors.FirstOrDefault() });
                }
                else
                {
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = model, Success = false, Message = "Email already exist" });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public Task<AuthenticationServiceResponse> SetPassword(int userId, string password)
        {
            UserManager.RemovePassword(userId);
            var result = UserManager.AddPassword(userId, password);
            if (result.Succeeded)
            {
                var userDetails = _unitOfWork.applicationUserRepository.Get(user => user.Id == userId).FirstOrDefault();
                if (userDetails != null)
                {
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = userDetails.UserName, Success = true, Message = result.Errors.FirstOrDefault() });
                }
                return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = "Error Fetching records! Try Login Manually" });
            }
            return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = result.Errors.FirstOrDefault() });
        }

        public AuthenticationServiceResponse SignInUser(LoginModel model, string authenticationType, bool isPersistent = false)
        {
            var claimsIdentity = new ClaimsIdentity();
            var user = UserManager.FindByEmail(model.UserName);
            if (user == null)
                return new AuthenticationServiceResponse() { Success = false, Message = UserLoginConstants.EMAIL_NOT_FOUND };

            var signStatus = SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, //loginModel.RememberMe,
                   (!user.LockoutEnabled ? user.LockoutEnabled : UserManager.UserLockoutEnabledByDefault)).Result;
            int accessFailedCount = UserManager.GetAccessFailedCount(user.Id);
            int attemptsLeft = UserManager.MaxFailedAccessAttemptsBeforeLockout - accessFailedCount;
            bool halfAttemptExceed = accessFailedCount > (UserManager.MaxFailedAccessAttemptsBeforeLockout) / 2;
            if (signStatus != SignInStatus.Success && !halfAttemptExceed)
            {
                return new AuthenticationServiceResponse() { Success = false, Message = UserLoginConstants.INVALID_LOGIN };
            }
            if (signStatus == SignInStatus.Success)
            {
                claimsIdentity = user.GenerateUserIdentityAsync(UserManager, authenticationType, true).Result;
                SignInManager.AuthenticationManager.SignOut(authenticationType);
                SignInManager.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimsIdentity);

            }
            return new AuthenticationServiceResponse() { Success = true, Message = "Login Successfully", identity = claimsIdentity };
        }

        public Task<ApplicationUser> FindUser(string userName, string password)
        {
            try
            {
                return UserManager.FindAsync(userName, password);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            ApplicationUser user = await UserManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> AddLoginAsync(int userId, UserLoginInfo login)
        {
            var result = await UserManager.AddLoginAsync(userId, login);

            return result;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var result = await UserManager.CreateAsync(user);
            return result;
        }

        public async Task<ApplicationUser> FindByNameAsync(string name)
        {
            var result = await UserManager.FindByNameAsync(name);
            return result;
        }

        public Task<AuthenticationServiceResponse> ConfirmAccount(int userId, string code)
        {
            if (!string.IsNullOrEmpty(userId.ToString()) && !string.IsNullOrEmpty(code))
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    string newCode = WebUtility.UrlDecode(code);
                    var result = UserManager.ConfirmEmail(userId, code);
                    if (!result.Succeeded)
                    {
                        return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = "Licence Experienced" });
                    }

                    user.EmailConfirmed = true;
                    user.UpdatedDate = DateTime.Now;
                    UserManager.Update(user);
                    return Task.FromResult(new AuthenticationServiceResponse() { Success = true, Message = "Successfully" });
                }
            }
            return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = "Invalid Code" });
        }

        public void SendEmail(ApplicationUser userInfo, string emailSubject)
        {
            string mailBody = string.Empty;
            string code = string.Empty;

            // mailBodyTemplate.UserName = userInfo.FirstName + " " + userInfo.LastName;

            MailHelper mailHelper = new MailHelper();
            mailHelper.ToEmail = userInfo.Email;


            if (emailSubject == MailSubject.CONFIRM_ACCOUNT)
            {
                code = UserManager.GenerateEmailConfirmationTokenAsync(userInfo.Id).Result.ToString();
                code = WebUtility.UrlEncode(code);

                string newCode = WebUtility.UrlDecode(code);

                string AccountLoginUrl = CommonFunction.GetConfirmAccountUrl(userInfo.Id.Encrypt(), code);

                mailHelper.Subject = MailSubject.CONFIRM_ACCOUNT.ToString();
                mailHelper.Body = CommonFunction.ConfigureConfirmAccountMailBodyDb(AccountLoginUrl); //GetEmailBody(AccountLoginUrl);
                //  mailHelper.Subject = emailTemplatesModel.SingleOrDefault(x => x.TemplateName == "ConfirmAccount").Subject;
                //  mailHelper.FromEmail = emailTemplatesModel.SingleOrDefault(x => x.TemplateName == "ConfirmAccount").FromAddress;
            }
            mailHelper.SendEmail();

        }

        public ClaimsIdentity ExternalLogin(string authenticationType, string userName, bool isPersistent = false)
        {
            var claimsIdentity = new ClaimsIdentity();
            var user = UserManager.FindByEmail(userName);
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //SignOut();
            claimsIdentity = user.GenerateUserIdentityAsync(UserManager, authenticationType, true).Result;

            SignInManager.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimsIdentity);
            return claimsIdentity;

        }
        public ExternalLoginInfo GetExternalLoginInfo()
        {
            return base.SignInManager.AuthenticationManager.GetExternalLoginInfo();
        }
        public void ForgetPasswordSendEmail(ApplicationUser userInfo, string emailSubject)
        {
            string mailBody = string.Empty;
            string code = string.Empty;

            // mailBodyTemplate.UserName = userInfo.FirstName + " " + userInfo.LastName;

            MailHelper mailHelper = new MailHelper();
            mailHelper.ToEmail = userInfo.Email;


            if (emailSubject == MailSubject.CONFIRM_ACCOUNT)
            {
                code = UserManager.GenerateEmailConfirmationTokenAsync(userInfo.Id).Result.ToString();
                code = WebUtility.UrlEncode(code);

                string newCode = WebUtility.UrlDecode(code);

                string AccountLoginUrl = CommonFunction.GetForgetPasswordUrl(userInfo.Id.Encrypt(), code);

                mailHelper.Subject = MailSubject.CONFIRM_ACCOUNT.ToString();
                mailHelper.Body = CommonFunction.ConfigureForgetPasswordMailBodyDb(AccountLoginUrl); //GetEmailBody(AccountLoginUrl);
                //  mailHelper.Subject = emailTemplatesModel.SingleOrDefault(x => x.TemplateName == "ConfirmAccount").Subject;
                //  mailHelper.FromEmail = emailTemplatesModel.SingleOrDefault(x => x.TemplateName == "ConfirmAccount").FromAddress;
            }
            mailHelper.SendEmail();

        }
        public Task<AuthenticationServiceResponse> ForgetUser(ForgetPasswordModel model)
        {
            var exstUsr = UserManager.FindByName(model.Email);
            try
            {
                if (exstUsr == null)
                {
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = model, Success = false, Message = "Invalid Email" });
                }
                else
                {
                    ForgetPasswordSendEmail(exstUsr, MailSubject.CONFIRM_ACCOUNT);
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = model, Success = true, Message = CommonConstants.EmailVerification });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<AuthenticationServiceResponse> ResetPassword(int userId, string password)
        {
            UserManager.RemovePassword(userId);
            var result = UserManager.AddPassword(userId, password);
            if (result.Succeeded)
            {
                var userDetails = _unitOfWork.applicationUserRepository.Get(user => user.Id == userId).FirstOrDefault();
                if (userDetails != null)
                {
                    return Task.FromResult(new AuthenticationServiceResponse() { Data = userDetails.UserName, Success = true, Message = result.Errors.FirstOrDefault() });
                }
                return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = "Error Fetching records! Try Login Manually" });
            }
            return Task.FromResult(new AuthenticationServiceResponse() { Success = false, Message = result.Errors.FirstOrDefault() });
        }

        public IndustryModel GetIndustryByName(string IndustryName)
        {
            var industryModel = new IndustryModel();

            var result = _unitOfWork.industryRepository.Get(data => data.Name == IndustryName).FirstOrDefault();
            if (result == null)
            {
                var industryId=Convert.ToInt32(ReadConfiguration.MiscellaneousIndustry);
                result = _unitOfWork.industryRepository.Get(data => data.IndustryId == industryId).FirstOrDefault();
            }
            //var userId = GetLoginUserId();
                    //var newIndustry = new Industry { Name = IndustryName };
                    //_unitOfWork.industryRepository.Insert(newIndustry);
                    //_unitOfWork.Save();
                    //Mapper.Map(newIndustry, industryModel);
            Mapper.Map(result, industryModel);
            return industryModel;
        }
        public void SignOut()
        {
            var loginInfo = this.GetExternalLoginInfo();
            var loggedUserId = GetLoginUserId();
            //Here we are removing user claims if exist
            if (loginInfo != null && loginInfo.ExternalIdentity.IsAuthenticated)
            {
                string userId = loginInfo.ExternalIdentity.Claims.First(c => c.Type.Equals("UserId")).Value;
                foreach (var claim in loginInfo.ExternalIdentity.Claims)
                {
                    UserManager.RemoveClaim(Convert.ToInt32(userId), claim);
                }
            }
            string[] types = new string[] { DefaultAuthenticationTypes.ExternalBearer, DefaultAuthenticationTypes.ExternalCookie };
            SignInManager.AuthenticationManager.SignOut(types);
        }


        public string GetLoggoedInUserRole(string UserName)
        {
            var user = UserManager.FindByEmail(UserName);
            var userRole = UserManager.GetRolesAsync(user.Id).Result.FirstOrDefault();
            return userRole;
        }

        public ApplicationUser GetUserById(int userId)
        {
            return UserManager.FindById(userId);
        }

        public Task<AuthenticationServiceResponse> ChangePassword(string OldPassword, string NewPassword)
        {
            var loggedInUserName = GetLoginUserEmail();
            var User = FindUser(loggedInUserName, OldPassword).Result;
            if (User != null)
            {
                UserManager.RemovePassword(User.Id);
                var result = UserManager.AddPassword(User.Id, NewPassword);
                if (result.Succeeded)
                {
                    return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = "Password Changed Successfully" });
                }
                else
                {
                    return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = result.Errors.FirstOrDefault() });
                }

            }
            else
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = "Old Password is incorrect" });
            }

        }
    }
}


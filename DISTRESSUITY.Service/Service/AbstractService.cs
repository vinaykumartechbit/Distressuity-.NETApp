using DISTRESSUITY.DAL.Interfaces;
using DISTRESSUITY.Service.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using System.Security.Claims;

namespace DISTRESSUITY.Service.Services
{
    public abstract class AbstractService : IDisposable
    {
        protected IUnitofWork _unitOfWork;
        protected ApplicationRoleManager _roleManager;
        protected ApplicationUserManager _userManager;
        protected ApplicationSignInManager _signInManager;
        protected IConversationRepository _conversationService;
        public AbstractService()
        {

        }
        public AbstractService(IUnitofWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public AbstractService(IConversationRepository conversationService)
        {
            this._conversationService = conversationService;
            
        }

        public void Dispose()
        {
            //_unitOfWork.Dispose();
        }
        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        protected ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        protected string GetLoginUserName()
        {
            return SignInManager.AuthenticationManager.User.Identity.GetUserName();
        }

        protected string GetLoginUserEmail()
        {
            return SignInManager.AuthenticationManager.User.Identity.Name;
        }
        protected int GetLoginUserId()
        {
            return Convert.ToInt32(SignInManager.AuthenticationManager.User.Identity.GetUserId());
        }

        
    }
}

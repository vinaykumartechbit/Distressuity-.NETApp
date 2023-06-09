using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using DISTRESSUITY.DAL;
using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Authentication
{
    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }
        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new ApplicationUserStore(context.Get<DISTRESSUITYContext>()));

            // Configure validation logic for usernames 
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords 
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = false,
            //    RequireDigit = false,
            //    RequireLowercase = false,
            //    RequireUppercase = false,
            //};

            // Register two factor authentication providers. This application uses Phone 
            // and Emails as a step of receiving a code for verifying the user 
            // You can write your own provider and plug in here. 
            //manager.RegisterTwoFactorProvider("PhoneCode",
            //    new PhoneNumberTokenProvider<ApplicationUser, int>
            //    {
            //        MessageFormat = "Your security code is: {0}"
            //    });

            manager.RegisterTwoFactorProvider("EmailCode",
                new EmailTokenProvider<ApplicationUser, int>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is: {0}"
                });
         //   manager.EmailService = new EmailService();
         //   manager.UserLockoutEnabledByDefault = ReadConfiguration.UserLockoutEnabled;
          //  manager.DefaultAccountLockoutTimeSpan = ReadConfiguration.AccountLockoutTime;
          //  manager.MaxFailedAccessAttemptsBeforeLockout = ReadConfiguration.MaxLoginAttempts;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, int>(
                       dataProtectionProvider.Create("Asp.Net Identity"))
                       {
                           TokenLifespan =TimeSpan.FromHours(7*24)  //ReadConfiguration.EmailTokenExpirationTime
                       };
            }
            return manager;
        }

    }
}

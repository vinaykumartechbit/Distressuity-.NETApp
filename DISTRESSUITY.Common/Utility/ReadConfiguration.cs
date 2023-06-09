using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Common.Utility
{
    public class ReadConfiguration
    {
        public static string WebsiteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebsiteUrl"];
            }
        }
        public static TimeSpan EmailTokenExpirationTime
        {
            get
            {
                return TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["EmailTokenExpirationMinute"]));
            }
        }

        public static string MailTemplateFolder { get { return ConfigurationManager.AppSettings["MailTemplateFolder"]; } }

        public static bool UserLockoutEnabled { get { return AccountLockoutTime.Minutes > 0; } }

        public static int MaxLoginAttempts { get { return Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"]); } }
        public static TimeSpan AccountLockoutTime
        {
            get
            {
                return TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["AccountLockoutMinute"]));
            }
        }
        public static string FacebookKey { get { return ConfigurationManager.AppSettings["fb_key"]; } }
        public static string FacebookSecret { get { return ConfigurationManager.AppSettings["fb_secret"]; } }
        public static string GoogleClientID { get { return ConfigurationManager.AppSettings["google_clientid"]; } }
        public static string GoogleClientSecret { get { return ConfigurationManager.AppSettings["google_clientsecret"]; } }
        public static string MicrosoftID { get { return ConfigurationManager.AppSettings["microsoft_clientid"]; } }
        public static string MicrosoftSecret { get { return ConfigurationManager.AppSettings["microsoft_clientsecret"]; } }

        //Smtp Email keys
        public static string HostName { get { return ConfigurationManager.AppSettings["HostName"]; } }
        public static string FromName { get { return ConfigurationManager.AppSettings["FromName"]; } }
        public static string FromEmail { get { return ConfigurationManager.AppSettings["FromEmail"]; } }
        public static string SmtpAccount { get { return ConfigurationManager.AppSettings["SmtpAccount"]; } }
        public static string SmtpPassword { get { return ConfigurationManager.AppSettings["SmtpPassword"]; } }
        public static bool EnableSSL { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]); } }
        public static int SmtpServerPort { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpServerPort"]); } }
        public static string PageSize { get { return ConfigurationManager.AppSettings["PageSize"]; } }
        public static string InitialPageSize { get { return ConfigurationManager.AppSettings["InitialPageSize"]; } }
        public static string ListingPageSize { get { return ConfigurationManager.AppSettings["ListingPageSize"]; } }
        public static string StorageConnection { get { return ConfigurationManager.AppSettings["StorageConnectionString"]; } }
        public static string ConversationReplySize { get { return ConfigurationManager.AppSettings["ConversationReplySize"]; } }
        public static string FeaturedClickPrice { get { return ConfigurationManager.AppSettings["FeaturedClickPrice"]; } }
        public static TimeSpan TokenExpiration { get { return TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiration"])); } }

        public static decimal SiteCommission { get { return Convert.ToDecimal(ConfigurationManager.AppSettings["SiteCommission"]); } }
        public static string SuperAdminPaypalAccount { get { return ConfigurationManager.AppSettings["SuperAdminPaypalAccount"]; } }
        public static string MiscellaneousIndustry { get { return ConfigurationManager.AppSettings["MiscellaneousIndustry"]; } }

    }
}


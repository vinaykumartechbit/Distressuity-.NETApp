using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace DISTRESSUITY.Common.Utility
{
    public class CommonFunction
    {

        public static string GetWebSiteUrl()
        {
            Uri _url = HttpContext.Current.Request.Url;
            return _url.Scheme + "://" + _url.Authority;
        }
        public static string GetConfirmAccountUrl(string userid, string code)
        {
            string relativePath = VirtualPathUtility.ToAbsolute("~/");
            return GetWebSiteUrl() + relativePath + "#/setpassword/" + userid + "?Key=" + code;
            //return GetWebSiteUrl() + relativePath + "#/setpassword?userid=" + userid + "&code=" + code;
        }

        public static string ConfigureConfirmAccountMailBodyDb(string url)
        {
            string body = string.Empty;

            body += "<html><body><p> Your Account has been successfully registered on Distressuity.<p> </br> Please confirm your account "
                  + "and set your password by <a href=" + url + "> clicking here</a></body> </html>";
            return body;
        }

        public static string GetForgetPasswordUrl(string userid, string code)
        {
            string relativePath = VirtualPathUtility.ToAbsolute("~/");
            return GetWebSiteUrl() + relativePath + "#/resetpassword/" + userid + "?Key=" + code;
            //return GetWebSiteUrl() + relativePath + "#/setpassword?userid=" + userid + "&code=" + code;
        }

        public static string ConfigureForgetPasswordMailBodyDb(string url)
        {
            string body = string.Empty;

            body += "<html><body><p> You have successfully requested for forget password. <p> </br> Please reset your password by "
                  + "<a href=" + url + "> clicking here</a></body> </html>";
            return body;
        }


    }


    public static class Helper
    {
        public static string toDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        public static DateTime toDateTime(this string dateTime)
        {
            return DateTime.ParseExact(dateTime, "yyyy-MM-dd", null);
        }

        public static string[] videoExtensions = {
             ".MPEG", ".MOV", ".AVI", ".3GP", ".MP4"
        };
        public static string[] imageExtensions = {
             ".PNG", ".JPG"
        };

        public static bool isVideoFile(this string path)
        {
            return videoExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public static bool isImageFile(this string path)
        {
            return imageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        public static string Encrypt(this Int32 toEncrypt)
        {
            if (toEncrypt == 0)
                return "0";

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt.ToString());

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            string key = "App";
            // (string)settingsReader.GetValue("SecurityKey",
            //                                 typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            ////If hashing use get hashcode regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            //hashmd5.Clear();
            //else
            //keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format

            return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("/", "^").Replace("+", "~");

        }

        public static int Decrypt(this string cipherString)
        {
            int checkInt = 0;
            if (string.IsNullOrEmpty(cipherString) || (int.TryParse(cipherString, out checkInt) && checkInt == 0))
                return 0;

            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace("^", "/").Replace("~", "+"));

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = "App";
            //(string)settingsReader.GetValue("SecurityKey",
            //                                       typeof(String));

            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            ////release any resource held by the MD5CryptoServiceProvider

            //hashmd5.Clear();

            //else
            //{
            //if hashing was not implemented get the byte code of the key
            // keyArray = UTF8Encoding.UTF8.GetBytes(key);
            //}

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            string result = UTF8Encoding.UTF8.GetString(resultArray);

            int iresult = 0;
            if (int.TryParse(result, out iresult))
                return iresult;

            return -1;
        }
    }
}

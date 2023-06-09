using System;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using DISTRESSUITY.Service.Model;
using DISTRESSUITY.Service.Interfaces;
using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.Service.Service;
using System.Threading;
using System.Net;
using DISTRESSUITY.DAL.Entities;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using DISTRESSUITY.Common.Utility;
using Microsoft.Owin.Security.Cookies;
using DISTRESSUITY.Common.Constants;
using System.Drawing;
using System.Drawing.Imaging;

namespace DISTRESSUITY.Web.Controllers
{

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        #region Parameters
        private IAuthenticationService _authenticationService;
        private const string LocalLoginProvider = "Local";
        #endregion

        #region Constructors
        public AccountController(IAuthenticationService AuthenticationService)
        {
            _authenticationService = AuthenticationService;
        }
        public AccountController()
        {
        }
        #endregion

        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<AuthenticationServiceResponse> Register(UserModel model)
        {
            if (!ModelState.IsValid && model == null)
            {
                return new AuthenticationServiceResponse() { Data = model, Success = false, Message = "ModelState.Valid is false" };
            }

            var response = await _authenticationService.CreateUser(model);
            return response;
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogins(string provider, string error = null)
        {
            string redirectUri = string.Empty;
            var listPositionModel = new List<UserPositionsModel>();

            try
            {
                if (error != null)
                {
                    return BadRequest(Uri.EscapeDataString(error));
                }

                if (!User.Identity.IsAuthenticated)
                {
                    return new ChallengeResult(provider, this);
                }

                var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);


                if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
                {
                    return BadRequest(redirectUriValidationResult);
                }



                ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

                if (externalLogin == null)
                {
                    return InternalServerError();
                }

                if (externalLogin.LoginProvider != provider)
                {
                    _authenticationService.SignOut();
                    return new ChallengeResult(provider, this);
                }

                var user = await _authenticationService.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

                bool hasRegistered = user != null;

                if (!hasRegistered)
                {
                    var isEmailExist = await _authenticationService.FindByNameAsync(externalLogin.Email);
                    if (isEmailExist != null)
                    {
                        redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&error_message={5}",
                                           redirectUri,
                                           externalLogin.ExternalAccessToken,
                                           externalLogin.LoginProvider,
                                           hasRegistered.ToString(),
                                           externalLogin.Email,
                                           "Email has already regestired! Please provide password for login");
                        return Redirect(redirectUri);
                    }
                }

                var extraData = GetProfileData(externalLogin.ExternalAccessToken);
                externalLogin.FirstName = extraData.firstName;
                externalLogin.LastName = extraData.lastName;

                string Industry = extraData.industry;
                var industry = new IndustryModel();
                if (Industry != null)
                {
                    industry = _authenticationService.GetIndustryByName(Industry);
                }

                if (user == null )
                {
                    if (extraData.positions.values != null)
                    {
                        foreach (var data in extraData.positions.values)
                        {
                            var positionModel = new UserPositionsModel();
                            positionModel.Title = data.title;
                            positionModel.StartMonth = data.startDate!=null?data.startDate.month:0;
                            positionModel.StartYear = data.startDate!=null?data.startDate.year:0;
                            positionModel.CompanyName = data.company != null ? data.company.name : "";
                            positionModel.Description = data.summary;
                            positionModel.IsCurrent = data.isCurrent;
                            listPositionModel.Add(positionModel);
                        }
                    }
                    string pictureUrl = extraData.pictureUrl;
                    var imageName = pictureUrl!=null?DownlodImageFromLinkedin(pictureUrl, externalLogin.Email):"";

                    var userModel = new UserModel()
                    {
                        FirstName = externalLogin.FirstName,
                        LastName = externalLogin.LastName,
                        Email = externalLogin.Email,
                        PictureUrl = ImagePathConstants.ProfileImage_Folder + imageName,
                        //PictureUrl = extraData.pictureUrl,
                        Summary = extraData.summary,
                        LoginProvider = externalLogin.LoginProvider,
                        ProviderKey = externalLogin.ProviderKey,
                        UserPositions = listPositionModel

                    };
                    if (Industry != null)
                    {
                        userModel.IndustryId = industry.IndustryId;
                    }
                    var response = await _authenticationService.CreateUser(userModel, true);
                    if (response.Success)
                    {
                        hasRegistered = true;
                    }
                }

                var claimsIdentity = _authenticationService.ExternalLogin("ExternalCookie", externalLogin.Email, false);
                var tokenResponse = GenerateLocalAccessTokenResponse(externalLogin.Email, claimsIdentity);
                var loginModel = new LoginModel();
                loginModel.UserName = externalLogin.Email;


                externalLogin.accessToken = (string)tokenResponse["access_token"];
                redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&access_token={5}",
                                           redirectUri,
                                           externalLogin.ExternalAccessToken,
                                           externalLogin.LoginProvider,
                                           hasRegistered,
                                           externalLogin.Email,
                                           externalLogin.accessToken);

                return Redirect(redirectUri);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string DownlodImageFromLinkedin(string path, string imageName)
        {
            imageName = imageName + ".Jpeg";
            var guId = Guid.NewGuid();
            var savePath = HttpContext.Current.Server.MapPath(@ImagePathConstants.ProfileImage_Folder);
            savePath = savePath + imageName;
            //var bitmapImage = new Bitmap(path);
            //bitmapImage.Save(savePath, ImageFormat.Jpeg);
            //var textFromFile = (new WebClient()).DownloadString(path); // get as string

            (new WebClient()).DownloadFile(new Uri(path), savePath); // or save to file
            return imageName;
        }

        [HttpGet]
        [Route("Logout")]
        public IHttpActionResult Signout()
        {
            _authenticationService.SignOut();
            return Ok();
        }

        [HttpPost]
        [Route("ConfirmEmail", Name = "ConfirmEmail")]
        public async Task<AuthenticationServiceResponse> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return new AuthenticationServiceResponse() { Success = false, Message = "ModelState.Valid is false" };
            }

            int Id = string.IsNullOrEmpty(userId) ? 0 : userId.Decrypt();
            var response = await _authenticationService.ConfirmAccount(Id, code);
            return response;
        }

        [HttpPost]
        [Route("SetPassword")]
        public async Task<AuthenticationServiceResponse> SetPassword(SetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = string.IsNullOrEmpty(model.UserId) ? 0 : model.UserId.Decrypt();
                var result = await _authenticationService.SetPassword(userId, model.Password);
                return result;
            }
            return new AuthenticationServiceResponse() { Data = model, Success = false, Message = "Invalid model state" };

        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<AuthenticationServiceResponse> ForgetPassword(ForgetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.ForgetUser(model);
                return result;
            }
            return new AuthenticationServiceResponse() { Data = model, Success = false, Message = "Invalid model state" };
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<AuthenticationServiceResponse> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = string.IsNullOrEmpty(model.UserId) ? 0 : model.UserId.Decrypt();
                var result = await _authenticationService.ResetPassword(userId, model.Password);
                return result;
            }
            return new AuthenticationServiceResponse() { Data = model, Success = false, Message = "Invalid model state" };

        }

       



        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string accessToken { get; set; }


            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("urn:linkedin:accesstoken"),
                    Email = identity.FindFirstValue(ClaimTypes.Email)
                };
            }
        }

        internal class ChallengeResult : IHttpActionResult
        {
            public string LoginProvider { get; set; }
            public HttpRequestMessage Request { get; set; }

            public ChallengeResult(string loginProvider, ApiController controller)
            {
                LoginProvider = loginProvider;
                Request = controller.Request;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                Request.GetOwinContext().Authentication.Challenge(LoginProvider);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.RequestMessage = Request;
                return Task.FromResult(response);
            }
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            //var clientId = GetQueryString(Request, "client_id");

            //if (string.IsNullOrWhiteSpace(clientId))
            //{
            //    return "client_Id is required";
            //}

            //var client = _repo.FindClient(clientId);

            //if (client == null)
            //{
            //    return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            //}

            //if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            //{
            //    return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            //}

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        //get access token response
        private JObject GenerateLocalAccessTokenResponse(string userName, ClaimsIdentity identity)
        {

            var tokenExpiration =ReadConfiguration.TokenExpiration;
            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }
        private async Task<ExternalLoginModels.ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ExternalLoginModels.ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                var appToken = "xxxxx";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ExternalLoginModels.ParsedExternalAccessToken();

                //if (provider == "Facebook")
                //{
                //    parsedToken.user_id = jObj["data"]["user_id"];
                //    parsedToken.app_id = jObj["data"]["app_id"];

                //    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                //    {
                //        return null;
                //    }
                //}
                //else if (provider == "Google")
                //{
                //    parsedToken.user_id = jObj["user_id"];
                //    parsedToken.app_id = jObj["audience"];

                //    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                //    {
                //        return null;
                //    }

                //}

            }

            return parsedToken;
        }
        private dynamic GetProfileData(string token)
        {


            //  string authUrl = "https://www.linkedin.com/uas/oauth2/accessToken";

            string url = "https://api.linkedin.com/v1/people/~:(id,first-name,email-address,last-name,headline,picture-url,industry,summary,specialties,positions:(id,title,summary,start-date,end-date,is-current,company:(id,name,type,size,industry,ticker)),educations:(id,school-name,field-of-study,start-date,end-date,degree,activities,notes),associations,interests,num-recommenders,date-of-birth,publications:(id,title,publisher:(name),authors:(id,name),date,url,summary),patents:(id,title,summary,number,status:(id,name),office:(name),inventors:(id,name),date,url),languages:(id,language:(name),proficiency:(level,name)),skills:(id,skill:(name)),certifications:(id,name,authority:(name),number,start-date,end-date),courses:(id,name,number),recommendations-received:(id,recommendation-type,recommendation-text,recommender),honors-awards,three-current-positions,three-past-positions,volunteer)";

            var sign = "oauth2_access_token=" + HttpUtility.UrlEncode(token) + "&format=json";
            //byte[] byteArray = Encoding.UTF8.GetBytes(sign);



            // byte[] msg = Encoding.Default.GetBytes(doc.OuterXml);
            // client.AddPostContent(msg);
            // Create the web request  
            HttpWebRequest request = WebRequest.Create(url + "?" + sign) as HttpWebRequest;

            // System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            dynamic jsonResponse;
            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());
                jsonResponse = JsonConvert.DeserializeObject(Convert.ToString(reader.ReadToEnd()));
            }
            return jsonResponse;
        }

        #endregion
    }
}

using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.Service.Interfaces;
using DISTRESSUITY.Service.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DISTRESSUITY.Common.Utility;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.Drawing;
using DISTRESSUITY.Common.Constants;
using DISTRESSUITY.Common.Enums;
using PayPal.Api;
using log4net.Repository.Hierarchy;
using System.Text.RegularExpressions;
using DISTRESSUITY.Service.Services;
using PayPal.AdaptiveAccounts.Model;
using PayPal.AdaptiveAccounts;
using PayPal.AdaptivePayments.Model;
using PayPal.AdaptivePayments;

namespace DISTRESSUITY.Web.Controllers
{
    //[Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        #region Parameters
        private IUserService _userService;
        public static string ClientId;
        public static string ClientSecret;
        #endregion

        #region Constructors
        public UserController(IUserService UserService)
        {
            _userService = UserService;
        }
        public UserController()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        #endregion

        #region CommonMethods
        [HttpGet]
        [Route("GetIndustries")]
        public async Task<List<IndustryModel>> GetIndustries()
        {
            var response = await _userService.GetIndustries();
            return response;
        }
        #endregion

        #region HelperMethods

        public class MyStreamProvider : MultipartFormDataStreamProvider
        {
            public MyStreamProvider(string uploadPath)
                : base(uploadPath)
            {

            }
            public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
            {
                // override the filename which is stored by the provider (by default is bodypart_x)
                string oldfileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                string newFileName = Path.GetFileNameWithoutExtension(oldfileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(oldfileName);

                return newFileName;
            }
        }

        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        #endregion

        #region Project

        #region GetMethods

        [HttpGet]
        [Route("GetProjects")]
        public async Task<IHttpActionResult> GetProjects()
        {
            int featuredProjectsCount;
            int projectsCount;
            var result = await _userService.GetProjects(out projectsCount);
            //var projectsCount = await _userService.GetProjectCount(null);
            var featuredProjects = await _userService.GetFeaturedProjects(out featuredProjectsCount);
            var Industries = await _userService.GetIndustries();
            //var featuredProjectsCount = await _userService.GetFeaturedProjectsCount(null);
            return Ok(new { result, projectsCount, featuredProjects, Industries, featuredProjectsCount });
        }

        [HttpGet]
        [Route("GetAllProject")]
        public async Task<IHttpActionResult> GetAllProject(int PageNumber, int PageSize)
        {
            var AllProjectCount = 0;
            var result = await _userService.GetAllProjects(PageNumber, PageSize, out AllProjectCount);
            var statusList = await _userService.getStatusList();
            return Ok(new { result, statusList, AllProjectCount });
        }


        [HttpGet]
        [Route("GetFundedProject")]
        public async Task<IHttpActionResult> GetFundedProject(int PageNumber, int PageSize)
        {
            var ProjectCount = 0;
            var result = await _userService.GetFundedProject(PageNumber, PageSize, out ProjectCount);
            return Ok(new { result, ProjectCount });
        }

        [HttpGet]
        [Route("GetFilteredFundedProjectList")]
        public async Task<IHttpActionResult> GetFilteredFundedProjectList(DateTime? StartDate, DateTime? EndDate)
        {
            var result = await _userService.GetFilteredFundedProjectList(StartDate, EndDate);
            return Ok(new { result });
        }


        [HttpGet]
        //[Route("GetAllProjetcsByStatus/{statusId}/{searchKeyword}")]
        [Route("GetAllProjetcsByStatus")]
        public async Task<IHttpActionResult> GetAllProjetcsByStatus(string statusId, string searchKeyword)
        {
            var result = await _userService.GetAllProjetcsByStatus(statusId, searchKeyword);
            return Ok(result);
        }

        [HttpGet]
        [Route("LoadMoreProjects")]
        public async Task<IHttpActionResult> LoadMoreProjects(int pageNumber, string name)
        {
            var result = await _userService.LoadMoreProjects(pageNumber, name);
            return Ok(result);
        }

        [HttpGet]
        [Route("LoadMoreFeaturedProjects")]
        public async Task<IHttpActionResult> LoadMoreFeaturedProjects(int pageNumber, string name)
        {
            var result = await _userService.LoadMoreFeaturedProjects(pageNumber, name);
            return Ok(result);
        }

        [HttpGet]
        [Route("LoadMoreMyProjects")]
        public async Task<IHttpActionResult> LoadMoreMyProjects(int pageNumber, string name)
        {
            var result = await _userService.LoadMoreMyProjects(pageNumber, name);
            return Ok(result);
        }

        [HttpGet]
        [Route("SearchProjects")]
        public async Task<IHttpActionResult> SearchProjects(string name, string Id)
        {
            int projectsCount = 0;
            int featuredProjectsCount = 0;
            var result = await _userService.SearchProjects(name, Id, out projectsCount);
            // var projectsCount = await _userService.GetProjectCount(name);
            var featuredProjects = await _userService.SearchFeaturedProjects(name, Id, out featuredProjectsCount);
            // var featuredProjectsCount = await _userService.GetFeaturedProjectsCount(name);
            return Ok(new { result, projectsCount, featuredProjects, featuredProjectsCount });
        }

        [HttpGet]
        [Route("SearchMyProjects")]
        public async Task<IHttpActionResult> SearchMyProjects(string name, string Id)
        {
            int projectsCount = 0;
            var result = await _userService.SearchMyProjects(name, Id, out projectsCount);
            //var projectsCount = await _userService.GetMyProjectCount(name);
            //var projectsCount = await _userService.GetProjectCount(name);
            return Ok(new { result, projectsCount });
        }


        [HttpGet]
        [Route("SearchAdminProjects")]
        public async Task<IHttpActionResult> SearchAdminProjects(string name, string statusId)
        {
            var result = await _userService.SearchAdminProjects(name, statusId);
            //var projectsCount = await _userService.GetMyProjectCount(name);
            //var projectsCount = await _userService.GetProjectCount(name);
            return Ok(new { result });
        }



        [HttpGet]
        [Route("GetProjectsByIndustry")]
        public async Task<IHttpActionResult> GetProjectsByIdustry(string id)
        {
            var result = new List<ProjectModel>();
            int projectsCount;
            var featuredProjects = new List<ProjectModel>();
            int featuredProjectsCount;
            if (id.Equals(0))
            {
                result = await _userService.GetProjects(out projectsCount);
                //projectsCount = await _userService.GetProjectCount(null);
                featuredProjects = await _userService.GetFeaturedProjects(out featuredProjectsCount);
                //featuredProjectsCount = await _userService.GetFeaturedProjectsCount(null);
            }
            else
            {
                result = await _userService.FilterProjectsByIndustryId(id, out projectsCount);
                //projectsCount = await _userService.GetProjectCountByIndustryId(id);
                featuredProjects = await _userService.FilterFeaturedProjectsByIndustryId(id, out featuredProjectsCount);
                //featuredProjectsCount = await _userService.GetFeaturedCountByIndustryId(id);
            }

            return Ok(new { result, projectsCount, featuredProjects, featuredProjectsCount });
        }

        [HttpGet]
        [Route("GetMyProjects")]
        public async Task<IHttpActionResult> GetMyProjects()
        {
            var result = await _userService.GetMyProjects();
            var projectsCount = await _userService.GetMyProjectCount(null);
            return Ok(new { result, projectsCount });
        }



        //[Authorize]
        [HttpGet]
        [Route("GetProjectById")]
        public async Task<IHttpActionResult> GetProjectById(string id, int pageType)
        {
            if (id.Equals(null))
            {
                return BadRequest("Invalid ProjectId");
            }
            var project = await _userService.GetProjectById(id, pageType);
            var industries = await _userService.GetIndustries();
            var months = _userService.GetMonths();
            var loggedInUserFunding = _userService.GetLoggedInUserFunding(id);
            var ifLoggedInUserIsProjectOwner = _userService.GetIfLoggedInUserIsProjectOwner(id, pageType);
            return Ok(new { project, industries, months, loggedInUserFunding, ifLoggedInUserIsProjectOwner });
        }

        [HttpGet]
        [Route("GetProjectTabEnums")]
        public IHttpActionResult GetProjectTabEnums()
        {

            var projectTabEnums = _userService.GetProjectTabEnums();
            return Ok(new { projectTabEnums });
        }

        [HttpGet]
        [Route("GetIfLogginInUserCanPay/{projectId}")]
        public IHttpActionResult GetIfLogginInUserCanPay(string projectId)
        {
            var ifUserCanPay = _userService.GetIfLogginInUserCanPay(projectId);

            return Ok(new { ifUserCanPay });
        }


        [HttpGet]
        [Route("CurrentUserPaypalAccounts")]

        public async Task<IHttpActionResult> CurrentUserPaypalAccounts()
        {
            var AccountList = _userService.CurrentUserPaypalAccounts();
            return Ok(AccountList);
        }




        #endregion

        #region AddMethods

        [Authorize]
        [HttpPost]
        [Route("AddProject")]
        public async Task<IHttpActionResult> AddProject(ProjectModel projectModel)
        {
            try
            {
                var logoPath = projectModel.LogoPath.Substring(projectModel.LogoPath.LastIndexOf("/") + 1);
                var logoName = logoPath.Substring(0, logoPath.LastIndexOf("_"));
                //var imagePath = projectModel.ImagePath.Substring(projectModel.ImagePath.LastIndexOf("/") + 1);
                //var imageName = imagePath.Substring(0, imagePath.LastIndexOf("_"));
                string oldPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.TempFiles_Folder + logoPath);
                var folderPath = string.Empty;
                var newPath = string.Empty;

                if (projectModel.ProjectId == null)
                {
                    projectModel = await _userService.AddUpdateProject(projectModel);
                    folderPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt());
                    if (!Directory.Exists(folderPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(folderPath);
                    }
                    newPath = folderPath + "/" + logoPath;
                    if (File.Exists(oldPath))
                        File.Copy(oldPath, newPath);
                    projectModel.LogoPath = ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt() + "/" + logoPath;
                    projectModel.LogoName = logoName;
                    projectModel = await _userService.AddUpdateProject(projectModel);
                    return Ok(projectModel);
                }
                else
                {
                    if (projectModel.TabType != Convert.ToInt32(DISTRESSUITY.Common.Enums.CommonEnums.ProjectTabEnums.BasicDetail))
                    {
                        projectModel = await _userService.AddUpdateProject(projectModel);
                        return Ok(projectModel);
                    }
                    //var tempPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.TempFiles_Folder + imagePath);
                    if (!File.Exists(oldPath))
                    {
                        projectModel = await _userService.AddUpdateProject(projectModel);
                        return Ok(projectModel);
                    }
                    var project = await _userService.GetProjectById(projectModel.ProjectId, (int)Common.Enums.CommonEnums.GetProjectByType.ViewProject);
                    var oldImage = HttpContext.Current.Server.MapPath(project.LogoPath);
                    if (File.Exists(oldImage))
                        File.Delete(oldImage);
                    folderPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt());
                    if (!Directory.Exists(folderPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(folderPath);
                    }
                    newPath = folderPath + "/" + logoPath;
                    File.Move(oldPath, newPath);
                    projectModel.LogoPath = ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt() + "/" + logoPath;
                    projectModel.LogoName = logoName;
                    projectModel = await _userService.AddUpdateProject(projectModel);
                    return Ok(projectModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //[Route("AddProject")]
        //public async Task<IHttpActionResult> AddUpdateProject()
        //{
        //    var projectModel = new ProjectModel();
        //    try
        //    {
        //        if (Request.Content.IsMimeMultipartContent())
        //        {
        //            string uploadPath = HttpContext.Current.Server.MapPath("~/images");

        //            MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
        //            var result = await Request.Content.ReadAsMultipartAsync(streamProvider);

        //            var data = GetFormData<ProjectModel>(result);
        //            projectModel = (ProjectModel)data;
        //            if (projectModel.ProjectId.Equals(0))
        //            {
        //                projectModel = await _userService.AddUpdateProject(projectModel);
        //            }

        //            var path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectModel.ProjectId);
        //            if (!Directory.Exists(path))
        //            {
        //                DirectoryInfo di = Directory.CreateDirectory(path);
        //            }

        //            foreach (var file in streamProvider.FileData)
        //            {
        //                FileInfo fi = new FileInfo(file.LocalFileName);

        //                if (fi.Name.isVideoFile())
        //                {
        //                    if (projectModel.VideoPath != null)
        //                        _userService.DeleteFile(projectModel.VideoPath);
        //                    projectModel.VideoPath = "/ProjectFiles/Project_" + projectModel.ProjectId + "/" + fi.Name;

        //                    projectModel.VideoName = fi.Name.Substring(0, fi.Name.LastIndexOf("_"));
        //                }
        //                if (fi.Name.isImageFile())
        //                {
        //                    if (projectModel.ImagePath != null)
        //                        _userService.DeleteFile(projectModel.ImagePath);
        //                    projectModel.ImagePath = "/ProjectFiles/Project_" + projectModel.ProjectId + "/" + fi.Name;

        //                    projectModel.ImageName = fi.Name.Substring(0, fi.Name.LastIndexOf("_"));
        //                }
        //                if (File.Exists(file.LocalFileName))
        //                {
        //                    path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectModel.ProjectId + "/" + fi.Name);
        //                    File.Move(file.LocalFileName, path);
        //                }
        //            }
        //            await _userService.AddUpdateProject(projectModel);
        //            return Ok(projectModel);
        //        }
        //        else
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
        //            throw new HttpResponseException(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        [Route("UploadDocument")]
        public async Task<IHttpActionResult> AddProjectDocument()
        {
            try
            {
                var projectDocumentModel = new ProjectDocumentModel();
                if (!Request.Content.IsMimeMultipartContent())
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
                //string uploadPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.Image_Folder);
                string uploadPath = HttpContext.Current.Server.MapPath("~/TempFiles");


                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var result = await Request.Content.ReadAsMultipartAsync(streamProvider);

                projectDocumentModel.ProjectId = result.FormData["ProjectId"];

                if (projectDocumentModel.ProjectId.Equals(0))
                {
                    foreach (var file in streamProvider.FileData)
                    {
                        File.Delete(file.LocalFileName);
                    }
                    return BadRequest("Invalid ProjectId");
                }

                var path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectDocumentModel.ProjectId.Decrypt());
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                FileInfo fi = new FileInfo(streamProvider.FileData.FirstOrDefault().LocalFileName);
                if (File.Exists(streamProvider.FileData.FirstOrDefault().LocalFileName))
                {
                    var length = fi.Length;
                    var measurementUnit = "Kb";
                    if (length > 1048576)
                    {
                        length = length / 1048576;
                        measurementUnit = "Mb";
                    }
                    path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectDocumentModel.ProjectId.Decrypt() + "/" + fi.Name);
                    File.Move(streamProvider.FileData.FirstOrDefault().LocalFileName, path);
                    projectDocumentModel.DocumentName = fi.Name.Substring(0, fi.Name.LastIndexOf("_")) + Path.GetExtension(fi.Name);
                    projectDocumentModel.DocumentPath = "/ProjectFiles/Project_" + projectDocumentModel.ProjectId.Decrypt() + "/" + fi.Name;
                    projectDocumentModel.DocumentSize = length + " " + measurementUnit;
                    projectDocumentModel = await _userService.AddUpdateProjectDocument(projectDocumentModel);
                    return Ok(projectDocumentModel);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //throw ex;
            }
        }

        [HttpPost]
        [Route("AddTempImage")]
        public async Task<IHttpActionResult> AddTempImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }

            var userId = User.Identity.GetUserId();
            string uploadPath = HttpContext.Current.Server.MapPath("~/TempFiles");

            if (!Directory.Exists(uploadPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(uploadPath);
            }

            MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
            var result = await Request.Content.ReadAsMultipartAsync(streamProvider);
            FileInfo fi = new FileInfo(streamProvider.FileData.FirstOrDefault().LocalFileName);
            var fileName = "/TempFiles/" + fi.Name;
            return Ok(fileName);

        }

        private void DrawBitmapWithBorder(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                var pos = new Point();
                pos.X = 200;
                pos.Y = 200;
                var g = Graphics.FromImage(bmp);
                int size = 300;
                bmp = new Bitmap(bmp.Width + size * 2, bmp.Height + size * 2);
                //const int borderSize = 20;
                //using (Brush border = new SolidBrush(Color.White /* Change it to whichever color you want. */))
                //{
                //    g.FillRectangle(border, pos.X - borderSize, pos.Y - borderSize,
                //      bmp.Width + borderSize, bmp.Height + borderSize);
                //}
                //g.DrawImage(bmp, pos);
                //bmp.Save(path);

                //Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                int x = (bmp.Width - 300) / 2;
                int y = (bmp.Height - 200) / 2;
                g.DrawImage(bmp, new Point(x, y));
                bmp.Save(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion

        #region DeleteMethods
        [HttpGet]
        [Route("DeleteMyProject")]
        public async Task<IHttpActionResult> DeleteMyProject(string id)
        {
            var result = await _userService.DeleteProject(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("DeleteProjectDocument")]
        public async Task<IHttpActionResult> DeleteProjectDocument(ProjectDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid ModelState");
            }
            var result = await _userService.DeleteProjectDocument(model);
            return Ok(result);
        }


        #endregion


        #region Verification
        [HttpPost]
        [Route("VerifyPaypalAccount")]
        public async Task<IHttpActionResult> VerifyPaypalAccount(ProjectModel projectModel)
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //System.Net.ServicePointManager.CertificatePolicy = new DISTRESSUITY.Web.Providers.TrustAllCertificatePolicy();
          
            GetVerifiedStatusRequest getVerifiedStatusRequest = new GetVerifiedStatusRequest
            {
                emailAddress = projectModel.PaypalAccount,
                matchCriteria = "NONE"
            };
            AdaptiveAccountsService service = new AdaptiveAccountsService(GetConfig());
            try
            {
                //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                //System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
                GetVerifiedStatusResponse response = service.GetVerifiedStatus(getVerifiedStatusRequest);
                if (response.accountStatus == CommonEnums.PaypalAccountStatus.VERIFIED.ToString())
                {
                    projectModel.PaypalAccountStatus = true;
                }
                else
                {
                    projectModel.PaypalAccountStatus = false;
                }
            }
            catch (Exception ex)
            {
                return Ok(new { result = "failed", Message = "Something went worng" }); 
            }



            return Ok(projectModel);
        }

        #endregion


        #region UpdateMethods
        [HttpPost]
        [Route("UpdateProject")]
        public async Task<IHttpActionResult> UpdateProject(ProjectModel projectModel)
        {
            if(projectModel.TabType==3)
            {
                GetVerifiedStatusRequest getVerifiedStatusRequest = new GetVerifiedStatusRequest
                {
                    emailAddress = projectModel.PaypalAccount,
                    matchCriteria = "NONE"
                };
                AdaptiveAccountsService service = new AdaptiveAccountsService(GetConfig());
                try
                {
                    GetVerifiedStatusResponse response = service.GetVerifiedStatus(getVerifiedStatusRequest);
                    if (response.accountStatus == CommonEnums.PaypalAccountStatus.VERIFIED.ToString())
                    {
                        projectModel.PaypalAccountStatus = true;
                    }
                    else
                    {
                        projectModel.PaypalAccountStatus = false;
                        return Ok(new { result = "failed", Message = "PayPal Account is not correct" });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new { result = "failed", Message = "Something went worng" }); 
                }
                //if(projectModel.PaypalAccountStatus==false)
                //{
                    
                //}
            }
            
            var imagePath = projectModel.ImagePath != null ? projectModel.ImagePath.Substring(projectModel.ImagePath.LastIndexOf("/") + 1) : "";
            var imageName = imagePath != "" ? imagePath.Substring(0, imagePath.LastIndexOf("_")) : "";
            string oldPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.TempFiles_Folder + imagePath);
            var folderPath = string.Empty;
            var newPath = string.Empty;
            if (projectModel.ProjectId == null)
            {
                return BadRequest("Invalid request");
            }
            else
            {
                projectModel.ImagePath = ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt() + "/" + imagePath;
                projectModel.ImageName = imageName;
                //if (!File.Exists(oldPath))
                //{
                //    projectModel = await _userService.AddUpdateProject(projectModel);
                //    return Ok(projectModel);
                //}
                if (!File.Exists(oldPath))
                {
                    projectModel = await _userService.AddUpdateProject(projectModel);
                    return Ok(projectModel);
                }
                var project = await _userService.GetProjectById(projectModel.ProjectId, (int)Common.Enums.CommonEnums.GetProjectByType.ViewProject);
                var oldImage = HttpContext.Current.Server.MapPath(project.ImagePath);
                if (File.Exists(oldImage))
                    File.Delete(oldImage);
                folderPath = HttpContext.Current.Server.MapPath(@ImagePathConstants.ProjectFiles_Folder + projectModel.ProjectId.Decrypt());
                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(folderPath);
                }
                newPath = folderPath + "/" + imagePath;
                File.Move(oldPath, newPath);

            }
            var result = await _userService.AddUpdateProject(projectModel);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateFundingStatus/{projectFundingId}/{isApproved}")]
        public async Task<IHttpActionResult> UpdateFundingStatus(string projectFundingId, bool IsApproved)
        {
            var result = await _userService.UpdateFundingStatus(projectFundingId, IsApproved);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateProjectStatus/{projectId}/{Status}")]
        public async Task<IHttpActionResult> UpdateProjectStatus(string projectId, bool Status)
        {
            var result = await _userService.UpdateProjectStatus(projectId, Status);
            return Ok(result);
        }

        [HttpGet]
        [Route("SubmitProjectReview")]
        public async Task<IHttpActionResult> SubmitProjectReview(string id)
        {
            if (id.Equals(null))
            {
                return BadRequest("Invalid ProjectId");
            }
            var result = await _userService.SubmitProjectReview(id);
            return Ok(result);
        }
        [HttpPost]
        [Route("UploadProjectVideo")]
        public async Task<IHttpActionResult> UploadProjectVideo()
        {
            try
            {
                var projectModel = new ProjectModel();
                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }
                string uploadPath = HttpContext.Current.Server.MapPath("~/TempFiles");

                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var result = await Request.Content.ReadAsMultipartAsync(streamProvider);
                //var projectId = GetFormData<string>(result);
                var projectId = Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault() ?? String.Empty);
                projectModel.ProjectId = result.FormData["ProjectId"];
                projectModel = await _userService.GetProjectById(projectModel.ProjectId, (int)Common.Enums.CommonEnums.GetProjectByType.EditProject);
                var path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt());
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                FileInfo fi = new FileInfo(streamProvider.FileData.FirstOrDefault().LocalFileName);
                if (File.Exists(streamProvider.FileData.FirstOrDefault().LocalFileName))
                {
                    //if (fi.Name.Split('.')[1] == "3gp" || fi.Name.Split('.')[1] == "mp4" || fi.Name.Split('.')[1] == "3g2")
                    if (fi.Name.Split('.')[1] == "mp4")
                        path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name);
                    else
                        path = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name.Remove(fi.Name.IndexOf(".")) + ".webm");
                    if (projectModel.VideoPath != null)
                        _userService.DeleteFile(projectModel.VideoPath);
                    var extension = Path.GetExtension(streamProvider.FileData.FirstOrDefault().LocalFileName);
                    if (Path.GetExtension(path) != ".mp4")
                    {
                        await _userService.ConvertVideoService(streamProvider.FileData.FirstOrDefault().LocalFileName, fi.Name.Split('.')[1], path);
                        //ReturnVideo(streamProvider, fi.Name, projectModel.ProjectId.Decrypt());
                    }
                    else
                    {
                        File.Move(streamProvider.FileData.FirstOrDefault().LocalFileName, path);
                    }
                    projectModel.VideoName = fi.Name.Substring(0, fi.Name.LastIndexOf("_"));
                    if (fi.Name.Split('.')[1] == "mp4")
                        projectModel.VideoPath = "/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name;
                    else
                        projectModel.VideoPath = "/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name.Remove(fi.Name.IndexOf(".")) + ".webm";
                    //projectModel.VideoPath = "/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name;
                    //projectModel.VideoPath = "/ProjectFiles/Project_" + projectModel.ProjectId.Decrypt() + "/" + fi.Name; 
                    var updateProjectModel = await _userService.AddUpdateProject(projectModel);
                    return Ok(updateProjectModel);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //throw ex;
            }
        }
        private bool ReturnVideo(MyStreamProvider streamProvider, string fileName, int projectId)
        {

            try
            {
                string html = string.Empty;
                //rename if file already exists

                int j = 0;
                string AppPath;
                string inputPath;
                string outputPath;
                string imgpath;
                AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
                //Get the application path
                inputPath = AppPath + "ProjectFiles\\Project_" + projectId;
                //inputPath = streamProvider.FileData.FirstOrDefault().LocalFileName;
                //Path of the original file
                outputPath = AppPath + "ProjectFiles\\Project_" + projectId;
                //Path of the converted file
                imgpath = AppPath + "ProjectFiles\\Project_" + projectId;
                //Path of the preview file
                //string filepath = HttpContext.Current.Server.MapPath("~/OriginalVideo/" + fileName);
                string filepath = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectId + "/" + fileName);
                while (File.Exists(filepath))
                {
                    j = j + 1;
                    int dotPos = fileName.LastIndexOf(".");
                    string namewithoutext = fileName.Substring(0, dotPos);
                    string ext = fileName.Substring(dotPos + 1);
                    fileName = namewithoutext + j + "." + ext;
                    filepath = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectId + "/" + fileName);
                }
                try
                {
                    File.Move(streamProvider.FileData.FirstOrDefault().LocalFileName, filepath);
                    //File.SaveAs(filepath);
                }
                catch
                {
                    return false;
                }
                //string outPutFile;
                //outPutFile = HttpContext.Current.Server.MapPath("~/ProjectFiles/Project_" + projectId + "/" + fileName);
                //int i = this.fileuploadImageVideo.PostedFile.ContentLength;
                //int i = streamProvider.FileData.FirstOrDefault().PostedFile.ContentLength;
                //System.IO.FileInfo a = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(outPutFile));
                //while (a.Exists == false)
                //{

                //}
                //long b = a.Length;
                //while (i != b)
                //{

                //}


                string cmd = " -i \"" + inputPath + "\\" + fileName + "\" -c:v libx264 \"" + outputPath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".webm" + "\"";
                ConvertNow(cmd);
                string imgargs = " -i \"" + outputPath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".webm" + "\" -f image2 -ss 1 -vframes 1 -s 280x200 -an \"" + imgpath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".jpg" + "\"";
                ConvertNow(imgargs);

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void ConvertNow(string cmd)
        {
            try
            {
                string exepath;
                string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
                //Get the application path
                exepath = AppPath + "bin\\ffmpeg.exe";
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = exepath;
                //Path of exe that will be executed, only for "filebuffer" it will be "flvtool2.exe"
                proc.StartInfo.Arguments = cmd;
                //The command which will be executed
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.Start();

                while (proc.HasExited)
                {
                    proc.ExitTime.AddMinutes(2);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //protected void btn_Submit_Click(object sender, EventArgs e)
        //{
        //    ReturnVideo(this.fileuploadImageVideo.FileName.ToString());
        //}
        [HttpPost]
        [Route("BackItFunding/{id}")]
        public async Task<IHttpActionResult> BackItFunding(string id)
        {
            var backitFunding = await _userService.BackItFunding(id);
            return Ok(new { backitFunding });
        }

        [HttpPost]
        [Route("DeductFeaturedClick/{projectId}")]
        public async Task<IHttpActionResult> DeductFeaturedClick(string projectId)
        {
            var result = await _userService.deductFeaturedClick(projectId);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetClicksLog/{projectId}")]
        public async Task<IHttpActionResult> GetClicksLog(string projectId)
        {
            var result = await _userService.GetClicksLog(projectId);
            return Ok(result);
        }
        #endregion

        #endregion

        #region MyProfile

        [Authorize]
        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IHttpActionResult> GetMyProfile()
        {
            var profile = await _userService.GetMyProfile();
            if (profile != null)
            {
                var months = _userService.GetMonths();
                var industries = await _userService.GetIndustries();
                return Ok(new { profile, months, industries });
            }
            return BadRequest("Error getting records");
        }

        [Authorize]
        [HttpGet]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser()
        {
            var profile = await _userService.GetMyProfile();
            if (profile == null)
                return BadRequest("Error getting records");
            return Ok(profile);
        }

        [HttpPost]
        [Route("UploadProfileImage")]
        public async Task<IHttpActionResult> UploadProfileImage()
        {
            var userModel = new UserModel();

            if (!Request.Content.IsMimeMultipartContent())
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
            string uploadPath = HttpContext.Current.Server.MapPath("~/ProfileImages");

            MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
            var result = await Request.Content.ReadAsMultipartAsync(streamProvider);

            FileInfo fi = new FileInfo(streamProvider.FileData.FirstOrDefault().LocalFileName);
            if (File.Exists(streamProvider.FileData.FirstOrDefault().LocalFileName))
            {
                userModel.PictureUrl = "/ProfileImages/" + fi.Name;
                userModel = await _userService.UploadProfileImage(userModel);
                return Ok(userModel);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("UpdateUserProfile")]
        public async Task<AuthenticationServiceResponse> UpdateUserProfile(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return new AuthenticationServiceResponse { Data = userModel, Success = false, Message = "Invalid Model State" };
            }
            var newResult = await _userService.UpdateUserProfile(userModel);
            return newResult;
        }

        [HttpPost]
        [Route("UpdateUserProfileAndImage")]
        public async Task<AuthenticationServiceResponse> UpdateUserProfileAndImage()
        {
            try
            {
                var model = new UserModel();
                if (!Request.Content.IsMimeMultipartContent())
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }

                string uploadPath = HttpContext.Current.Server.MapPath("~/ProfileImages");

                MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var result = await Request.Content.ReadAsMultipartAsync(streamProvider);
                var data = GetFormData<UserModel>(result);
                model = (UserModel)data;
                FileInfo fi = new FileInfo(streamProvider.FileData.FirstOrDefault().LocalFileName);
                if (File.Exists(streamProvider.FileData.FirstOrDefault().LocalFileName))
                {
                    model.PictureUrl = "/ProfileImages/" + fi.Name;
                }

                var authResponse = await _userService.UpdateUserProfile(model);
                return authResponse;
            }
            catch (Exception ex)
            {
                return new AuthenticationServiceResponse { Success = false, Message = ex.Message, Data = ex };
            }
        }
        [HttpPost]
        [Route("ChangePassword/{OldPassword}/{NewPassword}")]
        public async Task<IHttpActionResult> ChangePassword(string OldPassword, string NewPassword)
        {
            var result = await _userService.ChangePassword(OldPassword, NewPassword);
            return Ok(result);
        }

        #endregion

        #region Payment
        [HttpPost]
        [Route("AddPaymentInfo")]
        public async Task<IHttpActionResult> AddPaymentInfo(PaymentModel paymentModel)
        {
            var result = await _userService.AddProjectFunding(paymentModel);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetMyPayments")]
        public async Task<IHttpActionResult> GetMyPayments(int PageNumber, int PageSize)
        {
            int PaymentCount;
            var result = await _userService.GetMyPayments(PageNumber, PageSize, out PaymentCount);
            return Ok(new { result, PaymentCount });
        }

        [HttpGet]
        //[Route("GetProjectFundings/{ProjectId}/{PageNumber}/{PageSize}")]
        [Route("GetProjectFundings")]
        public async Task<IHttpActionResult> GetProjectFundings(string ProjectId, int PageNumber, int PageSize)
        {
            int ProjectFundingCount;
            var result = await _userService.GetProjectFundings(ProjectId, PageNumber, PageSize, out ProjectFundingCount);
            return Ok(new { result, ProjectFundingCount });
        }
        [HttpPost]
        [Route("AddFeaturedClicksPayment")]
        public async Task<IHttpActionResult> AddFeaturedClicksPayment(FeaturedClicksPaymentModel paymentModel)
        {
            var transactionModel = PaymentWithCreditCard(paymentModel);
            int transactionId;
            if (transactionModel.Status != "failed")
            {
                var result = await _userService.AddFeaturedClicksPayment(paymentModel, transactionModel);
                return Ok(result);
            }
            else
            {
                var result = await _userService.AddFailedTransaction(transactionModel, out transactionId);
                return Ok(result);
            }
        }

        private TransactionModel PaymentWithCreditCard(FeaturedClicksPaymentModel paymentModel)
        {
            try
            {
                TransactionModel transactionModel = new TransactionModel();
                if (paymentModel.Amount.ToString().Split('.')[1].ToCharArray().Count() != 2)
                {
                    paymentModel.Amount = paymentModel.Amount + "0";
                }
                var firstName = paymentModel.Name.Split(' ')[0];
                var lastName = "";
                if (paymentModel.Name.Split(' ').Count() < 2)
                {
                    lastName = paymentModel.Name.Split(' ')[0];
                }
                else
                {
                    foreach (var name in paymentModel.Name.Split(' ').Skip(1))
                    {
                        lastName += " " + name;
                    }

                }

                Item item = new Item();
                item.name = "Buy " + paymentModel.Clicks + " Clicks";
                item.currency = "USD";
                item.price = paymentModel.Amount.ToString();
                item.quantity = "1";

                //Now make a List of Item and add the above item to it
                List<Item> itms = new List<Item>();
                itms.Add(item);
                ItemList itemList = new ItemList();
                itemList.items = itms;

                //Address for the payment
                PayPal.Api.Address billingAddress = new PayPal.Api.Address();
                billingAddress.city = paymentModel.City;
                billingAddress.country_code = "US";
                billingAddress.line1 = paymentModel.Address1;
                billingAddress.line2 = paymentModel.Address2;
                billingAddress.postal_code = paymentModel.PostalCode;
                billingAddress.state = paymentModel.Region;

                //Now Create an object of credit card and add above details to it
                CreditCard crdtCard = new CreditCard();
                crdtCard.billing_address = billingAddress;
                crdtCard.cvv2 = paymentModel.CardCVN.ToString();
                crdtCard.expire_month = paymentModel.ExpirationMonth;
                crdtCard.expire_year = paymentModel.ExpirationYear;
                crdtCard.first_name = firstName;
                crdtCard.last_name = lastName;
                crdtCard.number = paymentModel.CardNumber;
                crdtCard.type = paymentModel.CardType.ToLower();

                // Specify details of your payment amount.
                Details details = new Details();
                //details.shipping = "1";
                details.subtotal = paymentModel.Amount.ToString();
                //details.tax = "1";

                // Specify your total payment amount and assign the details object
                Amount amnt = new Amount();
                amnt.currency = "USD";
                amnt.total = paymentModel.Amount.ToString();
                amnt.details = details;

                // Now make a trasaction object and assign the Amount object
                Transaction tran = new Transaction();
                tran.amount = amnt;
                tran.description = "Payment for clicks on Project : ";
                tran.item_list = itemList;

                // Now, we have to make a list of trasaction and add the trasactions object
                List<Transaction> transactions = new List<Transaction>();
                transactions.Add(tran);

                // Now we need to specify the FundingInstrument of the Payer
                FundingInstrument fundInstrument = new FundingInstrument();
                fundInstrument.credit_card = crdtCard;

                // The Payment creation API requires a list of FundingIntrument
                List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
                fundingInstrumentList.Add(fundInstrument);

                // Now create Payer object and assign the fundinginstrument list to the object
                Payer payr = new Payer();
                payr.funding_instruments = fundingInstrumentList;
                payr.payment_method = "credit_card";

                // finally create the payment object and assign the payer object & transaction list to it
                Payment pymnt = new Payment();
                pymnt.intent = "sale";
                pymnt.payer = payr;
                pymnt.transactions = transactions;

                transactionModel.Amount = paymentModel.Amount.ToString();
                transactionModel.UserId = _userService.getLoggedInUserId();
                transactionModel.ProjectId = paymentModel.ProjectId.Decrypt();
                try
                {
                    APIContext apiContext = GetAPIContext();

                    Payment createdPayment = pymnt.Create(apiContext);

                    //if the createdPayment.State is "approved" it means the payment was successfull else not
                    if (createdPayment.state.ToLower() != "approved")
                    {
                        //PaymentExecution pymntExecution = new PaymentExecution();
                        //pymntExecution.payer_id = ("");
                        //Payment executedPayment = pymnt.Execute(apiContext, pymntExecution);
                        //if (executedPayment.state.ToLower() != "approved")
                        //else
                        //    return true;
                    }
                    else
                    {

                    }
                    transactionModel.Message = "Success";
                    transactionModel.PaymentId = createdPayment.id;
                    transactionModel.Status = createdPayment.state;
                }
                catch (PayPal.PayPalException ex)
                {
                    if (((PayPal.PaymentsException)(ex)).Details.details != null)
                    {
                        foreach (var errorDetail in ((PayPal.PaymentsException)(ex)).Details.details)
                        {
                            transactionModel.Message += errorDetail.issue;
                        }
                    }
                    else
                    {
                        transactionModel.Message = ((PayPal.PaymentsException)(ex)).Details.message;
                    }

                    transactionModel.PaymentId = "";
                    transactionModel.Status = "failed";
                    //Logger.Log("Error: " + ex.Message);
                }
                return transactionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        [Route("GetPayment/{projectId}")]
        public async Task<IHttpActionResult> GetPayment(string projectId)
        {
            int TransactionId;
            var result = _userService.getProjectFundings(projectId);
            foreach (var item in result)
            {
                var transaction = PaymentWithCreditCard(item);
                var resultTransaction = await _userService.AddFailedTransaction(transaction, out TransactionId);
                _userService.updateProjectFunding(item.ProjectFundingId, TransactionId, transaction.Status);
            }
            var paymentResult = _userService.UpdateProjectStatus(projectId);
            return Ok(paymentResult);
        }
        private TransactionModel PaymentWithCreditCard(PaymentModel paymentModel)
        {
            try
            {
                TransactionModel transactionModel = new TransactionModel();
                var paymnetAmount = (Convert.ToDecimal(paymentModel.FundingAmount) + Convert.ToDecimal(paymentModel.ProcessingFees)).ToString();
                if (paymnetAmount.ToString().Split('.')[1].ToCharArray().Count() != 2)
                {
                    paymnetAmount = paymnetAmount + "0";
                }
                var firstName = paymentModel.Name.Split(' ')[0];
                //var lastName = paymentModel.Name.Split(' ')[1] != null ? paymentModel.Name.Split(' ')[1] : "";
                var lastName = "";
                if (paymentModel.Name.Split(' ').Count() < 2)
                {
                    lastName = paymentModel.Name.Split(' ')[0];
                }
                else
                {
                    foreach (var name in paymentModel.Name.Split(' ').Skip(1))
                    {
                        lastName += " " + name;
                    }

                }
                Item item = new Item();
                item.name = "Funding Project";
                item.currency = "USD";
                item.price = paymnetAmount;
                item.quantity = "1";
                item.sku = "sku";

                //Now make a List of Item and add the above item to it
                List<Item> itms = new List<Item>();
                itms.Add(item);
                ItemList itemList = new ItemList();
                itemList.items = itms;

                //Address for the payment
                PayPal.Api.Address billingAddress = new PayPal.Api.Address();
                billingAddress.city = paymentModel.City;
                billingAddress.country_code = "US";
                billingAddress.line1 = paymentModel.Address1;
                billingAddress.line2 = paymentModel.Address2;
                billingAddress.postal_code = paymentModel.PostalCode;
                billingAddress.state = paymentModel.Region;

                //Now Create an object of credit card and add above details to it
                CreditCard crdtCard = new CreditCard();
                crdtCard.billing_address = billingAddress;
                crdtCard.cvv2 = paymentModel.CardCVN.ToString();
                crdtCard.expire_month = paymentModel.ExpirationMonth;
                crdtCard.expire_year = paymentModel.ExpirationYear;
                crdtCard.first_name = firstName;
                crdtCard.last_name = lastName;
                crdtCard.number = paymentModel.CardNumber;
                crdtCard.type = GetCardTypeFromNumber(crdtCard.number).ToString().ToLower();

                // Specify details of your payment amount.
                Details details = new Details();
                //details.shipping = "1";
                details.subtotal = paymnetAmount;
                //details.tax = "1";

                // Specify your total payment amount and assign the details object
                Amount amnt = new Amount();
                amnt.currency = "USD";
                amnt.total = paymnetAmount;
                amnt.details = details;

                // Now make a trasaction object and assign the Amount object
                Transaction tran = new Transaction();
                tran.amount = amnt;
                tran.description = "Payment for clicks on Project : ";
                tran.item_list = itemList;

                // Now, we have to make a list of trasaction and add the trasactions object
                List<Transaction> transactions = new List<Transaction>();
                transactions.Add(tran);

                // Now we need to specify the FundingInstrument of the Payer
                FundingInstrument fundInstrument = new FundingInstrument();
                fundInstrument.credit_card = crdtCard;

                // The Payment creation API requires a list of FundingIntrument
                List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
                fundingInstrumentList.Add(fundInstrument);

                // Now create Payer object and assign the fundinginstrument list to the object
                Payer payr = new Payer();
                payr.funding_instruments = fundingInstrumentList;
                payr.payment_method = "credit_card";

                // finally create the payment object and assign the payer object & transaction list to it
                Payment pymnt = new Payment();
                pymnt.intent = "sale";
                pymnt.payer = payr;
                pymnt.transactions = transactions;

                transactionModel.Amount = paymnetAmount;
                transactionModel.Fees = paymentModel.ProcessingFees;
                transactionModel.FinalAmount = (Convert.ToDecimal(transactionModel.Amount) - Convert.ToDecimal(paymentModel.ProcessingFees)).ToString();
                transactionModel.UserId = paymentModel.FundingUserId;
                transactionModel.ProjectId = Convert.ToInt32(paymentModel.ProjectId);
                try
                {
                    APIContext apiContext = GetAPIContext();

                    Payment createdPayment = pymnt.Create(apiContext);

                    //if the createdPayment.State is "approved" it means the payment was successful else not
                    if (createdPayment.state.ToLower() != "approved")
                    {
                        //PaymentExecution pymntExecution = new PaymentExecution();
                        //pymntExecution.payer_id = ("");
                        //Payment executedPayment = pymnt.Execute(apiContext, pymntExecution);
                        //if (executedPayment.state.ToLower() != "approved")


                        //else
                        //    return true;
                    }
                    else
                    {

                    }
                    transactionModel.Message = "Success";
                    transactionModel.PaymentId = createdPayment.id;
                    transactionModel.Status = createdPayment.state;
                }
                catch (PayPal.PayPalException ex)
                {
                    if (((PayPal.PaymentsException)(ex)).Details.details != null)
                    {
                        foreach (var errorDetail in ((PayPal.PaymentsException)(ex)).Details.details)
                        {
                            transactionModel.Message += errorDetail.issue;
                        }
                    }
                    else
                    {
                        transactionModel.Message = ((PayPal.PaymentsException)(ex)).Details.message;
                    }

                    transactionModel.PaymentId = "";
                    transactionModel.Status = "failed";
                    //Logger.Log("Error: " + ex.Message);
                }
                return transactionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IHttpActionResult> TransferPayment(string PaypalAccount, decimal Amount, string projectId)
        {

            ReceiverList receiverList = new ReceiverList();
            receiverList.receiver = new List<Receiver>();
            Receiver secondaryReceiver = new Receiver(Amount);
            secondaryReceiver.email = PaypalAccount;
            receiverList.receiver.Add(secondaryReceiver);


            SenderIdentifier sender = new SenderIdentifier();
            sender.email = ReadConfiguration.SuperAdminPaypalAccount;

            PayPal.AdaptivePayments.Model.RequestEnvelope requestEnvelope = new PayPal.AdaptivePayments.Model.RequestEnvelope("en_US");
            string actionType = "PAY";
            string returnUrl = "https://devtools-paypal.com/guide/ap_chained_payment/dotnet?success=true";
            string cancelUrl = "https://devtools-paypal.com/guide/ap_chained_payment/dotnet?cancel=true";
            string currencyCode = "USD";
            PayRequest payRequest = new PayRequest(requestEnvelope, actionType, cancelUrl, currencyCode, receiverList, returnUrl);
            payRequest.ipnNotificationUrl = "http://replaceIpnUrl.com";
            payRequest.senderEmail = sender.email;


            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(GetConfig());
            PayResponse payResponse = adaptivePaymentsService.Pay(payRequest);
            if (payResponse.payKey != null && payResponse.paymentExecStatus.ToLower() == CommonEnums.PaymentTransferStatus.completed.ToString())
            {
                _userService.UpdateProjectTransferStatus(projectId);
            }

            return Ok(payResponse);
        }
        public static APIContext GetAPIContext()
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();

            // Use this variant if you want to pass in a request id  
            // that is meaningful in your application, ideally 
            // a order id.
            // String requestId = Long.toString(System.nanoTime();
            // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

            return apiContext;
        }
        private static string GetAccessToken()
        {
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window                
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
        }
        public enum CreditCardTypeType
        {
            Visa,
            MasterCard,
            Discover,
            Amex,
            Switch,
            Solo
        }

        public static CreditCardTypeType? GetCardTypeFromNumber(string cardNum)
        {
            string cardRegex = "^(?:(?<Visa>4\\d{3})|" +
            "(?<MasterCard>5[1-5]\\d{2})|(?<Discover>6011)|(?<DinersClub>" +
            "(?:3[68]\\d{2})|(?:30[0-5]\\d))|(?<Amex>3[47]\\d{2}))([ -]?)" +
            "(?(DinersClub)(?:\\d{6}\\1\\d{4})|(?(Amex)(?:\\d{6}\\1\\d{5})" +
            "|(?:\\d{4}\\1\\d{4}\\1\\d{4})))$";
            //Create new instance of Regex comparer with our
            //credit card regex patter
            Regex cardTest = new Regex(cardRegex);

            //Compare the supplied card number with the regex
            //pattern and get reference regex named groups
            GroupCollection gc = cardTest.Match(cardNum).Groups;

            //Compare each card type to the named groups to 
            //determine which card type the number matches
            if (gc[CreditCardTypeType.Amex.ToString()].Success)
            {
                return CreditCardTypeType.Amex;
            }
            else if (gc[CreditCardTypeType.MasterCard.ToString()].Success)
            {
                return CreditCardTypeType.MasterCard;
            }
            else if (gc[CreditCardTypeType.Visa.ToString()].Success)
            {
                return CreditCardTypeType.Visa;
            }
            else if (gc[CreditCardTypeType.Discover.ToString()].Success)
            {
                return CreditCardTypeType.Discover;
            }
            else
            {
                //Card type is not supported by our system, return null
                //(You can modify this code to support more (or less)
                // card types as it pertains to your application)
                return null;
            }
        }

        #endregion

        //#region Messages

        //#region GetMethods
        //[HttpGet]
        //[Route("GetMessages")]
        //public async Task<IHttpActionResult> GetMessages()
        //{
        //    var result = await _userService.GetMessages();
        //    return Ok(result);
        //}
        //#endregion

        //#region AddUpdateMethods
        //[HttpPost]
        //[Route("AddComment")]
        //public async Task<IHttpActionResult> AddComment(MessageModel message)
        //{
        //    var result = await _userService.AddComment(message);
        //    return Ok(result);
        //}
        //#endregion

        //#region DeleteMethods
        //[HttpPost]
        //[Route("DeleteComment")]
        //public async Task<IHttpActionResult> DeleteComment(MessageModel messageModel)
        //{
        //    var result = await _userService.DeleteComment(messageModel);
        //    return Ok(result);
        //}
        //#endregion

        //#endregion

    }
}

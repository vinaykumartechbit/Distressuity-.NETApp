using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.DAL.Interfaces;
using DISTRESSUITY.Service.Interfaces;
using DISTRESSUITY.Service.Model;
using DISTRESSUITY.Service.Services;
using ExpressMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DISTRESSUITY.Common.Enums;
using System.IO;
using System.Web;
using NReco.VideoConverter;
using DISTRESSUITY.Common.Utility;
using DISTRESSUITY.Common.Constants;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace DISTRESSUITY.Service.Service
{
    public class UserService : AbstractService, IUserService
    {
        int initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
        #region Constructors
        public UserService(IUnitofWork unitwork) :
            base(unitwork)
        {

        }
        public UserService()
        {

        }
        #endregion

        #region CommonMethods
        public List<object> GetMonths()
        {
            var enumVals = new List<object>();
            foreach (var item in Enum.GetValues(typeof(DISTRESSUITY.Common.Enums.CommonEnums.Months)))
            {
                enumVals.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return enumVals;
        }
        public Task<List<IndustryModel>> GetIndustries()
        {
            try
            {
                var industriesModel = new List<IndustryModel>();
                var industries = _unitOfWork.industryRepository.Get().OrderBy(x => x.Name).ToList();
                Mapper.Map(industries, industriesModel);
                return Task.FromResult(industriesModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public void DeleteFile(string path)
        {
            if (path != null && !path.Contains("https://"))
            {
                path = HttpContext.Current.Server.MapPath(@path);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
        public Task<string> ConvertVideoService(string inputFile, string inputFormat, string outputFile)
        {
            try
            {
                var ffMpeg = new FFMpegConverter();
                ConvertSettings settings = new ConvertSettings();
                settings.CustomOutputArgs = "-b:v 3000k -bufsize 3000k";
                //settings.CustomOutputArgs = "-c:v libx264 -c:a copy";
                settings.SetVideoFrameSize(1280, 720);
                //settings.VideoFrameRate = 100;

                if (inputFormat.ToLower() == "wmv")
                {
                    ffMpeg.ConvertMedia(inputFile, Format.wmv, outputFile, Format.webm, settings);
                }
                else if (inputFormat.ToLower() == "mpeg")
                {
                    ffMpeg.ConvertMedia(inputFile, Format.mpeg, outputFile, Format.webm, settings);
                }
                else if (inputFormat.ToLower() == "mov")
                {
                    ffMpeg.ConvertMedia(inputFile, Format.mov, outputFile, Format.webm, settings);
                }
                else if (inputFormat.ToLower() == "avi")
                {
                    ffMpeg.ConvertMedia(inputFile, Format.avi, outputFile, Format.webm, settings);
                }
                else if (inputFormat.ToLower() == "flv")
                {
                    ffMpeg.ConvertMedia(inputFile, Format.flv, outputFile, Format.webm, settings);
                }
                else if (inputFormat.ToLower() == "3gp" || inputFormat.ToLower() == "3g2")
                {
                    ffMpeg.ConvertMedia(inputFile, outputFile, Format.mp4);
                }
                //ffMpeg.GetVideoThumbnail(inputFile, outputFile);
                return Task.FromResult(outputFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<string> ConvertVideoServiceByUrl(string Inputfile, string inputFormat, string outputFile)
        {
            string response = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                string url = "";
                url = url + "/api/VideoConverter/VideoConvert";
               string baseUrl = "http://localhost:33714/";
                //string baseUrl = "http://mediaconvertor.techbitsolution.com/";
                if (baseUrl[baseUrl.Length - 1] == '/')
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                }
                if (url[0] == '/')
                {
                    url = url.Substring(1);
                }
                url = baseUrl + "/" + url;
                url = url.Replace(" ", "%20");

                byte[] bytes = File.ReadAllBytes(Inputfile);

                // Create HTTP transport objects
                HttpRequestMessage httpRequest = new HttpRequestMessage();
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = new Uri(url);

                HttpContent bytesContent = new ByteArrayContent(bytes);
                HttpContent stringContent = new StringContent(inputFormat);
                HttpContent stringContent1 = new StringContent(outputFile);
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(stringContent, "file1", "file1");
                    formData.Add(stringContent1, "file2", "file2");
                    formData.Add(bytesContent, "file3", "file3");
                    client.Timeout = TimeSpan.FromMinutes(59);
                    HttpResponseMessage httpResponse = client.PostAsync(url, formData).Result;
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    byte[] outputbyteStream = httpResponse.Content.ReadAsByteArrayAsync().Result;
                    if (outputbyteStream.Length > 0)
                    {
                        File.WriteAllBytes(outputFile, outputbyteStream);
                        response = outputFile;
                    }
                    return Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region MyProfile

        #region GetMethods
        public Task<UserModel> GetMyProfile()
        {
            var user = new UserModel();
            int userId = GetLoginUserId();
            var exstUsr = _unitOfWork.applicationUserRepository.GetById(userId);
            if (exstUsr == null)
                return null;
            exstUsr.UserPositions = exstUsr.UserPositions.Where(up => !up.IsDeleted).ToList();
            Mapper.Map(exstUsr, user);
            Mapper.Map(exstUsr.UserPositions, user.UserPositions);
            //user.Months = GetMonths();
            return Task.FromResult(user);
        }
        #endregion

        #region UpdateMethods
        public Task<AuthenticationServiceResponse> UpdateUserProfile(UserModel model)
        {
            try
            {

                var UserPositions = new List<UserPosition>();
                //var appUser = new ApplicationUser();
                var user = _unitOfWork.applicationUserRepository.GetById(model.Id.Decrypt());
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.IndustryId = model.IndustryId.Decrypt();
                user.Summary = model.Summary;
                user.Specialists = model.Specialists;
                if (user.PictureUrl != null && !user.PictureUrl.Contains("https://"))
                {
                    var path = HttpContext.Current.Server.MapPath(@user.PictureUrl.Substring(2));
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                user.PictureUrl = model.PictureUrl;

                _unitOfWork.applicationUserRepository.Update(user);


                #region Update Positions
                var existUserPositionsModel = model.UserPositions.Where(u => !u.PosititonId.Equals(0)).ToList();
                Mapper.Map(existUserPositionsModel, UserPositions);
                _unitOfWork.userPositionsRepository.UpdateAll(UserPositions);
                #endregion

                #region New Positions
                var newUserPositionsModel = model.UserPositions.Where(u => u.PosititonId.Equals(0)).ToList();
                UserPositions = new List<UserPosition>();
                Mapper.Map(newUserPositionsModel, UserPositions);
                UserPositions.ForEach(x => x.UserId = user.Id);
                _unitOfWork.userPositionsRepository.InsertAll(UserPositions);
                #endregion
                _unitOfWork.Save();


                user = _unitOfWork.applicationUserRepository.GetById(model.Id.Decrypt());
                user.UserPositions = user.UserPositions.Where(up => !up.IsDeleted).ToList();
                Mapper.Map(user, model);
                return Task.FromResult(new AuthenticationServiceResponse { Data = model, Success = true, Message = "Profile updated Successfully" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<UserModel> UploadProfileImage(UserModel userModel)
        {
            try
            {
                var userId = GetLoginUserId();
                var exstUser = _unitOfWork.applicationUserRepository.GetById(userId);
                if (exstUser.PictureUrl != null && !exstUser.PictureUrl.Contains("https://"))
                {
                    var path = HttpContext.Current.Server.MapPath("~" + exstUser.PictureUrl.Substring(2));
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                exstUser.PictureUrl = userModel.PictureUrl;
                _unitOfWork.applicationUserRepository.Update(exstUser);
                _unitOfWork.Save();
                exstUser = _unitOfWork.applicationUserRepository.GetById(userId);
                Mapper.Map(exstUser, userModel);
                return Task.FromResult(userModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #endregion

        #region Project

        #region GetMethods
        public List<object> GetProjectTabEnums()
        {
            var enumVals = new List<object>();
            foreach (var item in Enum.GetValues(typeof(CommonEnums.ProjectTabEnums)))
            {
                enumVals.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return enumVals;
        }

        public List<string> CurrentUserPaypalAccounts()
        {
            var userId = GetLoginUserId();
            List<string> PaypalAccountList = _unitOfWork.projectRepository.Get(data => data.OwnerId == userId).OrderByDescending(data => data.PaypalAccount).Select(x => x.PaypalAccount).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            return PaypalAccountList;
        }
        public Task<List<ProjectModel>> FilterProjectsByIndustryId(string id, out int ProjectCount)
        {
            ProjectCount = 0;
            var projects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            int Id = id.Decrypt();
            if (id.Equals(0))
            {
                return null;
            }
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            projects = _unitOfWork.projectRepository.Get(data => data.IndustryId == Id && data.StatusId == statusId && data.FundingDuration > DateTime.Now
                && !featuredProjectIds.Any(s => data.ProjectId == s)).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            ProjectCount = projects.Count();
            Mapper.Map(projects, projectsModel);
            //projects = _unitOfWork.projectRepository.Get( data.StatusId.Equals(statusId) && data.FundingDuration > DateTime.Now).Take(initialPageSize).ToList();
            //Mapper.Map(projects, projectsModel);
            return Task.FromResult(projectsModel);

        }
        public Task<List<ProjectModel>> FilterFeaturedProjectsByIndustryId(string id, out int featuredProjectsCount)
        {
            featuredProjectsCount = 0;
            var projects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            int Id = id.Decrypt();
            if (id.Equals(0))
            {
                return null;
            }
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            projects = _unitOfWork.projectRepository.Get(data => data.IndustryId == Id && data.StatusId == statusId && data.FundingDuration > DateTime.Now
                && featuredProjectIds.Any(s => data.ProjectId == s)).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            featuredProjectsCount = projects.Count();
            //projects = _unitOfWork.projectRepository.Get(data => data.IndustryId == id && data.StatusId.Equals(statusId) && data.FundingDuration > DateTime.Now).Take(initialPageSize).ToList();
            Mapper.Map(projects, projectsModel);
            return Task.FromResult(projectsModel);

        }
        public Task<AuthenticationServiceResponse> ChangePassword(string OldPassword, string NewPassword)
        {
            var result = (new AuthenticationService()).ChangePassword(OldPassword, NewPassword);
            return result;
        }
        public Task<List<ProjectModel>> GetMyProjects()
        {
            var projectsModel = new List<ProjectModel>();
            var userId = GetLoginUserId();
            var projects = _unitOfWork.projectRepository.Get(data => data.OwnerId == userId).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            Mapper.Map(projects, projectsModel);
            for (int i = 0; i < projects.Count; i++)
            {
                for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                {
                    if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                        Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                        projectsModel[i].ProjectFundings.Add(projectFundingModel);
                    }
                }
                projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                Mapper.Map(projects[i].Status, projectsModel[i].Status);
            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> LoadMoreProjects(int pageNumber, string name)
        {
            var projectsModel = new List<ProjectModel>();
            var projects = new List<Project>();
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var pageSize = Convert.ToInt32(ReadConfiguration.PageSize);
            if (name != null)
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                    && (data.Title.Contains(name) || data.Location.Contains(name)), data => data.CreatedDate, pageNumber, pageSize).ToList();
            else
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now, data => data.CreatedDate, pageNumber, pageSize).ToList();
            Mapper.Map(projects, projectsModel);
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> LoadMoreFeaturedProjects(int pageNumber, string name)
        {
            var projectsModel = new List<ProjectModel>();
            var projects = new List<Project>();
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var pageSize = Convert.ToInt32(ReadConfiguration.PageSize);
            if (name != null)
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                    && (data.Title.Contains(name) || data.Location.Contains(name)), data => data.CreatedDate, pageNumber, pageSize).ToList();
            else
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now, data => data.CreatedDate, pageNumber, pageSize).ToList();
            Mapper.Map(projects, projectsModel);
            return Task.FromResult(projectsModel);
        }

        public Task<List<ProjectModel>> LoadMoreMyProjects(int pageNumber, string name)
        {
            var projectsModel = new List<ProjectModel>();
            var projects = new List<Project>();
            var userId = GetLoginUserId();
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var pageSize = Convert.ToInt32(ReadConfiguration.PageSize);
            if (name != null)
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.OwnerId == userId && (data.Title.Contains(name) || data.Location.Contains(name))
                    , data => data.CreatedDate, pageNumber, pageSize).ToList();
            else
                projects = _unitOfWork.projectRepository.GetPagedRecords(data => data.OwnerId == userId, data => data.CreatedDate, pageNumber, pageSize).ToList();
            Mapper.Map(projects, projectsModel);
            for (int i = 0; i < projects.Count; i++)
            {
                Mapper.Map(projects[i].Status, projectsModel[i].Status);
            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> GetProjects(out int ProjectCount)
        {
            try
            {
                var projectsModel = new List<ProjectModel>();
                var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
                var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
                var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
                var projects = _unitOfWork.projectRepository.Get(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                    && !featuredProjectIds.Any(s => data.ProjectId == s)).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
                ProjectCount = projects.Count();
                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                    {
                        if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            var projectFundingModel = new ProjectFundingModel();
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                            projectsModel[i].ProjectFundings.Add(projectFundingModel);
                        }
                    }
                    projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                    // Mapper.Map(projects[i].User, projectsModel[i].User);
                }
                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Task<List<ProjectModel>> GetFeaturedProjects(out int featuredProjectsCount)
        {
            try
            {
                var projectsModel = new List<ProjectModel>();
                var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
                var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
                var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
                var projects = _unitOfWork.projectRepository.Get(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                    && featuredProjectIds.Any(s => data.ProjectId == s)).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
                featuredProjectsCount = projects.Count();
                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                    {
                        if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            var projectFundingModel = new ProjectFundingModel();
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                            projectsModel[i].ProjectFundings.Add(projectFundingModel);
                        }
                    }
                    projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                    Mapper.Map(projects[i].User, projectsModel[i].User);
                }
                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Task<List<ProjectModel>> GetAllProjects(int PageNumber, int PageSize, out int AllProjectCount)
        {
            try
            {
                var projectsModel = new List<ProjectModel>();
                var Active = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
                var WaitingForApproaval = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
                var Declined = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Declined);
                var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
                AllProjectCount = _unitOfWork.projectRepository.GetCount(data => (data.StatusId == Active || data.StatusId == WaitingForApproaval || data.StatusId == Declined)
                  && data.FundingDuration > DateTime.Now);
                var projects = _unitOfWork.projectRepository.Get(data => (data.StatusId == Active || data.StatusId == WaitingForApproaval || data.StatusId == Declined)
                    && data.FundingDuration > DateTime.Now).OrderByDescending(data => data.CreatedDate).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList(); //.Take(initialPageSize)
                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                    {
                        if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            var projectFundingModel = new ProjectFundingModel();
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                            projectsModel[i].ProjectFundings.Add(projectFundingModel);
                        }
                    }
                    projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                    Mapper.Map(projects[i].User, projectsModel[i].User);
                    Mapper.Map(projects[i].Status, projectsModel[i].Status);

                }
                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public Task<List<ProjectModel>> GetFundedProject(int PageNumber, int PageSize, out int ProjectCount)
        {
            try
            {
                var projectsModel = new List<ProjectModel>();
                var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
                ProjectCount = _unitOfWork.projectRepository.GetCount(data => data.IsFunded == true);
                var projects = _unitOfWork.projectRepository.Get(data => data.IsFunded == true).OrderByDescending(data => data.CreatedDate).Skip((PageNumber - 1) * PageSize).Take(PageSize).Select(x => new Project
                {
                    TotalFundingAmount = (x.Transaction.Where(y => y.Status == CommonEnums.TransactionStatus.approved.ToString()).Select(y => Convert.ToDecimal(y.FinalAmount)).Sum()),
                    ProjectId = x.ProjectId,
                    ProjectTitle = x.ProjectTitle,
                    ImagePath = x.ImagePath,
                    IsTransfered = x.IsTransfered,
                    User = x.User,
                    PaypalAccount = x.PaypalAccount,
                    PhoneNumber = x.PhoneNumber,
                    ImageName = x.ImageName
                }
                    ).ToList();

                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    Mapper.Map(projects[i].User, projectsModel[i].User);
                }

                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Task<List<ProjectModel>> GetFilteredFundedProjectList(DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                DateTime startDate = StartDate != null ? Convert.ToDateTime(StartDate) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = EndDate != null ? Convert.ToDateTime(EndDate) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                var projectsModel = new List<ProjectModel>();
                var projects = _unitOfWork.projectRepository.Get(data => data.IsFunded == true && (data.FundingDuration >= startDate && data.FundingDuration <= endDate)).OrderByDescending(data => data.CreatedDate).Select(x => new Project
                {
                    TotalFundingAmount = (x.Transaction.Where(y => y.Status == CommonEnums.TransactionStatus.approved.ToString()).Select(y => Convert.ToDecimal(y.FinalAmount)).Sum()),
                    ProjectId = x.ProjectId,
                    ProjectTitle = x.ProjectTitle,
                    ImagePath = x.ImagePath,
                    IsTransfered = x.IsTransfered,
                    User = x.User,
                    PaypalAccount = x.PaypalAccount,
                    PhoneNumber = x.PhoneNumber,
                    ImageName = x.ImageName,
                    FundingDuration = x.FundingDuration
                }
                    ).ToList();

                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    Mapper.Map(projects[i].User, projectsModel[i].User);
                }

                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Task<List<ProjectModel>> SearchProjects(string name, string id, out int projectsCount)
        {
            var Name = name == null ? "" : name;
            var Id = id == null ? 0 : id.Decrypt();
            var searchedProjects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            if (Id == 0)
            {
                projectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                   && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => keywords.ProjectId == s));

                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                   && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => keywords.ProjectId == s))
                   .OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            }
            else
            {
                projectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                   && keywords.IndustryId == Id && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => keywords.ProjectId == s));

                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.IndustryId == Id && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => keywords.ProjectId == s))
                    .OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            }

            Mapper.Map(searchedProjects, projectsModel);
            for (int i = 0; i < searchedProjects.Count; i++)
            {
                for (int j = 0; j < searchedProjects[i].ProjectFundings.Count; j++)
                {
                    if (searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j], projectFundingModel);
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                        projectsModel[i].ProjectFundings.Add(projectFundingModel);
                    }
                }
                projectsModel[i].FundingDuration = (searchedProjects[i].FundingDuration - DateTime.Now).Days > 0 ? (searchedProjects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                Mapper.Map(searchedProjects[i].User, projectsModel[i].User);
                Mapper.Map(searchedProjects[i].Status, projectsModel[i].Status);

            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> SearchFeaturedProjects(string name, string id, out int featuredProjectsCount)
        {
            var searchedProjects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            var Name = name == null ? "" : name;
            var Id = id == null ? 0 : id.Decrypt();
            if (Id == 0)
            {
                featuredProjectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                   && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => keywords.ProjectId == s));


                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => keywords.ProjectId == s))
                    .OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();


            }
            else
            {
                featuredProjectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                  && keywords.IndustryId == Id && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => keywords.ProjectId == s));


                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.IndustryId == Id && keywords.StatusId == statusId && keywords.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => keywords.ProjectId == s))
                    .OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList();
            }

            Mapper.Map(searchedProjects, projectsModel);
            for (int i = 0; i < searchedProjects.Count; i++)
            {
                for (int j = 0; j < searchedProjects[i].ProjectFundings.Count; j++)
                {
                    if (searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j], projectFundingModel);
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                        projectsModel[i].ProjectFundings.Add(projectFundingModel);
                    }
                }
                projectsModel[i].FundingDuration = (searchedProjects[i].FundingDuration - DateTime.Now).Days > 0 ? (searchedProjects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                Mapper.Map(searchedProjects[i].User, projectsModel[i].User);
                Mapper.Map(searchedProjects[i].Status, projectsModel[i].Status);

            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> SearchMyProjects(string name, string id, out int projectsCount)
        {
            var userId = GetLoginUserId();
            var searchedProjects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
            var Id = id == null ? 0 : id.Decrypt();
            var Name = name == null ? "" : name;
            //var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            if (Id == 0)
            {
                projectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.OwnerId == userId);

                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.OwnerId == userId).OrderByDescending(data => data.CreatedDate)
                    .Take(initialPageSize).ToList(); //data.StatusId == statusId &&
            }
            else
            {
                projectsCount = _unitOfWork.projectRepository.GetCount(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.OwnerId == userId && keywords.IndustryId == Id);


                searchedProjects = _unitOfWork.projectRepository.Get(keywords => (keywords.Title.Contains(Name) || keywords.ProjectTitle.Contains(Name) || keywords.Location.Contains(Name))
                    && keywords.OwnerId == userId && keywords.IndustryId == Id).OrderByDescending(data => data.CreatedDate).Take(initialPageSize).ToList(); //keywords.StatusId == statusId && 
            }

            Mapper.Map(searchedProjects, projectsModel);
            for (int i = 0; i < searchedProjects.Count; i++)
            {
                for (int j = 0; j < searchedProjects[i].ProjectFundings.Count; j++)
                {
                    if (searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || searchedProjects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j], projectFundingModel);
                        Mapper.Map(searchedProjects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                        projectsModel[i].ProjectFundings.Add(projectFundingModel);
                    }
                }
                projectsModel[i].FundingDuration = (searchedProjects[i].FundingDuration - DateTime.Now).Days > 0 ? (searchedProjects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                Mapper.Map(searchedProjects[i].User, projectsModel[i].User);
                Mapper.Map(searchedProjects[i].Status, projectsModel[i].Status);

            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<ProjectModel>> SearchAdminProjects(string name, string statusId)
        {
            //var userId = GetLoginUserId();
            var projects = new List<Project>();
            var projectsModel = new List<ProjectModel>();
            var StatusId = statusId == null ? 0 : statusId.Decrypt();
            var Name = name == null ? "" : name;
            //var Active = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            //var WaitingForApproaval = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
            var Inactive = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Inactive);
            var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
            if (StatusId == 0)
            {
                projects = _unitOfWork.projectRepository.Get(data => (data.ProjectTitle.Contains(Name)) && (data.StatusId != Inactive)
                    && data.FundingDuration > DateTime.Now).OrderByDescending(data => data.CreatedDate).ToList();
            }
            else
            {
                projects = _unitOfWork.projectRepository.Get(data => (data.StatusId == StatusId)
                    && data.FundingDuration > DateTime.Now && (data.ProjectTitle.Contains(Name))).OrderByDescending(data => data.CreatedDate).ToList(); //.Take(initialPageSize) //keywords.StatusId == statusId && 
            }
            Mapper.Map(projects, projectsModel);
            for (int i = 0; i < projects.Count; i++)
            {
                for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                {
                    if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                        Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                        projectsModel[i].ProjectFundings.Add(projectFundingModel);
                    }
                }
                projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                Mapper.Map(projects[i].User, projectsModel[i].User);
                Mapper.Map(projects[i].Status, projectsModel[i].Status);
            }
            return Task.FromResult(projectsModel);
        }
        public Task<List<StatusModel>> getStatusList()
        {
            var statusModelList = new List<StatusModel>();
            var Active = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var WaitingForApproaval = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
            var Declined = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Declined);
            var statusList = _unitOfWork.statusRepository.Get(x => x.StatusId == Active || x.StatusId == WaitingForApproaval || x.StatusId == Declined).ToList();
            Mapper.Map(statusList, statusModelList);
            return Task.FromResult(statusModelList);
        }
        public Task<List<ProjectModel>> GetAllProjetcsByStatus(string statusId, string searchKeyword)
        {
            try
            {
                var projectsModel = new List<ProjectModel>();
                var StatusId = statusId == null ? 0 : statusId.Decrypt();
                //var initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
                var projects = new List<Project>();
                if (searchKeyword == null)
                {
                    projects = _unitOfWork.projectRepository.Get(data => (data.StatusId == StatusId)
                        && data.FundingDuration > DateTime.Now).OrderByDescending(data => data.CreatedDate).ToList(); //.Take(initialPageSize)
                }
                else if (statusId == null)
                {
                    var Active = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
                    var WaitingForApproaval = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
                    var Declined = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Declined);
                    projects = _unitOfWork.projectRepository.Get(data => (data.StatusId == Active || data.StatusId == WaitingForApproaval || data.StatusId == Declined)
                        && data.FundingDuration > DateTime.Now && (data.Title.Contains(searchKeyword) || data.OwnerName.Contains(searchKeyword) || data.Industry.Name.Contains(searchKeyword))
                        ).OrderByDescending(data => data.CreatedDate).ToList(); //.Take(initialPageSize)
                }
                else
                {
                    projects = _unitOfWork.projectRepository.Get(data => (data.StatusId == StatusId) && data.FundingDuration > DateTime.Now
                        && (data.Title.Contains(searchKeyword) || data.OwnerName.Contains(searchKeyword) || data.Industry.Name.Contains(searchKeyword)))
                        .OrderByDescending(data => data.CreatedDate).ToList(); //.Take(initialPageSize)
                }
                Mapper.Map(projects, projectsModel);
                for (int i = 0; i < projects.Count; i++)
                {
                    for (int j = 0; j < projects[i].ProjectFundings.Count; j++)
                    {
                        if (projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || projects[i].ProjectFundings.ToList()[j].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            var projectFundingModel = new ProjectFundingModel();
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j], projectFundingModel);
                            Mapper.Map(projects[i].ProjectFundings.ToList()[j].User, projectFundingModel.UserModel);
                            projectsModel[i].ProjectFundings.Add(projectFundingModel);
                        }
                    }
                    projectsModel[i].FundingDuration = (projects[i].FundingDuration - DateTime.Now).Days > 0 ? (projects[i].FundingDuration - DateTime.Now).Days.ToString() : "0";
                    Mapper.Map(projects[i].User, projectsModel[i].User);
                    Mapper.Map(projects[i].Status, projectsModel[i].Status);
                }
                return Task.FromResult(projectsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<ProjectModel> GetProjectById(string projectId, int pageType)
        {
            try
            {
                var userId = GetLoginUserId();
                var projectModel = new ProjectModel();
                int id = projectId.Decrypt();
                Project project = _unitOfWork.projectRepository.GetById(id);
                Mapper.Map(project, projectModel);
                projectModel.FundingDuration = project.FundingDuration.toDateString();
                Mapper.Map(project.FinancialBreakdowns, projectModel.FinancialBreakdowns);
                Mapper.Map(project.ProjectDocuments, projectModel.ProjectDocuments);
                Mapper.Map(project.User, projectModel.User);
                Mapper.Map(project.Status, projectModel.Status);
                //Mapper.Map(project.Conversations, projectModel.Conversations);
                Mapper.Map(project.PublicMessages, projectModel.PublicMessages);
                //var PaypalAccountList = _unitOfWork.projectRepository.Get(x => x.OwnerId == userId).Distinct();
                projectModel.FeaturedClickPrice = Convert.ToDecimal(ReadConfiguration.FeaturedClickPrice);
                //for (int i = 0; i < projectModel.Conversations.Count; i++)
                //{
                //    Mapper.Map(project.Conversations.ToList()[i].ConversationReply, projectModel.Conversations[i].ConversationReplyModel);
                //    Mapper.Map(project.Conversations.ToList()[i].ToUser, projectModel.Conversations[i].ToUserModel);
                //    Mapper.Map(project.Conversations.ToList()[i].CreatedByUser, projectModel.Conversations[i].CreatedByUserModel);
                //    for (int j = 0; j < projectModel.Conversations[i].ConversationReplyModel.Count; j++)
                //    {
                //        Mapper.Map(project.Conversations.ToList()[i].ConversationReply.ToList()[j].User, projectModel.Conversations[i].ConversationReplyModel[j].UserModel);
                //    }
                //    //    Mapper.Map(project.Messages.ToList()[i].User, projectModel.Messages[i].UserModel);
                //}

                for (int i = 0; i < project.PublicMessages.ToList().Count; i++)
                {
                    Mapper.Map(project.PublicMessages.ToList()[i].User, projectModel.PublicMessages[i].UserModel);
                }
                for (int i = 0; i < project.ProjectFundings.Count; i++)
                {
                    if (pageType == (int)CommonEnums.GetProjectByType.EditProject)
                    {
                        var projectFundingModel = new ProjectFundingModel();
                        Mapper.Map(project.ProjectFundings.ToList()[i], projectFundingModel);
                        Mapper.Map(project.ProjectFundings.ToList()[i].User, projectFundingModel.UserModel);
                        Mapper.Map(project.ProjectFundings.ToList()[i].Status, projectFundingModel.Status);
                        //projectFundingModel.TotalAmount = projectFundingModel.FundingAmount + projectFundingModel.ProcessingFees;
                        if (project.ProjectFundings.ToList()[i].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || project.ProjectFundings.ToList()[i].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            projectModel.TotalFundingAmount = Math.Round(projectModel.TotalFundingAmount + project.ProjectFundings.ToList()[i].FundingAmount, 2);
                            //projectModel.TotalProcessingAmount = Math.Round(projectModel.TotalProcessingAmount + project.ProjectFundings.ToList()[i].ProcessingFees, 2);
                        }
                        projectFundingModel.SiteCommission = Math.Round(projectFundingModel.FundingAmount * ReadConfiguration.SiteCommission, 2);

                        projectFundingModel.TransactionCharges = Math.Round(projectFundingModel.FundingAmount != 0 ? ((projectFundingModel.FundingAmount - projectFundingModel.SiteCommission) * (decimal)(0.029)) : 0, 2);
                        //projectFundingModel.TransactionCharges = Math.Round(projectFundingModel.ProcessingFees,2);//Math.Round(projectFundingModel.PaymentReceived - (projectFundingModel.FundingAmount - projectFundingModel.SiteCommission), 2);

                        projectFundingModel.PaymentReceived = Math.Round(projectFundingModel.FundingAmount - projectFundingModel.SiteCommission - projectFundingModel.TransactionCharges, 2);//Math.Round(projectFundingModel.FundingAmount != 0 ? ((projectFundingModel.FundingAmount - projectFundingModel.SiteCommission) + (decimal)(0.30)) / ((decimal)(1-0.029)) : 0,2); 
                        projectModel.ProjectFundings.Add(projectFundingModel);
                    }
                    else
                    {
                        if (project.ProjectFundings.ToList()[i].StatusId == (int)CommonEnums.ProjectStatusEnum.Approved || project.ProjectFundings.ToList()[i].StatusId == (int)CommonEnums.ProjectStatusEnum.Funded)
                        {
                            var projectFundingModel = new ProjectFundingModel();
                            Mapper.Map(project.ProjectFundings.ToList()[i], projectFundingModel);
                            Mapper.Map(project.ProjectFundings.ToList()[i].User, projectFundingModel.UserModel);
                            Mapper.Map(project.ProjectFundings.ToList()[i].Status, projectModel.Status);
                            //projectFundingModel.TotalAmount = projectFundingModel.FundingAmount + projectFundingModel.ProcessingFees;

                            projectModel.ProjectFundings.Add(projectFundingModel);
                        }

                    }

                }
                projectModel.SiteCommission = Math.Round(projectModel.TotalFundingAmount * ReadConfiguration.SiteCommission, 2);
                projectModel.TransactionCharges = Math.Round(projectModel.TotalFundingAmount != 0 ? ((projectModel.TotalFundingAmount - projectModel.SiteCommission) * (decimal)(0.029)) + (decimal)0.30 : 0, 2);
                projectModel.PaymentReceived = Math.Round(projectModel.TotalFundingAmount - projectModel.SiteCommission - projectModel.TransactionCharges, 2);//Math.Round(projectModel.TotalFundingAmount != 0 ? ((projectModel.TotalFundingAmount - projectModel.SiteCommission) + (decimal)(0.30)) / ((decimal)(1 - 0.029)) : 0, 2);//
                                                                                                                                                              //projectModel.TransactionCharges = Math.Round(projectModel.PaymentReceived - (projectModel.TotalFundingAmount - projectModel.SiteCommission), 2);//Math.Round(projectModel.TotalFundingAmount != 0 ? ((projectModel.TotalFundingAmount - projectModel.SiteCommission) * (decimal)(0.029)) + (decimal)0.30 : 0,2);
                projectModel.ProjectFundings = projectModel.ProjectFundings.OrderByDescending(x => x.FundingDate).ToList();

                return Task.FromResult(projectModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<ProjectFundingModel> GetLoggedInUserFunding(string id)
        {
            var userId = GetLoginUserId();
            Project project = _unitOfWork.projectRepository.GetById(id.Decrypt());

            ProjectFundingModel projectFundingModel = new ProjectFundingModel();
            foreach (var fundingModel in project.ProjectFundings)
            {

                var fundingUserId = fundingModel.FundingUserId;
                if (userId == fundingUserId && fundingModel.StatusId != (int)CommonEnums.ProjectStatusEnum.BackedFunding && fundingModel.StatusId != (int)CommonEnums.ProjectStatusEnum.Declined)
                {
                    projectFundingModel = Mapper.Map(fundingModel, projectFundingModel);
                    return Task.FromResult(projectFundingModel);
                }
            }
            return null;
        }
        public Task<int> GetProjectCount(string name)
        {
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            if (name != null)
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && (data.Title.Contains(name) || data.Location.Contains(name))
                 && data.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => data.ProjectId == s));
                return Task.FromResult(count);
            }
            else
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                  && data.FundingDuration > DateTime.Now && !featuredProjectIds.Any(s => data.ProjectId == s));
                return Task.FromResult(count);
            }
        }
        public Task<int> GetFeaturedProjectsCount(string name)
        {
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            var featuredProjectIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ClciksLeft > 0).Select(x => x.ProjectId).Distinct().ToList();
            if (name != null)
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && (data.Title.Contains(name) || data.Location.Contains(name))
                   && data.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => data.ProjectId == s));
                return Task.FromResult(count);
            }
            else
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && data.FundingDuration > DateTime.Now
                   && data.FundingDuration > DateTime.Now && featuredProjectIds.Any(s => data.ProjectId == s));
                return Task.FromResult(count);
            }
        }

        public Task<int> GetMyProjectCount(string name)
        {
            var userId = GetLoginUserId();
            //var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            if (name != null)
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.OwnerId == userId && (data.Title.Contains(name) || data.Location.Contains(name)));
                return Task.FromResult(count);
            }
            else
            {
                int count = _unitOfWork.projectRepository.GetCount(data => data.OwnerId == userId);
                return Task.FromResult(count);
            }
        }
        public Task<int> GetProjectCountByIndustryId(int id)
        {
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && data.IndustryId == id && data.FundingDuration > DateTime.Now);
            return Task.FromResult(count);

        }

        public Task<int> GetFeaturedCountByIndustryId(int id)
        {
            var statusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Active);
            int count = _unitOfWork.projectRepository.GetCount(data => data.StatusId == statusId && data.IndustryId == id && data.FundingDuration > DateTime.Now);
            return Task.FromResult(count);

        }
        public Task<bool> GetIfLoggedInUserIsProjectOwner(string id, int pageType)
        {
            var userId = GetLoginUserId();
            Project project = _unitOfWork.projectRepository.GetById(id.Decrypt());

            if (pageType == (int)CommonEnums.GetProjectByType.ViewProject && userId == project.CreatedBy)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
        public Task<bool> GetIfLogginInUserCanPay(string projectId)
        {
            var userId = GetLoginUserId();
            Project project = _unitOfWork.projectRepository.GetById(projectId.Decrypt());

            if (userId == project.CreatedBy)
            {
                return Task.FromResult(false);
            }
            for (int i = 0; i < project.ProjectFundings.Count; i++)
            {
                if (project.ProjectFundings.ToList()[i].FundingUserId == userId && project.ProjectFundings.ToList()[i].StatusId != (int)CommonEnums.ProjectStatusEnum.BackedFunding)
                {
                    return Task.FromResult(false);
                }
            }
            return Task.FromResult(true);
        }

        public Task<List<AdLogModel>> GetClicksLog(string projectId)
        {
            try
            {
                var ProjectId = projectId.Decrypt();
                List<AdLogModel> adLogModel = new List<AdLogModel>();
                var featuredIdeaIds = _unitOfWork.featuredIdeaRepository.Get(x => x.ProjectId == ProjectId).Select(x => x.FeaturedIdeaId).Distinct().ToList();
                // var projectFundings = _unitOfWork.projectFundingRepository.Get(data => data.FundingUserId == userId && data.ProjectId == projectId).ToList();
                var adLogs = _unitOfWork.adLogRepository.Get(data => featuredIdeaIds.Any(x => data.FeatureIdeaId == x)).ToList();
                Mapper.Map(adLogs, adLogModel);

                return Task.FromResult(adLogModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AddUpdateMethods

        public Task<ProjectModel> AddUpdateProject(ProjectModel model)
        {
            try
            {
                var project = new Project();
                var userId = GetLoginUserId();
                if (model.ProjectId == null)// This section checks if ProjectId not exist and Create new Project.
                {
                    Mapper.Map(model, project);
                    project.OwnerId = GetLoginUserId();
                    project.CreatedBy = GetLoginUserId();
                    //var date = DateTime.UtcNow.AddDays(30).toDateString();
                    project.FundingDuration = DateTime.UtcNow.AddDays(30);// This is code for time being!! will be removed.
                    project.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Inactive);
                    _unitOfWork.projectRepository.Insert(project);
                    _unitOfWork.Save();
                    project = _unitOfWork.projectRepository.GetById(project.ProjectId);
                    Mapper.Map(project, model);
                    return Task.FromResult(model);
                }
                var projectId = model.ProjectId.Decrypt();
                project = _unitOfWork.projectRepository.Get(data => data.ProjectId.Equals(projectId) && data.OwnerId == userId).FirstOrDefault();

                if (model.TabType == 1)
                {

                    project.LogoName = model.LogoName;
                    project.LogoPath = model.LogoPath;
                    project.Title = model.Title;
                    project.ProjectTitle = model.ProjectTitle;
                    project.ShortDescription = model.ShortDescription;
                    project.Description = model.Description;
                    project.IndustryId = model.IndustryId.Decrypt();
                    project.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Inactive);
                    project.UpdatedBy = GetLoginUserId();
                    _unitOfWork.projectRepository.Update(project);
                    _unitOfWork.Save();
                    project = _unitOfWork.projectRepository.GetById(project.ProjectId);
                    Mapper.Map(project, model);
                    return Task.FromResult(model);
                }
                if (model.TabType == 2)
                {
                    var financialBreakDowns = new List<FinancialBreakdown>();
                    var newfinancialBreakDowns = new List<FinancialBreakdown>();

                    project.CompanyName = ""; //model.CompanyName;
                    project.Location = model.Location;
                    project.AnnualSales = model.AnnualSales;
                    project.EquityOffered = model.EquityOffered;
                    project.InvestmentAmount = model.InvestmentAmount;
                    project.FundingDuration = model.FundingDuration.toDateTime();
                    project.MinPledge = model.MinPledge;
                    project.UpdatedBy = GetLoginUserId();
                    project.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Inactive);
                    _unitOfWork.projectRepository.Update(project);

                    Mapper.Map(model.FinancialBreakdowns, newfinancialBreakDowns);
                    newfinancialBreakDowns.ForEach(id => id.ProjectId = project.ProjectId);
                    //Insert Financial Breakdown
                    var insertFinancialBreakDowns = newfinancialBreakDowns.Where(data => data.FinancialBreakdownId.Equals(0)).ToList();
                    _unitOfWork.financialBreakdownRepository.InsertAll(insertFinancialBreakDowns);

                    //Delete Financial Breakdown
                    int[] ids = model.FinancialBreakdowns.Select(x => x.FinancialBreakdownId).ToArray();
                    financialBreakDowns = _unitOfWork.financialBreakdownRepository.Get(data => data.ProjectId.Equals(project.ProjectId) &&
                        !ids.Contains(data.FinancialBreakdownId)).ToList();
                    //var deleteFinancialBreakdowns = financialBreakDowns.Except(newfinancialBreakDowns).ToList();
                    //var deleteFinancialBreakdowns = new List<FinancialBreakdown>();
                    //foreach (var item in financialBreakDowns)
                    //{
                    //    if (!newfinancialBreakDowns.Any(x => x.FinancialBreakdownId == item.FinancialBreakdownId))
                    //    {
                    //        deleteFinancialBreakdowns.Add(item);
                    //    }
                    //}

                    _unitOfWork.financialBreakdownRepository.DeleteAll(financialBreakDowns);

                    //Update Financial Breakdown
                    var updateModelFinancialBreakDowns = model.FinancialBreakdowns.Where(data => !data.FinancialBreakdownId.Equals(0)).ToList();
                    newfinancialBreakDowns = new List<FinancialBreakdown>();
                    Mapper.Map(updateModelFinancialBreakDowns, newfinancialBreakDowns);
                    _unitOfWork.financialBreakdownRepository.UpdateAll(newfinancialBreakDowns);
                    _unitOfWork.Save();
                    project = _unitOfWork.projectRepository.GetById(project.ProjectId);
                    Mapper.Map(project, model);
                    return Task.FromResult(model);
                }
                if (model.TabType == 3)
                {
                    project.PhoneNumber = model.PhoneNumber;
                    project.Address = model.Address;
                    project.WebsiteLink = model.WebsiteLink;
                    project.Email = model.Email;

                    project.PaypalAccount = model.PaypalAccount;
                    project.PaypalAccountStatus = model.PaypalAccountStatus;
                    project.UpdatedBy = GetLoginUserId();
                    project.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Inactive);
                    _unitOfWork.projectRepository.Update(project);
                    _unitOfWork.Save();
                    project = _unitOfWork.projectRepository.GetById(project.ProjectId);
                    Mapper.Map(project, model);
                    return Task.FromResult(model);
                }
                else
                {
                    project.ImageName = model.ImagePath != null ? model.ImagePath.Substring(0, model.ImagePath.LastIndexOf("_")) : "";
                    project.ImagePath = model.ImagePath;
                    project.VideoPath = model.VideoPath;
                    project.VideoName = model.VideoName;
                    _unitOfWork.projectRepository.Update(project);
                    _unitOfWork.Save();
                    project = _unitOfWork.projectRepository.GetById(project.ProjectId);
                    Mapper.Map(project, model);
                    return Task.FromResult(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<ProjectDocumentModel> AddUpdateProjectDocument(ProjectDocumentModel model)
        {
            try
            {
                if (model == null)
                {
                    return null;
                }
                var projectDocument = new ProjectDocument();
                Mapper.Map(model, projectDocument);
                if (model.ProjectDocumentId.Equals(0))
                {
                    _unitOfWork.projectDocumentRepository.Insert(projectDocument);
                    _unitOfWork.Save();
                    Mapper.Map(projectDocument, model);
                    return Task.FromResult(model);
                }
                _unitOfWork.projectDocumentRepository.Update(projectDocument);
                _unitOfWork.Save();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<AuthenticationServiceResponse> SubmitProjectReview(string id)
        {
            var projectModel = new ProjectModel();
            var projectId = id.Decrypt();
            var project = _unitOfWork.projectRepository.GetById(projectId);
            if (project == null)
            {
                Task.FromResult(new AuthenticationServiceResponse { Message = "No records found", Success = false });
            }
            project.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
            _unitOfWork.projectRepository.Update(project);
            Mapper.Map(project, projectModel);
            _unitOfWork.Save();

            MailHelper helper = new MailHelper();
            helper.ToEmail = "super@mailinator.com";
            helper.Subject = MailSubject.NewProjectSubmit.ToString();

            var body = helper.GenerateMailBody("SubmitProject.html");
            body = body.Replace("{ProjectName}", project.Title);
            //body = body.Replace("{ProjectDetails}", project.Description);
            body = body.Replace("{ProjectLink}", HttpContext.Current.Request.UrlReferrer.ToString() + "#/projectdetail/" + id);
            body = body.Replace("{ProjectOwner}", project.User.FirstName + " " + project.User.LastName);
            helper.Body = body;
            helper.SendEmail();

            return Task.FromResult(new AuthenticationServiceResponse { Data = projectModel, Message = "Request Send successfully!! Please wait for approval, It can take maximum 2 days", Success = true });
        }

        public Task<AuthenticationServiceResponse> UpdateFundingStatus(string projectFundingId, bool IsApproved)
        {
            try
            {
                ProjectFundingModel projectFundingModel = new ProjectFundingModel();
                var ProjectFundingId = projectFundingId.Decrypt();
                var projectFunding = _unitOfWork.projectFundingRepository.Get(x => x.ProjectFundingId == ProjectFundingId).FirstOrDefault();
                projectFunding.StatusId = IsApproved == true ? (int)CommonEnums.ProjectStatusEnum.Approved : (int)CommonEnums.ProjectStatusEnum.Declined;
                _unitOfWork.projectFundingRepository.Update(projectFunding);
                _unitOfWork.Save();

                var project = _unitOfWork.projectRepository.GetById(projectFunding.ProjectId);


                MailHelper helper = new MailHelper();
                helper.ToEmail = projectFunding.User.Email;
                var body = "";
                if (IsApproved == true)
                {
                    helper.Subject = MailSubject.ApproveFunding.ToString();
                    body = helper.GenerateMailBody("ApproveFunding.html");
                }
                else
                {
                    helper.Subject = MailSubject.DisapproveFunding.ToString();
                    body = helper.GenerateMailBody("DisapproveFunding.html");
                }
                body = body.Replace("{ProjectName}", project.Title);
                body = body.Replace("{EncryptedProjectId}", project.ProjectId.Encrypt());
                body = body.Replace("{FirstName}", projectFunding.User.FirstName);
                body = body.Replace("{LastName}", projectFunding.User.LastName);
                helper.Body = body;
                helper.SendEmail();

                //projectFunding.Project = null;
                Mapper.Map(projectFunding, projectFundingModel);
                return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = IsApproved == true ? "Approved successfully" : "Disapproved successfully", Data = projectFundingModel });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = "Something Went wrong" });
            }
        }

        public Task<AuthenticationServiceResponse> UpdateProjectStatus(string projectId, bool Status)
        {
            try
            {
                var ProjectId = projectId.Decrypt();
                var project = _unitOfWork.projectRepository.Get(x => x.ProjectId == ProjectId).FirstOrDefault();
                project.StatusId = Status == true ? (int)CommonEnums.ProjectStatusEnum.Active : (int)CommonEnums.ProjectStatusEnum.Declined;
                _unitOfWork.projectRepository.Update(project);
                _unitOfWork.Save();

                MailHelper helper = new MailHelper();
                helper.ToEmail = project.User.Email;
                helper.Subject = Status == true ? MailSubject.AcceptProject.ToString() : MailSubject.DeclineProject.ToString();
                var body = "";

                if (Status == true)
                {
                    body = helper.GenerateMailBody("ApproveProject.html");
                    body = body.Replace("{FirstName}", project.User.FirstName);
                    body = body.Replace("{LastName}", project.User.LastName);
                    body = body.Replace("{Title}", project.Title);
                    body = body.Replace("{EncryptedProjectId}", project.ProjectId.Encrypt());
                }
                else
                {
                    body = helper.GenerateMailBody("DeclineProject.html");
                    body = body.Replace("{FirstName}", project.User.FirstName);
                    body = body.Replace("{LastName}", project.User.LastName);
                    body = body.Replace("{EncryptedProjectId}", project.ProjectId.Encrypt());
                }


                helper.Body = body;
                helper.SendEmail();


                return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = Status == true ? "Accepted successfully" : "Declined successfully", Data = project.Status });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = "Something Went wrong" });
            }
        }


        public Task<AuthenticationServiceResponse> UpdateProjectStatus(string projectId)
        {
            List<ProjectFundingModel> projectFundingModelList = new List<ProjectFundingModel>();
            var ProjectId = projectId.Decrypt();
            var project = _unitOfWork.projectRepository.Get(x => x.ProjectId == ProjectId).FirstOrDefault();
            project.IsFunded = true;
            _unitOfWork.projectRepository.Update(project);
            _unitOfWork.Save();
            Mapper.Map(project.ProjectFundings, projectFundingModelList);
            return Task.FromResult(new AuthenticationServiceResponse { Success = true, Data = projectFundingModelList.OrderByDescending(x => x.FundingDate) });

        }

        public void UpdateProjectTransferStatus(string projectId)
        {
            var ProjectId = projectId.Decrypt();
            var project = _unitOfWork.projectRepository.Get(x => x.ProjectId == ProjectId).FirstOrDefault();
            project.IsTransfered = true;
            _unitOfWork.projectRepository.Update(project);
            _unitOfWork.Save();

        }

        public void updateProjectFunding(int ProjectFundingId, int TransactionId, string Status)
        {
            ProjectFunding projectFunding = _unitOfWork.projectFundingRepository.Get(x => x.ProjectFundingId == ProjectFundingId).FirstOrDefault();
            projectFunding.TransactionId = TransactionId;
            if (Status.ToLower() == "approved")
            {
                projectFunding.StatusId = (int)(CommonEnums.ProjectStatusEnum.Funded);
            }
            else
            {
                projectFunding.StatusId = (int)(CommonEnums.ProjectStatusEnum.Failed);
            }
            _unitOfWork.projectFundingRepository.Update(projectFunding);
            _unitOfWork.Save();
            //return Task.FromResult(projectFundingModel);
        }

        #endregion

        #region DeleteMethods
        public Task<AuthenticationServiceResponse> DeleteProjectDocument(ProjectDocumentModel model)
        {
            if (model.ProjectDocumentId.Equals(0))
            {
                return null;
            }
            var doc = _unitOfWork.projectDocumentRepository.GetById(model.ProjectDocumentId);
            if (doc == null)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = "Document does not exist" });
            }
            _unitOfWork.projectDocumentRepository.Delete(doc.ProjectDocumentId);
            DeleteFile(doc.DocumentPath);
            _unitOfWork.Save();

            doc = _unitOfWork.projectDocumentRepository.GetById(model.ProjectDocumentId);
            if (doc == null)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = "Document deleted successfully" });
            }
            else
                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = "Unable to delete document" });
        }
        public Task<AuthenticationServiceResponse> DeleteProject(string projectId)
        {

            var project = _unitOfWork.projectRepository.GetById(projectId.Decrypt());
            if (project == null)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false });
            }
            List<ProjectDocument> projectDocuments = project.ProjectDocuments.ToList();
            List<ProjectFunding> projectFundings = project.ProjectFundings.ToList();
            List<FinancialBreakdown> financialBreakdowns = project.FinancialBreakdowns.ToList();
            _unitOfWork.projectDocumentRepository.DeleteAll(projectDocuments);
            _unitOfWork.projectFundingRepository.DeleteAll(projectFundings);
            _unitOfWork.financialBreakdownRepository.DeleteAll(financialBreakdowns);
            _unitOfWork.projectRepository.Delete(project.ProjectId);
            _unitOfWork.Save();
            project = _unitOfWork.projectRepository.GetById(project.ProjectId);
            if (project != null)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Success = false });
            }
            return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = "Deleted Succesfully" });
        }

        public Task<bool> BackItFunding(string id)
        {
            try
            {
                var userId = GetLoginUserId();
                var projectId = id.Decrypt();
                ProjectFundingModel projectFundingModel = new ProjectFundingModel();
                // var projectFundings = _unitOfWork.projectFundingRepository.Get(data => data.FundingUserId == userId && data.ProjectId == projectId).ToList();
                var projectFunding = _unitOfWork.projectFundingRepository.Get(data => data.FundingUserId == userId && data.ProjectId == projectId).LastOrDefault();
                //foreach (var projectFunding in projectFundings)
                //{
                projectFunding.StatusId = (int)CommonEnums.ProjectStatusEnum.BackedFunding;
                _unitOfWork.projectFundingRepository.Update(projectFunding);
                //}
                _unitOfWork.Save();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> deductFeaturedClick(string projectId)
        {
            try
            {
                //Find IP Adress
                //string IPAdd = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (string.IsNullOrEmpty(IPAdd))
                //    IPAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                string hostName = System.Net.Dns.GetHostName(); // Retrive the Name of HOST
                                                                // Get the IP
                string IPAdd = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();

                var ProjectId = projectId.Decrypt();
                var featuredIdea = _unitOfWork.featuredIdeaRepository.Get(data => data.ProjectId == ProjectId && data.ClciksLeft > 0).FirstOrDefault();
                if (featuredIdea.ClciksLeft > 0)
                {
                    AdLog logFeaturedClick = new AdLog();
                    logFeaturedClick.FeatureIdeaId = featuredIdea.FeaturedIdeaId;
                    logFeaturedClick.DateTimeOfClick = DateTime.UtcNow;
                    logFeaturedClick.IpAddress = IPAdd;
                    try
                    {
                        logFeaturedClick.CreatedBy = GetLoginUserId();
                    }
                    catch (Exception ex)
                    {

                        logFeaturedClick.CreatedBy = null;
                    }
                    logFeaturedClick.CreatedDate = DateTime.UtcNow;

                    _unitOfWork.adLogRepository.Insert(logFeaturedClick);

                    featuredIdea.ClciksLeft -= 1;
                    _unitOfWork.featuredIdeaRepository.Update(featuredIdea);
                    _unitOfWork.Save();
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        #endregion

        #endregion

        #region Payments
        public Task<List<ProjectFundingModel>> GetMyPayments(int PageNumber, int PageSize, out int PaymentCount)
        {
            var projectFundingModel = new List<ProjectFundingModel>();
            var userId = GetLoginUserId();
            PaymentCount = _unitOfWork.projectFundingRepository.GetCount(data => data.FundingUserId == userId);
            List<ProjectFunding> projectFunding = _unitOfWork.projectFundingRepository.Get(data => data.FundingUserId == userId, includeProperties: "Project").OrderByDescending(data => data.FundingDate).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            Mapper.Map(projectFunding, projectFundingModel);

            return Task.FromResult(projectFundingModel);
        }
        public Task<List<ProjectFundingModel>> GetProjectFundings(string ProjectId, int PageNumber, int PageSize, out int ProjectFundingCount)
        {
            var projectFundingModel = new List<ProjectFundingModel>();
            var projectId = ProjectId.Decrypt();
            ProjectFundingCount = _unitOfWork.projectFundingRepository.GetCount(data => data.ProjectId == projectId);
            List<ProjectFunding> projectFunding = _unitOfWork.projectFundingRepository.Get(data => data.ProjectId == projectId).OrderByDescending(data => data.FundingDate).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();


            Mapper.Map(projectFunding, projectFundingModel);

            return Task.FromResult(projectFundingModel);
        }
        public Task<AuthenticationServiceResponse> AddProjectFunding(PaymentModel paymentModel)
        {
            try
            {
                ProjectFundingModel projectFundingModel = new ProjectFundingModel();
                var projectFunding = new ProjectFunding();
                var cardDetail = new CardDetail();
                var user_project_cardDetail = new User_Project_CardDetail();
                var userId = GetLoginUserId();
                var projectId = paymentModel.ProjectId.Decrypt();

                projectFunding = _unitOfWork.projectFundingRepository.Get(data => data.FundingUserId == userId && data.ProjectId == projectId
                    && data.StatusId != (int)CommonEnums.ProjectStatusEnum.BackedFunding && data.StatusId != (int)CommonEnums.ProjectStatusEnum.Declined).FirstOrDefault();
                if (projectFunding != null)
                {
                    return Task.FromResult(new AuthenticationServiceResponse { Data = paymentModel, Success = false, Message = ErrorMessageConstants.ProjectFunding_Exist });
                }
                projectFunding = new ProjectFunding();
                projectFunding.FundingAmount = Convert.ToDecimal(paymentModel.FundingAmount);
                projectFunding.ProcessingFees = Convert.ToDecimal(paymentModel.ProcessingFees);
                projectFunding.ProjectId = paymentModel.ProjectId.Decrypt();
                projectFunding.FundingUserId = userId;
                projectFunding.FundingDate = DateTime.UtcNow;
                projectFunding.StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.WaitingForApproaval);
                projectFunding.EquityDemanded = paymentModel.EquityDemanded;

                //Save projectFunding
                _unitOfWork.projectFundingRepository.Insert(projectFunding);

                //Start saving card details.
                Mapper.Map(paymentModel, cardDetail);
                //cardDetail.UserId = userId;
                //var exstCard = _unitOfWork.cardDetailRepository.Get(card => card.CardNumber == cardDetail.CardNumber && card.UserId == userId).FirstOrDefault();
                //if (exstCard == null)
                //{
                cardDetail.UserId = userId;
                _unitOfWork.cardDetailRepository.Insert(cardDetail);
                _unitOfWork.Save();
                //}
                //else
                //{
                //    cardDetail = exstCard;
                //}

                var existingUserProjectCardDetails = _unitOfWork.user_Project_CardDetailRepository.Get(x => x.UserId == userId && x.ProjectId == projectId && x.CardDetailId == cardDetail.CardDetailId).FirstOrDefault();
                if (existingUserProjectCardDetails == null)
                {
                    //Start saving records for selected credit card for selected project
                    user_project_cardDetail.ProjectId = projectFunding.ProjectId;
                    user_project_cardDetail.CardDetailId = cardDetail.CardDetailId;
                    user_project_cardDetail.UserId = userId;
                    _unitOfWork.user_Project_CardDetailRepository.Insert(user_project_cardDetail);
                }
                _unitOfWork.Save();

                var project = _unitOfWork.projectRepository.GetById(projectId);

                MailHelper helper = new MailHelper();
                helper.ToEmail = project.User.Email;
                helper.Subject = MailSubject.Newinvestor.ToString();

                var body = helper.GenerateMailBody("AddInvestment.html");
                body = body.Replace("{FirstName}", project.User.FirstName);
                body = body.Replace("{LastName}", project.User.LastName);
                body = body.Replace("{ProjectName}", project.Title);
                body = body.Replace("{EncryptedProjectId}", project.ProjectId.Encrypt());
                helper.Body = body;
                helper.SendEmail();

                //projectFunding.Project = null;
                Mapper.Map(projectFunding, projectFundingModel);
                return Task.FromResult(new AuthenticationServiceResponse { Data = projectFundingModel, Success = true });
            }
            catch (Exception ex)
            {

                throw ex;
            }




        }

        public Task<AuthenticationServiceResponse> AddFeaturedClicksPayment(FeaturedClicksPaymentModel paymentModel, TransactionModel transactionModel)
        {
            try
            {
                var transaction = new Transaction();
                Mapper.Map(transactionModel, transaction);
                _unitOfWork.transactionRepository.Insert(transaction);

                var featuredIdea = new FeaturedIdea();
                var featuredIdeaModel = new FeaturedIdeaModel();
                var cardDetail = new CardDetail();
                var userId = GetLoginUserId();
                var projectId = paymentModel.ProjectId.Decrypt();

                //Start saving card details.
                Mapper.Map(paymentModel, cardDetail);
                cardDetail.UserId = userId;
                _unitOfWork.cardDetailRepository.Insert(cardDetail);
                _unitOfWork.Save();

                featuredIdea = new FeaturedIdea();
                featuredIdea.Amount = Convert.ToDecimal(paymentModel.Amount);
                featuredIdea.ProjectId = paymentModel.ProjectId.Decrypt();
                featuredIdea.CreatedBy = userId;
                featuredIdea.TotalClicks = featuredIdea.ClciksLeft = paymentModel.Clicks;
                featuredIdea.CardDetailId = cardDetail.CardDetailId;

                //Save FeaturedIdea
                _unitOfWork.featuredIdeaRepository.Insert(featuredIdea);
                _unitOfWork.Save();

                featuredIdea = _unitOfWork.featuredIdeaRepository.GetById(featuredIdea.FeaturedIdeaId);
                Mapper.Map(featuredIdea, featuredIdeaModel);

                return Task.FromResult(new AuthenticationServiceResponse { Data = featuredIdeaModel, Success = true });
            }
            catch (Exception ex)
            {

                return Task.FromResult(new AuthenticationServiceResponse { Success = false, Message = ex.InnerException.Message });
            }

        }
        public Task<AuthenticationServiceResponse> AddFailedTransaction(TransactionModel transactionModel, out int TransactionId)
        {
            try
            {
                var transaction = new Transaction();
                Mapper.Map(transactionModel, transaction);
                _unitOfWork.transactionRepository.Insert(transaction);
                _unitOfWork.Save();
                TransactionId = transaction.TransactionId;
                Mapper.Map(transaction, transactionModel);
                return Task.FromResult(new AuthenticationServiceResponse { Data = transactionModel, Success = false, Message = transaction.Message });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int getLoggedInUserId()
        {
            var userId = GetLoginUserId();
            return userId;
        }
        public string getLoggedInUserEmail()
        {
            var userEmail = GetLoginUserEmail();
            return userEmail;
        }


        public List<PaymentModel> getProjectFundings(string projectId)
        {
            try
            {
                var ProjectId = projectId.Decrypt();
                var StatusId = Convert.ToInt32(CommonEnums.ProjectStatusEnum.Approved);
                var projectFundings = _unitOfWork.projectFundingRepository.Get(x => x.ProjectId == ProjectId && x.StatusId == StatusId).ToList();
                List<User_Project_CardDetail> user_Project_CardDetails = _unitOfWork.user_Project_CardDetailRepository.Get(x => x.ProjectId == ProjectId).ToList();
                List<CardDetail> cardDetails = new List<CardDetail>();
                List<PaymentModel> paymentModels = new List<PaymentModel>();
                foreach (var item in projectFundings)
                {
                    PaymentModel paymentModel = new PaymentModel();
                    var user_Project_CardDetail = user_Project_CardDetails.Where(x => x.UserId == item.User.Id).LastOrDefault();
                    var cardDetail = _unitOfWork.cardDetailRepository.GetById(user_Project_CardDetail.CardDetailId);
                    Mapper.Map(cardDetail, paymentModel);
                    Mapper.Map(item, paymentModel);
                    paymentModel.FundingAmount = item.FundingAmount.ToString();
                    paymentModels.Add(paymentModel);
                }
                return paymentModels;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        //#region Message

        //#region Get
        //public Task<AuthenticationServiceResponse> GetMessages()
        //{
        //    try
        //    {
        //        var userId = GetLoginUserId();
        //        List<MessageModel> messagesModel = new List<MessageModel>();
        //        var messages = _messageService.MessageRepository.Get(x => (x.Project.OwnerId == userId || x.CreatedBy == userId) && x.IsPrivate == true && x.IsDeleted == false).ToList();
        //        Mapper.Map(messages, messagesModel);

        //        for (int i = 0; i < messages.Count; i++)
        //        {
        //            Mapper.Map(messages[i].Project, messagesModel[i].Project);
        //            Mapper.Map(messages[i].User, messagesModel[i].UserModel);
        //        }


        //        //Dictionary<Project, Dictionary<ApplicationUser, List<Message>>> Conversation = new Dictionary<Project, Dictionary<ApplicationUser, List<Message>>>();
        //        //Dictionary<ProjectModel, Dictionary<UserModel, List<MessageModel>>> ConversationModel = new Dictionary<ProjectModel, Dictionary<UserModel, List<MessageModel>>>();
        //        Dictionary<string, Dictionary<string, List<MessageModel>>> ConversationModel = new Dictionary<string, Dictionary<string, List<MessageModel>>>();


        //        //var project = new Project();
        //        //var user = new ApplicationUser();
        //        //var message = new List<Message>();

        //        var projectModel = new ProjectModel();
        //        var userModel = new UserModel();
        //        var messageModel = new List<MessageModel>();

        //        Dictionary<string, List<MessageModel>> ConversationValue = new Dictionary<string, List<MessageModel>>();


        //        foreach (var item in messagesModel)
        //        {
        //            if (!ConversationValue.ContainsKey(item.UserModel.Id))
        //            {
        //                messageModel = new List<MessageModel>();
        //                messageModel.Add(item);
        //                ConversationValue.Add(item.UserModel.Id, messageModel);
        //            }
        //            else
        //            {
        //                //user = ConversationValue.Where(x => x.Key == item.User).Select(x => x.Key).FirstOrDefault();
        //                messageModel = ConversationValue.Where(x => x.Key == item.UserModel.Id).Select(x => x.Value).FirstOrDefault();
        //                messageModel.Add(item);
        //            }

        //            if (!ConversationModel.ContainsKey(item.ProjectId))
        //            {
        //                //project = item.Project;
        //                ConversationModel.Add(item.ProjectId, ConversationValue);
        //            }
        //        }

        //        //                Dictionary<ApplicationUser, List<Message>> ConversationValue = new Dictionary<ApplicationUser, List<Message>>();

        //        //foreach (var item in messagesModel)
        //        //{
        //        //    var project1 = item.Project;
        //        //    if (!ConversationValue.ContainsKey(item.User))
        //        //    {
        //        //        user = item.User;
        //        //        message = new List<Message>();
        //        //        item.Project = new Project();
        //        //        item.User = new ApplicationUser();
        //        //        message.Add(item);
        //        //        ConversationValue.Add(user, message);
        //        //    }
        //        //    else
        //        //    {
        //        //        user = ConversationValue.Where(x => x.Key == item.User).Select(x => x.Key).FirstOrDefault();
        //        //        message = ConversationValue.Where(x => x.Key == item.User).Select(x => x.Value).FirstOrDefault();
        //        //        item.Project = new Project();
        //        //        item.User = new ApplicationUser();
        //        //        message.Add(item);
        //        //    }

        //        //    if (!Conversation.ContainsKey(project1))
        //        //    {
        //        //        project = item.Project;
        //        //        Conversation.Add(project1, ConversationValue);
        //        //    }
        //        //}
        //        //var a = JsonConvert.SerializeObject(Conversation, Formatting.Indented, new JsonSerializerSettings() { 
        //        //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore});




        //        return Task.FromResult(new AuthenticationServiceResponse { Data = ConversationModel, Success = true });
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //#endregion

        //#region Add
        //public Task<AuthenticationServiceResponse> AddComment(MessageModel messageModel)
        //{
        //    try
        //    {
        //        Message message = new Message();
        //        Mapper.Map(messageModel, message);
        //        message.CreatedBy = GetLoginUserId();
        //        //message.CreatedDate = DateTime.UtcNo;w
        //        message.IsDeleted = false;
        //        _messageService.MessageRepository.Insert(message);
        //        _unitOfWork.Save();
        //        message = _messageService.MessageRepository.GetById(message.MessageId);
        //        Mapper.Map(message, messageModel);

        //        var user = _unitOfWork.applicationUserRepository.GetById(message.CreatedBy);
        //        Mapper.Map(user, messageModel.UserModel);

        //        return Task.FromResult(new AuthenticationServiceResponse { Data = messageModel, Success = true, Message = "Comment added succesfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //#endregion

        //#region Delete
        //public Task<AuthenticationServiceResponse> DeleteComment(MessageModel messageModel)
        //{
        //    try
        //    {
        //        Message message = new Message();
        //        Mapper.Map(messageModel, message);
        //        _messageService.MessageRepository.Delete(message);
        //        _messageService.Save();
        //        return Task.FromResult(new AuthenticationServiceResponse { Message = "Comment Deleted Successfully", Success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //#endregion
        //#endregion


    }
}

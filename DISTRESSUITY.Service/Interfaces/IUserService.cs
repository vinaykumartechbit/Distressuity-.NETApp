using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DISTRESSUITY.Service.Interfaces
{
    public interface IUserService
    {

        #region MyProject

        #region Get
        Task<List<ProjectModel>> GetMyProjects();
        Task<List<ProjectFundingModel>> GetMyPayments(int PageNumber,int PageSize, out int PaymentCount);
        Task<List<ProjectFundingModel>> GetProjectFundings(string ProjectId,int PageNumber, int PageSize, out int ProjectFundingCount);
        Task<List<ProjectModel>> GetProjects(out int ProjectCount);
        Task<List<ProjectModel>> GetFeaturedProjects(out int featuredProjectsCount);
        Task<List<ProjectModel>> GetAllProjects(int PageNumber, int PageSize, out int AllProjectCount);
        Task<List<ProjectModel>> GetFundedProject(int PageNumber, int PageSize, out int ProjectCount);
        Task<List<ProjectModel>> GetFilteredFundedProjectList(DateTime? StartDate, DateTime? EndDate);
        Task<int> GetProjectCount(string name);
        Task<int> GetFeaturedProjectsCount(string name);
        Task<int> GetMyProjectCount(string name);
        Task<int> GetProjectCountByIndustryId(int id);
        Task<int> GetFeaturedCountByIndustryId(int id);
        Task<ProjectModel> GetProjectById(string projectId, int pageType);
        List<string> CurrentUserPaypalAccounts();
        Task<ProjectFundingModel> GetLoggedInUserFunding(string id);
        List<object> GetProjectTabEnums();
        Task<List<ProjectModel>> SearchProjects(string name, string Id, out int projectsCount);
        Task<List<ProjectModel>> SearchFeaturedProjects(string name, string Id, out int featuredProjectsCount);
        Task<List<ProjectModel>> SearchMyProjects(string name, string id, out int projectsCount);
        Task<List<ProjectModel>> SearchAdminProjects( string name,string statusId);
        Task<List<StatusModel>> getStatusList();
        Task<List<ProjectModel>> GetAllProjetcsByStatus(string statusId, string searchKeyword);

        Task<List<ProjectModel>> LoadMoreProjects(int pageNumber, string name);
        Task<List<ProjectModel>> LoadMoreFeaturedProjects(int pageNumber, string name);

        Task<List<ProjectModel>> LoadMoreMyProjects(int pageNumber, string name);
        Task<bool> GetIfLoggedInUserIsProjectOwner(string id, int pageType);
        Task<bool> GetIfLogginInUserCanPay(string projectId);
        Task<List<AdLogModel>> GetClicksLog(string projectId);


        #endregion

        #region Update
        Task<AuthenticationServiceResponse> UpdateFundingStatus(string projectFundingId, bool IsApproved);
        Task<AuthenticationServiceResponse> UpdateProjectStatus(string projectId, bool Status);
        Task<AuthenticationServiceResponse> UpdateProjectStatus(string ProjectId);
        void UpdateProjectTransferStatus(string ProjectId);
        void updateProjectFunding(int ProjectFunding, int TransactionId,string Status);
        #endregion

        #region Delete
        Task<AuthenticationServiceResponse> DeleteProjectDocument(ProjectDocumentModel model);
        Task<AuthenticationServiceResponse> DeleteProject(string projectId);

        Task<bool> BackItFunding(string id);
        Task<bool> deductFeaturedClick(string projectId);

        #endregion

        #region Add
        Task<AuthenticationServiceResponse> AddProjectFunding(PaymentModel paymentModel);
        Task<AuthenticationServiceResponse> AddFeaturedClicksPayment(FeaturedClicksPaymentModel paymentModel, TransactionModel transactionModel);
        int getLoggedInUserId();
        string getLoggedInUserEmail();


        List<PaymentModel> getProjectFundings(string projectId);
        Task<AuthenticationServiceResponse> AddFailedTransaction(TransactionModel transactionModel, out int TransactionId);
        
        Task<ProjectModel> AddUpdateProject(ProjectModel model);
        Task<ProjectDocumentModel> AddUpdateProjectDocument(ProjectDocumentModel model);

        #endregion

       Task<string> ConvertVideoServiceByUrl(string Inputfile, string inputFormat, string outputFile);
        Task<string> ConvertVideoService( string Inputfile,string inputFormat, string outputFile);
        void DeleteFile(string path);
        #endregion

        #region MyProfile
        Task<UserModel> GetMyProfile();
        Task<UserModel> UploadProfileImage(UserModel userModel);
        Task<AuthenticationServiceResponse> UpdateUserProfile(UserModel model);
        Task<AuthenticationServiceResponse> SubmitProjectReview(string projectId);
        Task<List<ProjectModel>> FilterProjectsByIndustryId(string id,out int ProjectCount);
        Task<List<ProjectModel>> FilterFeaturedProjectsByIndustryId(string id, out int featuredProjectsCount);
        Task<AuthenticationServiceResponse> ChangePassword(string OldPassword, string NewPassword);

        #endregion

        #region Common Methods
        List<object> GetMonths();
        Task<List<IndustryModel>> GetIndustries();
        #endregion

        //#region Messages
        //#region Get
        //Task<AuthenticationServiceResponse> GetMessages();
        //#endregion

        //#region Add Update
        //Task<AuthenticationServiceResponse> AddComment(MessageModel message);
        //#endregion

        //#region Delete
        //Task<AuthenticationServiceResponse> DeleteComment(MessageModel messageModel);
        //#endregion
        //#endregion

    }
}

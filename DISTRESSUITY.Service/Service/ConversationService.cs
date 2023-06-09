using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.Common.Utility;
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

namespace DISTRESSUITY.Service.Service
{
    public class ConversationService : AbstractService, IConversationService
    {
        protected IUnitofWork _unitOfWork;

        int initialPageSize = Convert.ToInt32(ReadConfiguration.InitialPageSize);
        #region Constructors
        public ConversationService(IConversationRepository conversationRepository, IUnitofWork unitOfWork) :
            base(conversationRepository)
        {
            this._unitOfWork = unitOfWork;
        }
        public ConversationService()
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
        #endregion

        #region Get
        public Task<AuthenticationServiceResponse> GetNewMessagesCount() {
            try
            {
                var userId = GetLoginUserId();
                var conversations = _conversationService.conversationRepository.Get(x => (x.CreatedBy == userId || x.To == userId) && x.IsDeleted == false)
                    .OrderByDescending(x => x.UpdatedDate != null ? x.UpdatedDate : x.CreatedDate).ToList();
                int newMessagesCount = 0;
                for (int i = 0; i < conversations.Count; i++)
                {
                    for (int j = 0; j < conversations[i].ConversationReply.Count; j++)
                    {
                        if (conversations[i].ConversationReply.ToList()[j].Read == false && conversations[i].ConversationReply.ToList()[j].CreatedBy != userId)
                            newMessagesCount += 1;
                    }
                }
                return Task.FromResult(new AuthenticationServiceResponse { Data = newMessagesCount, Success = true });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<AuthenticationServiceResponse> GetConversations()
        {
            try
            {
                var userId = GetLoginUserId();
                var conversationsModel = new List<ConversationModel>();
                var ConversationReplySize = Convert.ToInt32(ReadConfiguration.ConversationReplySize);
                var conversations = _conversationService.conversationRepository.Get(x => (x.CreatedBy == userId || x.To == userId) && x.IsDeleted == false)
                    .OrderByDescending(x => x.UpdatedDate != null ? x.UpdatedDate : x.CreatedDate).ToList();

                //var conversationReplyForFirstConversation = _conversationService.conversationRepository.GetPagedRecords(x => 
                //    x.ConversationId == conversations[0].ConversationId && x.IsDeleted == false, x => x.CreatedDate, 1, pageSize).Result;
                Mapper.Map(conversations, conversationsModel);
                for (int i = 0; i < conversations.Count; i++)
                {
                    int unreadMessages = 0;
                    Mapper.Map(conversations[i].ToUser, conversationsModel[i].ToUserModel);
                    Mapper.Map(conversations[i].CreatedByUser, conversationsModel[i].CreatedByUserModel);
                    Mapper.Map(conversations[i].Project, conversationsModel[i].ProjectModel);
                    conversationsModel[i].ProjectModel.FundingDuration = conversations[i].Project.FundingDuration.toDateString();
                    if (i == 0)
                    {
                        int conversationId = conversations[i].ConversationId;
                        unreadMessages = UpdateConversationReplyRead(userId, conversationId);
                    }
                    conversations[i].ConversationReply = conversations[i].ConversationReply.OrderBy(x => x.CreatedDate).ToList();

                    for (int j = 0; j < conversations[i].ConversationReply.Count; j++)
                    {
                        ConversationReplyModel conversationReplyModel = new ConversationReplyModel();
                        Mapper.Map(conversations[i].ConversationReply.ToList()[j], conversationReplyModel);
                        Mapper.Map(conversations[i].ConversationReply.ToList()[j].User, conversationReplyModel.UserModel);
                        conversationsModel[i].ConversationReplyModel.Add(conversationReplyModel);
                        if (i != 0)
                            if (conversations[i].ConversationReply.ToList()[j].Read == false && conversations[i].ConversationReply.ToList()[j].CreatedBy != userId)
                                unreadMessages += 1;
                    }
                    conversationsModel[i].UnreadMessages = unreadMessages;

                }
                List<ConversationModel> DistinctConversation = conversationsModel.GroupBy(x => x.ProjectID).Select(x => x.First()).ToList();

                return Task.FromResult(new AuthenticationServiceResponse { MultipleData = new List<object>() { conversationsModel, DistinctConversation }, Success = true });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int UpdateConversationReplyRead(int userId, int conversationId)
        {
            try
            {
                var conversationReplies = _conversationService.conversationReplyRepository.Get(x => x.Read == false && x.CreatedBy != userId && x.ConversationId == conversationId).ToList();
                if (conversationReplies.Count > 0)
                {
                    conversationReplies.ForEach(x => x.Read = true);
                    _conversationService.conversationReplyRepository.UpdateAll(conversationReplies);
                    _conversationService.Save();
                }
                return conversationReplies.Count;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<AuthenticationServiceResponse> ChangeConversation(string conversationId)
        {
            try
            {
                int userId = GetLoginUserId();
                UpdateConversationReplyRead(userId, conversationId.Decrypt());
                var conversationModel = new ConversationModel();
                var conversation = _conversationService.conversationRepository.GetById(conversationId.Decrypt());
                Mapper.Map(conversation, conversationModel);
                Mapper.Map(conversation.ToUser, conversationModel.ToUserModel);
                Mapper.Map(conversation.CreatedByUser, conversationModel.CreatedByUserModel);
                Mapper.Map(conversation.Project, conversationModel.ProjectModel);
                conversationModel.ProjectModel.FundingDuration = conversation.Project.FundingDuration.toDateString();
                conversation.ConversationReply = conversation.ConversationReply.OrderBy(x => x.CreatedDate).ToList();
                int unreadMessages = 0;
                for (int i = 0; i < conversation.ConversationReply.Count; i++)
                {
                    var conversationReplyModel = new ConversationReplyModel();
                    ApplicationUser User = (new AuthenticationService()).GetUserById(conversation.ConversationReply.ToList()[i].CreatedBy);
                    Mapper.Map(conversation.ConversationReply.ToList()[i], conversationReplyModel);
                    Mapper.Map(User, conversationReplyModel.UserModel);
                    conversationModel.ConversationReplyModel.Add(conversationReplyModel);
                    if (conversation.ConversationReply.ToList()[i].Read == false && conversation.ConversationReply.ToList()[i].CreatedBy != userId)
                        unreadMessages += 1;
                }
                conversationModel.UnreadMessages = unreadMessages;
                return Task.FromResult(new AuthenticationServiceResponse { Data = conversationModel, Success = true });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Data = ex, Success = false, Message = "Something went wrong" });
            }
        }
        public Task<AuthenticationServiceResponse> GetConversationsByProjectId(string projectId)
        {
            try
            {
                var userId = GetLoginUserId();
                int ProjectId = projectId.Decrypt();
                var conversationsModel = new List<ConversationModel>();
                var conversations = _conversationService.conversationRepository.Get(x => x.ProjectID == ProjectId && (x.CreatedBy == userId || x.To == userId) &&
                    x.IsDeleted == false).OrderByDescending(x => x.UpdatedDate != null ? x.UpdatedDate : x.CreatedDate).ToList();
                Mapper.Map(conversations, conversationsModel);
                for (int i = 0; i < conversations.Count; i++)
                {
                    Mapper.Map(conversations[i].ToUser, conversationsModel[i].ToUserModel);
                    Mapper.Map(conversations[i].CreatedByUser, conversationsModel[i].CreatedByUserModel);
                    Mapper.Map(conversations[i].Project, conversationsModel[i].ProjectModel);
                    conversationsModel[i].ProjectModel.FundingDuration = conversations[i].Project.FundingDuration.toDateString();
                    for (int j = 0; j < conversations[i].ConversationReply.Count; j++)
                    {
                        ConversationReplyModel conversationReplyModel = new ConversationReplyModel();
                        Mapper.Map(conversations[i].ConversationReply.ToList()[j], conversationReplyModel);
                        Mapper.Map(conversations[i].ConversationReply.ToList()[j].User, conversationReplyModel.UserModel);
                        conversationsModel[i].ConversationReplyModel.Add(conversationReplyModel);
                    }
                }
                return Task.FromResult(new AuthenticationServiceResponse { Data = conversationsModel, Success = true });

            }
            catch (Exception ex)
            {

                return Task.FromResult(new AuthenticationServiceResponse { Data = ex, Success = false, Message = "Something went wrong" });
            }

        }
        public Task<AuthenticationServiceResponse> GetPublicMessage(string projectId)
        {
            int id = projectId.Decrypt();
            var publicMessagesModel = new List<PublicMessageModel>();
            var publicMessages = _conversationService.publicMessageRepository.Get(x => x.ProjectId == id && x.IsDeleted == false).ToList();
            Mapper.Map(publicMessages, publicMessagesModel);
            for (int i = 0; i < publicMessages.Count; i++)
            {
                Mapper.Map(publicMessages[i].Project, publicMessagesModel[i].Project);
                Mapper.Map(publicMessages[i].User, publicMessagesModel[i].UserModel);
            }
            return Task.FromResult(new AuthenticationServiceResponse { Data = publicMessagesModel, Success = true });
        }
        #endregion

        #region Add Update
        //public Task<AuthenticationServiceResponse> AddConversation(ConversationModel conversationModel, string Reply)
        public Task<AuthenticationServiceResponse> AddConversation(ConversationModel conversationModel)
        {
            try
            {
                int CreatingUserId = GetLoginUserId();
                int ProjectId = conversationModel.ProjectID.Decrypt();

                Conversation conversation = new Conversation();
                ConversationReply conversationReply = new ConversationReply();
                //ConversationModel conversationModel = new ConversationModel();
                ConversationReplyModel conversationReplyModel = new ConversationReplyModel();

                // Get or Set new Conversation 
                var getConversation = _conversationService.conversationRepository.Get(x => x.ProjectID == ProjectId && x.CreatedBy == CreatingUserId
                    && x.IsDeleted == false).FirstOrDefault();
                //if (!getConversation.Any())
                if (getConversation == null)
                {
                    conversation.ProjectID = ProjectId;
                    conversation.CreatedBy = CreatingUserId;
                    conversation.To = _unitOfWork.projectRepository.GetById(ProjectId).OwnerId; //Project Owner
                    conversation.IsDeleted = false;
                    conversation.UpdatedBy = CreatingUserId;
                    conversation.CreatedDate = DateTime.UtcNow;
                    //conversation.UpdatedDate = DateTime.UtcNow;
                    _conversationService.conversationRepository.Insert(conversation);
                    _conversationService.Save();
                    conversation = _conversationService.conversationRepository.GetById(conversation.ConversationId);
                }
                else
                {
                    conversation = getConversation;
                    conversation.UpdatedDate = DateTime.UtcNow;
                    conversation.UpdatedBy = GetLoginUserId();
                    _conversationService.conversationRepository.Update(conversation);
                    _conversationService.Save();
                }

                // Insert Conversation Reply 
                conversationReply.Reply = conversationModel.Message;
                conversationReply.CreatedBy = CreatingUserId;
                conversationReply.ConversationId = conversation.ConversationId;
                conversationReply.IsDeleted = false;
                //conversationReply.CreatedDate = DateTime.UtcNow;
                _conversationService.conversationReplyRepository.Insert(conversationReply);
                _conversationService.Save();

                Mapper.Map(conversationReply, conversationReplyModel);

                //conversation.ConversationReply.Add(conversationReply);

                //Mapper.Map(conversation, conversationModel);
                ////Mapper.Map(conversa)

                //var user = _unitOfWork.applicationUserRepository.GetById(conversation.CreatedBy);
                //Mapper.Map(user, conversationModel.CreatedByUserModel);

                return Task.FromResult(new AuthenticationServiceResponse { Data = conversationReplyModel,  Success = true, Message = "Message sent succesfully" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Data = ex, Success = false, Message = "Something went Wrong" });
            }
            //return Task.FromResult(new AuthenticationServiceResponse { Message = "Comment Deleted Successfully", Success = true });
        }

        public Task<AuthenticationServiceResponse> AddConversation(string projectId, string userId, string reply)
        {
            try
            {
                int CreatingUserId = GetLoginUserId();
                int ProjectId = projectId.Decrypt();
                int UserId = userId.Decrypt();

                Conversation conversation = new Conversation();
                ConversationReply conversationReply = new ConversationReply();
                //ConversationModel conversationModel = new ConversationModel();
                ConversationReplyModel conversationReplyModel = new ConversationReplyModel();

                // Get or Set new Conversation 
                var getConversation = _conversationService.conversationRepository.Get(x => x.ProjectID == ProjectId && (x.CreatedBy == CreatingUserId || x.CreatedBy == UserId)
                    && (x.To == UserId || x.To == CreatingUserId) && x.IsDeleted == false).FirstOrDefault();
                //if (!getConversation.Any())
                if (getConversation == null)
                {
                    conversation.ProjectID = ProjectId;
                    conversation.CreatedBy = CreatingUserId;
                    conversation.To = UserId; //Specific user with tih we need to start new conversation 
                    conversation.IsDeleted = false;
                    conversation.UpdatedBy = CreatingUserId;
                    conversation.CreatedDate = DateTime.UtcNow;
                    //conversation.UpdatedDate = DateTime.UtcNow;
                    _conversationService.conversationRepository.Insert(conversation);
                    _conversationService.Save();
                    conversation = _conversationService.conversationRepository.GetById(conversation.ConversationId);
                }
                else
                {
                    conversation = getConversation;
                    conversation.UpdatedDate = DateTime.UtcNow;
                    conversation.UpdatedBy = GetLoginUserId();
                    _conversationService.conversationRepository.Update(conversation);
                    _conversationService.Save();
                }

                // Insert Conversation Reply 
                conversationReply.Reply = reply;
                conversationReply.CreatedBy = CreatingUserId;
                conversationReply.ConversationId = conversation.ConversationId;
                conversationReply.IsDeleted = false;
                //conversationReply.CreatedDate = DateTime.UtcNow;
                _conversationService.conversationReplyRepository.Insert(conversationReply);
                _conversationService.Save();

                Mapper.Map(conversationReply, conversationReplyModel);

                //conversation.ConversationReply.Add(conversationReply);

                //Mapper.Map(conversation, conversationModel);
                ////Mapper.Map(conversa)

                //var user = _unitOfWork.applicationUserRepository.GetById(conversation.CreatedBy);
                //Mapper.Map(user, conversationModel.CreatedByUserModel);

                return Task.FromResult(new AuthenticationServiceResponse { Data = conversationReplyModel, Success = true, Message = "Message sent succesfully" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Data = ex, Success = false, Message = "Something went Wrong" });
            }
        }
        

        public Task<AuthenticationServiceResponse> AddConversationReply(ConversationReplyModel conversationReplyModel)
        {
            try
            {
                var conversationReply = new ConversationReply();
                Mapper.Map(conversationReplyModel, conversationReply);
                conversationReply.CreatedBy = GetLoginUserId();
                //conversationReply.CreatedDate = DateTime.UtcNow;
                conversationReply.Read = false;
                _conversationService.conversationReplyRepository.Insert(conversationReply);
                _conversationService.Save();

                var conversation = _conversationService.conversationRepository.GetById(conversationReply.ConversationId);
                conversation.UpdatedDate = DateTime.UtcNow;
                conversation.UpdatedBy = GetLoginUserId();
                _conversationService.conversationRepository.Update(conversation);
                _conversationService.Save();

                Mapper.Map(conversationReply, conversationReplyModel);
                ApplicationUser User = (new AuthenticationService()).GetUserById(conversationReply.CreatedBy);
                Mapper.Map(User, conversationReplyModel.UserModel);
                return Task.FromResult(new AuthenticationServiceResponse { Data = conversationReplyModel, Success = true });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new AuthenticationServiceResponse { Data = ex, Success = false, Message = "Something went wrong" });
            }

        }


        public Task<AuthenticationServiceResponse> AddPublicMessage(PublicMessageModel publicMessageModel)
        {
            try
            {
                int userid = GetLoginUserId();
                PublicMessage publicMessage = new PublicMessage();
                Mapper.Map(publicMessageModel, publicMessage);
                publicMessage.CreatedBy = userid;
                publicMessage.CreatedDate = DateTime.UtcNow;
                publicMessage.UpdatedBy = userid;
                publicMessage.UpdatedDate = DateTime.UtcNow;
                publicMessage.IsDeleted = false;
                _conversationService.publicMessageRepository.Insert(publicMessage);
                _conversationService.Save();
                publicMessage = _conversationService.publicMessageRepository.Get(x => x.MessageId == publicMessage.MessageId).FirstOrDefault();
                Mapper.Map(publicMessage, publicMessageModel);
                var User = (new AuthenticationService()).GetUserById(userid);
                Mapper.Map(User, publicMessageModel.UserModel);
                return Task.FromResult(new AuthenticationServiceResponse { Data = publicMessageModel, Success = true });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Delete
        public Task<AuthenticationServiceResponse> DeleteConversation(ConversationModel conversationModel)
        {
            //int id = conversationModel.MessageId.Decrypt();
            //var publicMessage = _conversationService.publicMessageRepository.Get(x => x.MessageId == id).FirstOrDefault();
            //publicMessage.IsDeleted = true;
            //_conversationService.publicMessageRepository.Update(publicMessage);
            return Task.FromResult(new AuthenticationServiceResponse { Message = "Message Deleted Successfully", Success = true });
        }
        public Task<AuthenticationServiceResponse> DeletePublicMessage(PublicMessageModel publicMessageModel)
        {
            int id = publicMessageModel.MessageId.Decrypt();
            var publicMessage = _conversationService.publicMessageRepository.Get(x => x.MessageId == id).FirstOrDefault();
            publicMessage.IsDeleted = true;
            _conversationService.publicMessageRepository.Update(publicMessage);
            _conversationService.Save();
            return Task.FromResult(new AuthenticationServiceResponse { Success = true, Message = "Message Deleted Successfully" });
        }
        #endregion
    }
}

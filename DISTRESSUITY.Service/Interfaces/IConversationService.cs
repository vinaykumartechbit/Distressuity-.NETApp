using DISTRESSUITY.Common.Service_Response;
using DISTRESSUITY.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.Service.Interfaces
{
    public interface IConversationService
    {
        #region Conversation
        #region Get
        Task<AuthenticationServiceResponse> GetNewMessagesCount();
        Task<AuthenticationServiceResponse> GetConversations();
        Task<AuthenticationServiceResponse> ChangeConversation(string conversationId);
        Task<AuthenticationServiceResponse> GetConversationsByProjectId(string projectId);
        Task<AuthenticationServiceResponse> GetPublicMessage(string projectId);

        #endregion

        #region Add Update
        //Task<AuthenticationServiceResponse> AddConversation(ConversationModel conversationModel, string Reply);
        Task<AuthenticationServiceResponse> AddConversation(ConversationModel conversationModel);
        Task<AuthenticationServiceResponse> AddConversation(string projectId, string userId, string reply);

        Task<AuthenticationServiceResponse> AddConversationReply(ConversationReplyModel conversationReplyModel);
        Task<AuthenticationServiceResponse> AddPublicMessage(PublicMessageModel publicMessageModel);
        #endregion

        #region Delete
        Task<AuthenticationServiceResponse> DeleteConversation(ConversationModel conversationModel);
        Task<AuthenticationServiceResponse> DeletePublicMessage(PublicMessageModel publicMessageModel);
        #endregion
        #endregion

    }
}

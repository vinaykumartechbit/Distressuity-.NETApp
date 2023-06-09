using DISTRESSUITY.Service.Interfaces;
using DISTRESSUITY.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DISTRESSUITY.Web.Controllers
{
    [RoutePrefix("api/Conversation")]
    public class ConversationController : ApiController
    {
        public IConversationService _conversationService;
        public ConversationController()
        {

        }
        public ConversationController(IConversationService ConversationService)
        {
            this._conversationService = ConversationService;
        }


        #region Conversation

        #region GetMethods
        [HttpGet]
        [Route("GetNewMessagesCount")]
        public async Task<IHttpActionResult> GetNewMessagesCount()
        {
            var result = await _conversationService.GetNewMessagesCount();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetConversations")]
        public async Task<IHttpActionResult> GetConversations()
        {
            var result = await _conversationService.GetConversations();
            return Ok(result);
        }
        [HttpGet]
        [Route("ChangeConversation/{conversationId}")]
        public async Task<IHttpActionResult> ChangeConversation(string conversationId)
        {
            var result = await _conversationService.ChangeConversation(conversationId);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetConversationsByProjectId/{projectId}")]
        public async Task<IHttpActionResult> GetConversationsByProjectId(string projectId)
        {
            var result = await _conversationService.GetConversationsByProjectId(projectId);
            return Ok(result);
        }
        #endregion

        #region AddUpdateMethods
        [HttpPost]
        [Route("AddConversation")]
        public async Task<IHttpActionResult> AddConversation(ConversationModel conversationModel)
        {
            var result = await _conversationService.AddConversation(conversationModel);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddConversationWithSpecificUser/{projectId}/{userId}/{reply}")]
        public async Task<IHttpActionResult> AddConversationWithSpecificUser(string projectId, string userId, string reply)
        {
            var result = await _conversationService.AddConversation(projectId, userId, reply);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddConversationReply")]
        public async Task<IHttpActionResult> AddConversationReply(ConversationReplyModel conversationReplyModel)
        {
            var result = await _conversationService.AddConversationReply(conversationReplyModel);
            return Ok(result);
        }
        #endregion

        #region DeleteMethods
        [HttpPost]
        [Route("DeleteConversation")]
        public async Task<IHttpActionResult> DeleteConversation(ConversationModel ConversationModel)
        {
            var result = await _conversationService.DeleteConversation(ConversationModel);
            return Ok(result);
        }
        #endregion

        #endregion

        #region PublicMessage

        #region GetMethods
        [HttpGet]
        [Route("GetPublicMessage")]
        public async Task<IHttpActionResult> GetPublicMessage(string projectId)
        {
            var result = await _conversationService.GetPublicMessage(projectId);
            return Ok(result);
        }
        #endregion

        #region AddUpdateMethods
        [HttpPost]
        [Route("AddPublicMessage")]
        public async Task<IHttpActionResult> AddPublicMessage(PublicMessageModel publicMessageModel)
        {
            var result = await _conversationService.AddPublicMessage(publicMessageModel);
            return Ok(result);
        }
        #endregion

        #region DeleteMethods
        [HttpPost]
        [Route("DeletePublicMessage")]
        public async Task<IHttpActionResult> DeletePublicMessage(PublicMessageModel publicMessageModel)
        {
            var result = await _conversationService.DeletePublicMessage(publicMessageModel);
            return Ok(result);
        }
        #endregion

        #endregion
    }
}

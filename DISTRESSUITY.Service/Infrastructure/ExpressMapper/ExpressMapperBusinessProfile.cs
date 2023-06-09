using ExpressMapper;
using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DISTRESSUITY.Service.Model;
using DISTRESSUITY.Common.Utility;

namespace DISTRESSUITY.Service.ExpressMapper
{
    public static class ExpressMapperServiceProfile
    {
        public static void RegisterMapping()
        {
            #region EntityToModel

            Mapper.Register<Industry, IndustryModel>()
                .Member(dest => dest.IndustryId, src => src.IndustryId.Encrypt());

            Mapper.Register<ProjectFunding, ProjectFundingModel>()
                .Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt())
                .Member(dest => dest.ProjectFundingId, src => src.ProjectFundingId.Encrypt())
                .Member(dest => dest.TotalAmount, src => src.FundingAmount + src.ProcessingFees)
                .Member(dest => dest.InvestedBy,src=>src.User.FirstName+" "+src.User.LastName)
                 .Member(dest => dest.ProjectTitle, src => src.Project.ProjectTitle)
                  .Member(dest => dest.ProjectFundingDuration, src => src.Project.FundingDuration.toDateString());

            Mapper.Register<ApplicationUser, UserModel>().Member(dest => dest.Id, src => src.Id.Encrypt())
                .Member(dest => dest.IndustryId, src => Convert.ToInt32(src.IndustryId).Encrypt())
                .Ignore(dest => dest.UserPositions);

            Mapper.Register<UserPosition, UserPositionsModel>();

            Mapper.Register<ProjectDocument, ProjectDocumentModel>().Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt());

            Mapper.Register<Project, ProjectModel>().Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt())
                .Member(dest => dest.IndustryId, src => src.IndustryId.Encrypt())
                .Member(dest => dest.FundingDuration, src => src.FundingDuration.toDateString())
                .Member(dest => dest.SiteCommission, src => Math.Round(src.TotalFundingAmount * ReadConfiguration.SiteCommission,2))
                .Member(dest => dest.UserName,src=>src.User.FirstName+" "+src.User.LastName)
                .Member(dest => dest.UserImage, src => src.User.PictureUrl)
                .Ignore(dest => dest.ProjectFundings)
                .Ignore(dest => dest.User)
                .Ignore(dest => dest.Status)
                .Ignore(dest => dest.ProjectDocuments)
                //.Ignore(dest => dest.FundingDuration)
                .Ignore(dest => dest.FinancialBreakdowns)
                .Ignore(dest => dest.PublicMessages);
            //.Member(dest => dest.FundingDuration, src => src.FundingDuration.toDateString());

            //Mapper.Register<Message, MessageModel>()
            //    .Member(dest => dest.MessageId, src => src.MessageId.Encrypt())
            //    .Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt())
            //    .Member(dest => dest.CreatedBy, src => src.CreatedBy.Encrypt());

            Mapper.Register<Conversation, ConversationModel>()
                .Member(dest => dest.ConversationId, src => src.ConversationId.Encrypt())
                .Member(dest => dest.ProjectID, src => src.ProjectID.Encrypt())
                .Member(dest => dest.CreatedBy, src => src.CreatedBy.Encrypt())
                .Member(dest => dest.To, src => src.To.Encrypt());

            Mapper.Register<ConversationReply, ConversationReplyModel>()
               .Member(dest => dest.ConversationId, src => src.ConversationId.Encrypt())
               .Member(dest => dest.ConversationReplyId, src => src.ConversationReplyId.Encrypt());

            Mapper.Register<PublicMessage, PublicMessageModel>()
             .Member(dest => dest.MessageId, src => src.MessageId.Encrypt())
             .Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt())
             .Member(dest => dest.CreatedBy, src => src.CreatedBy.Encrypt());

            Mapper.Register<FeaturedIdea, FeaturedIdeaModel>()
                .Member(dest => dest.FeaturedIdeaId, src => src.FeaturedIdeaId.Encrypt())
                .Member(dest => dest.ProjectId, src => src.ProjectId.Encrypt());
            Mapper.Register<AdLog, AdLogModel>()
                .Member(dest => dest.AdLogsId, src => src.AdLogsId.Encrypt())
                .Member(dest => dest.FeatureIdeaId, src => src.FeatureIdeaId.Encrypt());
            
            Mapper.Register<Status, StatusModel>()
                .Member(dest => dest.StatusId, src => src.StatusId.Encrypt());
            #endregion



            #region ModelToEntity
            Mapper.Register<IndustryModel,Industry>()
                .Member(dest => dest.IndustryId, src => src.IndustryId.Decrypt());

            Mapper.Register<ProjectFundingModel, ProjectFunding>()
                .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt())
                .Member(dest => dest.ProjectFundingId, src => src.ProjectFundingId.Decrypt());

            Mapper.Register<UserModel, ApplicationUser>()
               .Member(dest => dest.UserName, src => src.Email)
               .Member(dest => dest.Id, src => src.Id.Decrypt())
               .Member(dest => dest.IndustryId, src => src.IndustryId.Decrypt())
               .Ignore(dest => dest.UserPositions);

            Mapper.Register<ProjectModel, Project>()
                .Ignore(dest => dest.Industry)
                .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt())
                .Member(dest => dest.IndustryId, src => src.IndustryId.Decrypt())
                //.Member(dest => dest.ProjectFundings, src => src.FundingDuration.toDateTime())
                .Ignore(dest => dest.User)
                .Ignore(dest => dest.Status)
                .Ignore(dest => dest.ProjectDocuments)
                .Ignore(dest => dest.FundingDuration)
                .Ignore(dest => dest.PublicMessages);

            Mapper.Register<ProjectDocumentModel, ProjectDocument>()
                .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt());
            //.Member(dest => dest.FundingDuration, src => src.FundingDuration.toDateTime());

            //Mapper.Register<MessageModel, Message>()
            //    .Member(dest => dest.MessageId, src => src.MessageId.Decrypt())
            //    .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt())
            //    .Member(dest => dest.CreatedBy, src => src.CreatedBy.Decrypt());

            Mapper.Register<ConversationModel, Conversation>()
               .Member(dest => dest.ConversationId, src => src.ConversationId.Decrypt())
               .Member(dest => dest.ProjectID, src => src.ProjectID.Decrypt())
               .Ignore(dest => dest.ToUser)
               .Ignore(dest => dest.CreatedByUser);
            //.Member(dest => dest.User2, src => src.User2.Decrypt());

            Mapper.Register<ConversationReplyModel, ConversationReply>()
              .Member(dest => dest.ConversationId, src => src.ConversationId.Decrypt())
              .Member(dest => dest.ConversationReplyId, src => src.ConversationReplyId.Decrypt());

            Mapper.Register<PublicMessageModel, PublicMessage>()
             .Member(dest => dest.MessageId, src => src.MessageId.Decrypt())
             .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt())
             .Member(dest => dest.CreatedBy, src => src.CreatedBy.Decrypt())
             .Ignore(dest => dest.Project);

            Mapper.Register<FeaturedIdeaModel, FeaturedIdea>()
               .Member(dest => dest.FeaturedIdeaId, src => src.FeaturedIdeaId.Decrypt())
               .Member(dest => dest.ProjectId, src => src.ProjectId.Decrypt());
            Mapper.Register<AdLogModel, AdLog>()
                .Member(dest => dest.AdLogsId, src => src.AdLogsId.Decrypt())
                .Member(dest => dest.FeatureIdeaId, src => src.FeatureIdeaId.Decrypt());
            Mapper.Register<StatusModel, Status>()
                .Member(dest => dest.StatusId, src => src.StatusId.Decrypt());
            #endregion
        }
    }
}


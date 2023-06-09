using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Interfaces
{
    public interface IConversationRepository : IDisposable
    {
        DISTRESSUITYContext GetContext();
        void Save();

        IRepository<Conversation> conversationRepository { get; }
        IRepository<ConversationReply> conversationReplyRepository { get; }
        IRepository<PublicMessage> publicMessageRepository { get; }

    }
}

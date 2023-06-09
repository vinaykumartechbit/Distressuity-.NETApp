using DISTRESSUITY.DAL;
using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private DISTRESSUITYContext _context;
        private bool _isDisposed;

        public ConversationRepository(DISTRESSUITYContext context)
        {
            this._context = context;
        }
        public DISTRESSUITYContext GetContext()
        {
            return _context;
        }
        public void Dispose()
        {
            if (_context != null && !_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public IRepository<Conversation> conversationRepository
        {
            get { return new BaseRepository<Conversation>(GetContext()); }
        }
        public IRepository<ConversationReply> conversationReplyRepository
        {
            get { return new BaseRepository<ConversationReply>(GetContext()); }
        }
        public IRepository<PublicMessage> publicMessageRepository
        {
            get { return new BaseRepository<PublicMessage>(GetContext()); }
        }
    }
}

using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Repository
{
    public class MessageService : IMessageService
    {
         private DISTRESSUITYContext _context;
        private bool _isDisposed;

        public MessageService(DISTRESSUITYContext context)
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
        public IRepository<Message> MessageRepository
        {
            get { return new BaseRepository<Message>(GetContext()); }
        }


    }
}

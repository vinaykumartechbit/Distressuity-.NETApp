using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Interfaces
{
    public interface IMessageService : IDisposable
    {
        DISTRESSUITYContext GetContext();
        void Save();
        IRepository<Message> MessageRepository { get; }
    }
}

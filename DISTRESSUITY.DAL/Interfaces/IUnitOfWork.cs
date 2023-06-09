using DISTRESSUITY.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Interfaces
{
    public interface IUnitofWork : IDisposable
    {
        DISTRESSUITYContext GetContext();
        void Save();

        //   IRepository<Role> RoleRepository { get; }
        IRepository<UserPosition> userPositionsRepository { get; }
        IRepository<ApplicationUser> applicationUserRepository { get; }
        IRepository<Project> projectRepository { get; }
        IRepository<Industry> industryRepository { get; }
        IRepository<ProjectDocument> projectDocumentRepository { get; }
        IRepository<FinancialBreakdown> financialBreakdownRepository { get; }
        IRepository<ProjectFunding> projectFundingRepository { get; }
        IRepository<CardDetail> cardDetailRepository { get; }
        IRepository<User_Project_CardDetail> user_Project_CardDetailRepository { get; }
        IRepository<FeaturedIdea> featuredIdeaRepository { get; }
        IRepository<AdLog> adLogRepository { get; }
        IRepository<Transaction> transactionRepository { get; }
        IRepository<Status> statusRepository { get; }
        //IRepository<Message> MessageRepository { get; }
        
    }
}

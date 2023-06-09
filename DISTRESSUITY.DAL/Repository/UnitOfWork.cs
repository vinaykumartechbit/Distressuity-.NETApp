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
    public class UnitOfWork : IUnitofWork
    {
        private DISTRESSUITYContext _context;
        private bool _isDisposed;

        public UnitOfWork(DISTRESSUITYContext context)
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

        public IRepository<UserPosition> userPositionsRepository
        {
            get { return new BaseRepository<UserPosition>(GetContext()); }
        }

        public IRepository<ApplicationUser> applicationUserRepository
        {
            get { return new BaseRepository<ApplicationUser>(GetContext()); }
        }

        public IRepository<Project> projectRepository
        {
            get { return new BaseRepository<Project>(GetContext()); }
        }

        public IRepository<Industry> industryRepository
        {
            get { return new BaseRepository<Industry>(GetContext()); }
        }
        public IRepository<ProjectDocument> projectDocumentRepository
        {
            get { return new BaseRepository<ProjectDocument>(GetContext()); }
        }
        public IRepository<FinancialBreakdown> financialBreakdownRepository
        {
            get { return new BaseRepository<FinancialBreakdown>(GetContext()); }
        }
        public IRepository<ProjectFunding> projectFundingRepository
        {
            get { return new BaseRepository<ProjectFunding>(GetContext()); }
        }
        public IRepository<CardDetail> cardDetailRepository
        {
            get { return new BaseRepository<CardDetail>(GetContext()); }
        }
        public IRepository<User_Project_CardDetail> user_Project_CardDetailRepository
        {
            get { return new BaseRepository<User_Project_CardDetail>(GetContext()); }
        }
        public IRepository<FeaturedIdea> featuredIdeaRepository
        {
            get { return new BaseRepository<FeaturedIdea>(GetContext()); }
        }
        public IRepository<AdLog> adLogRepository
        {
            get { return new BaseRepository<AdLog>(GetContext()); }
        }
        public IRepository<Transaction> transactionRepository
        {
            get { return new BaseRepository<Transaction>(GetContext()); }
        }
        public IRepository<Status> statusRepository
        {
            get { return new BaseRepository<Status>(GetContext()); }
        }
    }
}

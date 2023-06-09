using DISTRESSUITY.DAL.Entities;
using DISTRESSUITY.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DISTRESSUITY.DAL.Repository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal DISTRESSUITYContext context;
        internal DbSet<TEntity> dbSet;

        public BaseRepository(DISTRESSUITYContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ignoreLanguage = false)
        {
            var query = GetQuery(filter, includeProperties, ignoreLanguage);

            if (orderBy != null)
                return orderBy(query).ToList();

            return query.ToList();
        }

        public virtual List<TEntity> Get(int limit, int skip, Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ignoreLanguage = false)
        {
            var query = GetQuery(filter, includeProperties, ignoreLanguage);

            if (orderBy != null)
                return orderBy(query).Skip(skip).Take(limit).ToList();

            return query.Skip(skip).Take(limit).ToList();
        }

        public virtual List<TResult> GetAs<TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "") where TResult : class
        {
            var query = GetQuery(filter, includeProperties);
            return query.Select(select).ToList();
        }

        public virtual Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {

            var query = GetQuery(filter, includeProperties);

            if (orderBy != null)
                return orderBy(query).ToListAsync();

            return query.ToListAsync();
        }

        public virtual Task<List<TEntity>> GetAsync(int limit, int skip, Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = ""
          )
        {

            var query = GetQuery(filter, includeProperties);

            if (orderBy != null)
                return orderBy(query).Skip(skip).Take(limit).ToListAsync();

            return query.Skip(skip).Take(limit).ToListAsync();
        }

        public int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter);
            var count = query.Count();
            return count;
        }
        private IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "", bool ignoreLanguage = false)
        {
            IQueryable<TEntity> query = dbSet;

            Type[] types = typeof(TEntity).GetInterfaces();

            //here we re filter deleted item
            if (types.Contains(typeof(IFlagRemove)))
            {
                query = (query as IQueryable<IFlagRemove>).Where(e => !e.IsDeleted).Cast<TEntity>();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
        public virtual TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }
        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            return dbSet.FirstOrDefault(filter);
        }
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        public virtual void InsertAll(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                dbSet.Add(entity);
            }
        }
        public virtual void DeleteAll(List<TEntity> entities)
        {
            foreach (var entityToDelete in entities)
            {
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                if (entityToDelete is IFlagRemove)
                {
                    ((IFlagRemove)entityToDelete).IsDeleted = true;
                    Update(entityToDelete);
                }
                else
                {
                    dbSet.Remove(entityToDelete);
                }
            }
        }
        public virtual void UpdateAll(List<TEntity> entities)
        {
            foreach (var entityToUpdate in entities)
            {
                if (context.Entry(entityToUpdate).State == EntityState.Detached)
                    dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete)
        {

            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            if (entityToDelete is IFlagRemove)
            {
                ((IFlagRemove)entityToDelete).IsDeleted = true;
                Update(entityToDelete);
            }
            else
            {
                dbSet.Remove(entityToDelete);
            }

        }
        public virtual void Update(TEntity entityToUpdate)
        {
            if (context.Entry(entityToUpdate).State == EntityState.Detached)
                dbSet.Attach(entityToUpdate);

            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return dbSet.Any(filter);
        }


        public IEnumerable<TEntity> GetPagedRecords(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, string>> orderBy, Expression<Func<TEntity, string>> thenBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderBy(orderBy).ThenBy(thenBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }
        public IEnumerable<TEntity> GetPagedRecords(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, string>> orderBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderBy(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }
        public IEnumerable<TEntity> GetPagedRecords(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, int>> orderBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderBy(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }
        public IEnumerable<TEntity> GetPagedRecords(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, long>> orderBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderBy(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }
        public IEnumerable<TEntity> GetPagedRecords(Expression<Func<TEntity, bool>> whereCondition, Expression<Func<TEntity, DateTime?>> orderBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderByDescending(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }
    }
}

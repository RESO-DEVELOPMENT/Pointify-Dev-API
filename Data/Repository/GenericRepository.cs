using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal PromotionEngineContext _context;
        internal DbSet<TEntity> _dbSet;
        internal Expression<Func<TEntity, bool>> delflgCond;
        public GenericRepository(PromotionEngineContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentException("entity");

            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            var entry = _context.Entry(entity);
            Type type = typeof(TEntity);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {

                if (property.GetValue(entity, null) == null)
                {
                    try
                    {
                        entry.Property(property.Name).IsModified = false;
                    }
                    catch (InvalidOperationException)
                    {
                        entry.Reference(property.Name).IsModified = false;
                    }

                }
            }
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        public void Delete(Guid id, Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;


            if (filter == null)
            {
                TEntity entity = _dbSet.Find(id);
                if (entity != null)
                {

                    _dbSet.Attach(entity);
                    _dbSet.Remove(entity);
                }
            }
            else
            {
                query = query.Where(filter);
                _dbSet.RemoveRange(query);
            }
        }

        public void DeleteUsername(string username)
        {
            TEntity entity = _dbSet.Find(username);

            if (entity != null)
            {
                _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> Get(int pageIndex = 0, int pageSize = 0, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return pageIndex != 0 && pageSize != 0
                ? await PaginatedList<TEntity>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize)
                : await query.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.AsNoTracking().FirstOrDefaultAsync();
        }

        public void Hide(Guid id, string value)
        {
            TEntity entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).Property("DelFlg").CurrentValue = value;
                _context.Entry(entity).Property("UpdDate").CurrentValue = DateTime.Now;
            }

        }

        public void HideUsername(string username, string value)
        {
            TEntity entity = _dbSet.Find(username);
            if (entity != null)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).Property("DelFlg").CurrentValue = value;
                _context.Entry(entity).Property("UpdDate").CurrentValue = DateTime.Now;
            }
        }




    }
}

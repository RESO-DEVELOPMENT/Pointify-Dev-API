using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> Get(int pageIndex = 0, int pageSize = 0, Expression<Func<TEntity, bool>> filter = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                 string includeProperties = "");
        Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        Task<TEntity> GetById(Guid id);
        void Add(TEntity entity);
        void Delete(Guid id, Expression<Func<TEntity, bool>> filter = null);
        void Update(TEntity entity);
        void Hide(Guid id, string value);
        void HideDeFlg(Guid id, bool value);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);
        void DeleteUsername(string username);
        void HideUsername(string username, string value);

    }
}

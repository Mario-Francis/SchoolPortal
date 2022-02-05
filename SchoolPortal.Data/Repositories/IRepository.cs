using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Data.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        string TableName { get; }
        IEnumerable<T> GetAll();
        IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression);
        Task<T> GetById(long id);
        Task<T> GetSingleWhere(Expression<Func<T, bool>> expression);
        Task Insert(T entity, bool save = true);
        Task InsertRange(IEnumerable<T> entityList, bool save = true);
        Task InsertBulk(IEnumerable<T> entityList);
        Task InsertOrUpdateBulk(IEnumerable<T> entityList);
        Task Update<T1>(T1 entity, bool save = true) where T1 : BaseEntity, IUpdatable;
        Task UpdateRange<T1>(IEnumerable<T1> entityList, bool save = true) where T1 : BaseEntity, IUpdatable;
        Task UpdateBulk<T1>(IEnumerable<T1> entityList) where T1 : BaseEntity, IUpdatable;
        Task Delete(long id, bool save = true);
        Task DeleteRange(IEnumerable<long> ids, bool save = true);
        Task DeleteWhere(Expression<Func<T, bool>> expression, bool save = true);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        bool Any(Expression<Func<T, bool>> expression);
        Task<int> Count();
        Task<int> CountWhere(Expression<Func<T, bool>> expression);
        Task Save();
    }
}

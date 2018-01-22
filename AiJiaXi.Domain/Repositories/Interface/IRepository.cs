using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiJiaXi.Domain.Repositories.Interface
{
    public interface IRepository<T> where T : class 
    {
        T Add(T entity);

        Task<T> AddAsync(T entity);

        int Add(IEnumerable<T> entities);

        Task<int> AddAsync(IEnumerable<T> entities);

        int AddOrUpdate(Expression<Func<T, object>> predicate, params T[] entity);

        long Count(Expression<Func<T, bool>> predicate);

        Task<long> CountAsync(Expression<Func<T, bool>> predicate);

        bool Update(T entity);

        Task<bool> UpdateAsync(T entity);

        int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression);

        bool Delete(T entity);

        Task<bool> DeleteAsync(T entity);

        int Delete(Expression<Func<T, bool>> delExpression);

        Task<int> DeleteAsync(Expression<Func<T, bool>> delExpression);

        bool Exist(Expression<Func<T, bool>> anyLambda);

        Task<bool> ExistAsync(Expression<Func<T, bool>> anyLambda);

        T Find(Expression<Func<T, bool>> whereLambda);

        Task<T> FindAsync(Expression<Func<T, bool>> whereLambda);

        IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba);

        IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba, string orderName, bool isAsc);

        IEnumerable<T> FindPageList(
            int pageIndex,
            int pageSize,
            out int totalRecord,
            Expression<Func<T, bool>> whereLamdba,
            string orderName,
            bool isAsc);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Domain.Repositories.Interface;

namespace Project.Domain.Repositories.Impl
{
    public class MysqlRepository<T> : IRepository<T> where T : class
    {
        public T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public int Add(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public int AddOrUpdate(Expression<Func<T, object>> predicate, params T[] entity)
        {
            throw new NotImplementedException();
        }

        public long Count(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(Expression<Func<T, bool>> delExpression)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Expression<Func<T, bool>> delExpression)
        {
            throw new NotImplementedException();
        }

        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistAsync(Expression<Func<T, bool>> anyLambda)
        {
            throw new NotImplementedException();
        }

        public T Find(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba, string orderName, bool isAsc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindPageList(int pageIndex, int pageSize, out int totalRecord, Expression<Func<T, bool>> whereLamdba, string orderName,
            bool isAsc)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Domain.Repositories.Interface;
using EntityFramework.Extensions;
using ZhiYuan.IAR.Repository.EF;

namespace Project.Domain.Repositories.Impl
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext nContext = ContextFactory.GetCurrentContext();

        public T Add(T entity)
        {
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
            this.nContext.SaveChanges();
            return entity;
        }

        public int Add(IEnumerable<T> entities)
        {
            var list = nContext.Set<T>().AddRange(entities);
            int result = this.nContext.SaveChanges();

            return result;
        }

        public async Task<T> AddAsync(T entity)
        {
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
            await this.nContext.SaveChangesAsync();
            return entity;
        }

        public async Task<int> AddAsync(IEnumerable<T> entities)
        {
            var list = nContext.Set<T>().AddRange(entities);
            int result = await this.nContext.SaveChangesAsync();

            return result;
        }

        public int AddOrUpdate(Expression<Func<T, object>> predicate,params T[] entity)
        {
            this.nContext.Set<T>().AddOrUpdate(predicate,entity);
            int result = this.nContext.SaveChanges();

            return result;
        }

        public long Count(Expression<Func<T, bool>> predicate)
        {
            return this.nContext.Set<T>().LongCount(predicate);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await this.nContext.Set<T>().LongCountAsync(predicate);
        }

        public bool Update(T entity)
        {
            this.nContext.Set<T>().Attach(entity);
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            return this.nContext.SaveChanges() > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            this.nContext.Set<T>().Attach(entity);
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            return await this.nContext.SaveChangesAsync() > 0;
        }

        public int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression)
        {
            return nContext.Set<T>().Where(predicate).Update(updateExpression);
        }

        public bool Delete(T entity)
        {
            this.nContext.Set<T>().Attach(entity);
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            return this.nContext.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            this.nContext.Set<T>().Attach(entity);
            this.nContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            return await this.nContext.SaveChangesAsync() > 0;
        }

        public int Delete(Expression<Func<T, bool>> delExpression)
        {
            return this.FindList<T>(delExpression).Delete();
        }


        public async Task<int> DeleteAsync(Expression<Func<T, bool>> delExpression)
        {
            return await this.FindList<T>(delExpression).DeleteAsync();
        }

        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            return this.nContext.Set<T>().Any(anyLambda);
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> anyLambda)
        {
            return await this.nContext.Set<T>().AnyAsync(anyLambda);
        }

        public T Find(Expression<Func<T, bool>> whereLambda)
        {
            T _entity = this.nContext.Set<T>().FirstOrDefault<T>(whereLambda);
            return _entity;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> whereLambda)
        {
            T _entity = await this.nContext.Set<T>().FirstOrDefaultAsync<T>(whereLambda);
            return _entity;
        }

        public IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba)
        {
            return this.nContext.Set<T>().Where<T>(whereLamdba);
        }

        public IQueryable<T> FindList<S>(Expression<Func<T, bool>> whereLamdba, string orderName, bool isAsc)
        {
            var _list = this.nContext.Set<T>().Where<T>(whereLamdba);
            _list = this.OrderBy(_list, orderName, isAsc);
            return _list;
        }

        public IEnumerable<T> FindPageList(
            int pageIndex,
            int pageSize,
            out int totalRecord,
            Expression<Func<T, bool>> whereLamdba,
            string orderName,
            bool isAsc)
        {
            var _list = this.nContext.Set<T>().Where<T>(whereLamdba);
            var total = _list.FutureCount();
            var list = this.OrderBy(_list, orderName, isAsc).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).Future();
            totalRecord = total.Value;
            return list.ToList();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">原IQueryable</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns>排序后的IQueryable<T></returns>
        private IQueryable<T> OrderBy(IQueryable<T> source, string propertyName, bool isAsc)
        {
            if (source == null) throw new ArgumentNullException("source", "不能为空");
            if (string.IsNullOrEmpty(propertyName)) return source;
            var _parameter = Expression.Parameter(source.ElementType);
            var _property = Expression.Property(_parameter, propertyName);
            if (_property == null) throw new ArgumentNullException("propertyName", "属性不存在");
            var _lambda = Expression.Lambda(_property, _parameter);
            var _methodName = isAsc ? "OrderBy" : "OrderByDescending";
            var _resultExpression = Expression.Call(typeof(Queryable), _methodName, new Type[] { source.ElementType, _property.Type }, source.Expression, Expression.Quote(_lambda));
            return source.Provider.CreateQuery<T>(_resultExpression);
        }
    }
}
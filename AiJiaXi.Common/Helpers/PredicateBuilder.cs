using System;
using System.Linq;
using System.Linq.Expressions;

namespace Project.Common.Helpers
{
    /// <summary>
    /// Linq条件表达式拼接工具类.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 组装lamder表达式
        /// </summary>
        public static Expression<T> Compose<T>(
            this Expression<T> first,
            Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // 创建参数字典
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);


            // 替换lamder表达式
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);


            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        /// <summary>
        /// 建立一个初始值为TRUE的表达式.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        /// <summary>
        /// 创建返回值为false.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        } 
    }
}
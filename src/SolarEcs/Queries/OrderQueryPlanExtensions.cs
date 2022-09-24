using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Queries;

namespace SolarEcs
{
    public static partial class QueryPlan
    {
        /// <summary>
        /// Sorts the elements of this query in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TResult> OrderBy<TResult, TOrder>(this IQueryPlan<TResult> query, Expression<Func<IKeyWith<Guid, TResult>, TOrder>> keySelector)
        {
            return ((IQueryPlan<Guid, TResult>)query).OrderBy(keySelector).AsEntityQuery() as IOrderedQueryPlan<TResult>;
        }


        /// <summary>
        /// Sorts the elements of this query in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TKey, TResult> OrderBy<TKey, TResult, TOrder>(this IQueryPlan<TKey, TResult> query, Expression<Func<IKeyWith<TKey, TResult>, TOrder>> keySelector)
        {
            var cleanSelector = QueryExpressions.Clean(keySelector);
            return query.TransformWithKeyPreserved(qry => qry.OrderBy(cleanSelector)) as IOrderedQueryPlan<TKey, TResult>;
        }

        /// <summary>
        /// Sorts the elements of this query in descending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TResult> OrderByDescending<TResult, TOrder>(this IQueryPlan<TResult> query, Expression<Func<IKeyWith<Guid, TResult>, TOrder>> keySelector)
        {
            return ((IQueryPlan<Guid, TResult>)query).OrderByDescending(keySelector).AsEntityQuery() as IOrderedQueryPlan<TResult>;
        }

        /// <summary>
        /// Sorts the elements of this query in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TKey, TResult> OrderByDescending<TKey, TResult, TOrder>(this IQueryPlan<TKey, TResult> query, Expression<Func<IKeyWith<TKey, TResult>, TOrder>> keySelector)
        {
            var cleanSelector = QueryExpressions.Clean(keySelector);
            return query.TransformWithKeyPreserved(qry => qry.OrderByDescending(cleanSelector)) as IOrderedQueryPlan<TKey, TResult>;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements of this query in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TResult> ThenBy<TResult, TOrder>(this IOrderedQueryPlan<TResult> query, Expression<Func<IKeyWith<Guid, TResult>, TOrder>> keySelector)
        {
            return ((IOrderedQueryPlan<Guid, TResult>)query).ThenBy(keySelector).AsEntityQuery() as IOrderedQueryPlan<TResult>;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements of this query in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TKey, TResult> ThenBy<TKey, TResult, TOrder>(this IOrderedQueryPlan<TKey, TResult> query, Expression<Func<IKeyWith<TKey, TResult>, TOrder>> keySelector)
        {
            var cleanSelector = QueryExpressions.Clean(keySelector);
            return query.TransformWithKeyPreserved(qry => ((IOrderedQueryable<IKeyWith<TKey, TResult>>)qry).ThenBy(cleanSelector)) as IOrderedQueryPlan<TKey, TResult>;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements of this query in descending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TResult> ThenByDescending<TResult, TOrder>(this IOrderedQueryPlan<TResult> query, Expression<Func<IKeyWith<Guid, TResult>, TOrder>> keySelector)
        {
            return ((IOrderedQueryPlan<Guid, TResult>)query).ThenByDescending(keySelector).AsEntityQuery() as IOrderedQueryPlan<TResult>;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements of this query in descending order according to a key.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="keySelector">An expression to extract the ordinal key from each element of this query.</param>
        /// <returns></returns>
        public static IOrderedQueryPlan<TKey, TResult> ThenByDescending<TKey, TResult, TOrder>(this IOrderedQueryPlan<TKey, TResult> query, Expression<Func<IKeyWith<TKey, TResult>, TOrder>> keySelector)
        {
            var cleanSelector = QueryExpressions.Clean(keySelector);
            return query.TransformWithKeyPreserved(qry => ((IOrderedQueryable<IKeyWith<TKey, TResult>>)qry).ThenByDescending(cleanSelector)) as IOrderedQueryPlan<TKey, TResult>;
        }
    }
}

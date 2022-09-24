using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public static partial class QueryPlan
    {
        /// <summary>
        /// Filters this query based on a predicate.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="predicate">The filter to apply to this query.  The query will only return elements for which this predicate returns true.</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Where<TResult>(this IQueryPlan<TResult> query, Expression<Func<IKeyWith<Guid, TResult>, bool>> predicate)
        {
            return ((IQueryPlan<Guid, TResult>)query).Where(predicate).AsEntityQuery();
        }

        /// <summary>
        /// Filters this query based on a predicate.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="predicate">The filter to apply to this query.  The query will only return elements for which this predicate returns true.</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Where<TKey, TResult>(this IQueryPlan<TKey, TResult> query, Expression<Func<IKeyWith<TKey, TResult>, bool>> predicate)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            return new FilterQueryPlan<TKey, TResult>(query, predicate);
        }
    }
}

namespace SolarEcs.Queries
{
    public class FilterQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        public IQueryPlan<TKey, TResult> BaseQuery { get; private set; }
        public Expression<Func<IKeyWith<TKey, TResult>, bool>> FilterPredicate { get; private set; }

        public FilterQueryPlan(IQueryPlan<TKey, TResult> baseQuery, Expression<Func<IKeyWith<TKey, TResult>, bool>> filterPredicate)
        {
            this.BaseQuery = baseQuery;
            this.FilterPredicate = filterPredicate;
        }

        public QueryPlanState State
        {
            get { return BaseQuery.State; }
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (BaseQuery.State == QueryPlanState.Materialized)
            {
                var compiled = FilterPredicate.Compile();
                return BaseQuery.Execute(predicate).Where(compiled);
            }
            else
            {
                return ImmaterialQuery.Where(QueryExpressions.Clean(predicate).ForKeyWith<TKey, TResult>()).ToList();
            }
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get { return BaseQuery.ImmaterialQuery.Where(FilterPredicate); }
        }
    }
}

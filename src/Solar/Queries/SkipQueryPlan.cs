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
        /// Bypasses a specified number of elements from a sequence and then returns a query plan for the remaining elements.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Skip<TResult>(this IQueryPlan<TResult> query, int count)
        {
            return ((IQueryPlan<Guid, TResult>)query).Skip(count).AsEntityQuery();
        }

        /// <summary>
        /// Bypasses a specified number of elements from a sequence and then returns a query plan for the remaining elements.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Skip<TKey, TResult>(this IQueryPlan<TKey, TResult> query, int count)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            return new SkipQueryPlan<TKey, TResult>(query, count);
        }
    }
}

namespace SolarEcs.Queries
{
    public class SkipQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        public IQueryPlan<TKey, TResult> BaseQuery { get; }
        public int Count { get; }

        public SkipQueryPlan(IQueryPlan<TKey, TResult> baseQuery, int count)
        {
            this.BaseQuery = baseQuery;
            this.Count = count;
        }

        public QueryPlanState State => BaseQuery.State;
        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery => BaseQuery.ImmaterialQuery.Skip(Count);

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (BaseQuery.State == QueryPlanState.Materialized)
            {
                return BaseQuery.Execute(predicate).Skip(Count);
            }
            else
            {
                return BaseQuery.ImmaterialQuery
                    .Where(QueryExpressions.Clean(predicate).ForKeyWith<TKey, TResult>())
                    .Skip(Count)
                    .ToList();
            }
        }
    }
}


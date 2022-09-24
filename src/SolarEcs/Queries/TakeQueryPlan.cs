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
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Take<TResult>(this IQueryPlan<TResult> query, int count)
        {
            return ((IQueryPlan<Guid, TResult>)query).Take(count).AsEntityQuery();
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Take<TKey, TResult>(this IQueryPlan<TKey, TResult> query, int count)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            return new TakeQueryPlan<TKey, TResult>(query, count);
        }
    }
}

namespace SolarEcs.Queries
{
    public class TakeQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        public IQueryPlan<TKey, TResult> BaseQuery { get; }
        public int Count { get; }

        public TakeQueryPlan(IQueryPlan<TKey, TResult> baseQuery, int count)
        {
            this.BaseQuery = baseQuery;
            this.Count = count;
        }

        public QueryPlanState State => BaseQuery.State;
        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery => BaseQuery.ImmaterialQuery.Take(Count);

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (BaseQuery.State == QueryPlanState.Materialized)
            {
                return BaseQuery.Execute(predicate).Take(Count);
            }
            else
            {
                return BaseQuery.ImmaterialQuery
                    .Where(QueryExpressions.Clean(predicate).ForKeyWith<TKey, TResult>())
                    .Take(Count)
                    .ToList();
            }
        }
    }
}

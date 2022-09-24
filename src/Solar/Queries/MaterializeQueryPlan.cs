using Solar.Ecs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    public static partial class QueryPlan
    {
        /// <summary>
        /// Marks a point in this query plan where entities are guaranteed to be materialized in memory.
        /// Use this before performing actions which may not be supported by the underlying immaterial query provider.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Materialize<TResult>(this IQueryPlan<TResult> query)
        {
            return ((IQueryPlan<Guid, TResult>)query).Materialize().AsEntityQuery();
        }

        /// <summary>
        /// Marks a point in this query plan where entities are guaranteed to be materialized in memory.
        /// Use this before performing actions which may not be supported by the underlying immaterial query provider.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Materialize<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }
            else if (query.State == QueryPlanState.Materialized)
            {
                return query;
            }

            return new MaterializeQueryPlan<TKey, TResult>(query);
        }
    }
}

namespace Solar.Ecs.Queries
{
    public class MaterializeQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        public IQueryPlan<TKey, TResult> BaseQuery { get; private set; }

        public MaterializeQueryPlan(IQueryPlan<TKey, TResult> baseQuery)
        {
            this.BaseQuery = baseQuery;
        }

        public QueryPlanState State
        {
            get { return QueryPlanState.Materialized; }
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get { throw new InvalidOperationException("Cannot retrieve an ImmaterialQuery from a materialized query plan."); }
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            return BaseQuery.Execute(predicate);
        }
    }
}

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
        /// Concatenates two queries.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="withQuery">The query to concatenate to this one.</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Concat<TResult>(this IQueryPlan<TResult> query, IQueryPlan<TResult> withQuery)
        {
            return ((IQueryPlan<Guid, TResult>)query).Concat(withQuery).AsEntityQuery();
        }

        /// <summary>
        /// Concatenates two queries.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="withQuery">The query to concatenate to this one.</param>
        /// <returns></returns>
        [Obsolete($"Deprecated due to deceptive name. Use {nameof(Concat)} instead.")]
        public static IQueryPlan<TResult> Union<TResult>(this IQueryPlan<TResult> query, IQueryPlan<TResult> withQuery) => query.Concat(withQuery);

        /// <summary>
        /// Concatenates two queries.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="withQuery">The query to concatenate to this one.</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Concat<TKey, TResult>(this IQueryPlan<TKey, TResult> query, IQueryPlan<TKey, TResult> withQuery)
        {
            var queries = new[] { query, withQuery };
            return new ConcatenateQueryPlan<TKey, TResult>(queries);
        }

        /// <summary>
        /// Concatenates two queries.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query.</param>
        /// <param name="withQuery">The query to concatenate to this one.</param>
        /// <returns></returns>
        [Obsolete($"Deprecated due to deceptive name. Use {nameof(Concat)} instead.")]
        public static IQueryPlan<TKey, TResult> Union<TKey, TResult>(this IQueryPlan<TKey, TResult> query, IQueryPlan<TKey, TResult> withQuery) => query.Concat(withQuery);

        /// <summary>
        /// Concatenates multiple query plans.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Concat<TKey, TResult>(IEnumerable<IQueryPlan<TKey, TResult>> queries)
        {
            return new ConcatenateQueryPlan<TKey, TResult>(queries);
        }

        /// <summary>
        /// Concatenates multiple query plans.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queries"></param>
        /// <returns></returns>
        [Obsolete($"Deprecated due to deceptive name. Use {nameof(Concat)} instead.")]
        public static IQueryPlan<TKey, TResult> Union<TKey, TResult>(IEnumerable<IQueryPlan<TKey, TResult>> queries) => Concat(queries);

        /// <summary>
        /// Concatenates multiple query plans.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Concat<TResult>(IEnumerable<IQueryPlan<TResult>> queries)
        {
            return new ConcatenateQueryPlan<Guid, TResult>(queries).AsEntityQuery();
        }

        /// <summary>
        /// Concatenates multiple query plans.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queries"></param>
        /// <returns></returns>
        [Obsolete($"Deprecated due to deceptive name. Use {nameof(Concat)} instead.")]
        public static IQueryPlan<TResult> Union<TResult>(IEnumerable<IQueryPlan<TResult>> queries) => Concat(queries);
    }
}

namespace SolarEcs.Queries
{
    public class ConcatenateQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        readonly IEnumerable<IQueryPlan<TKey, TResult>> Plans;

        public ConcatenateQueryPlan(IEnumerable<IQueryPlan<TKey, TResult>> plans)
        {
            this.Plans = plans.Where(o => o.State != QueryPlanState.Empty).ToList();
        }

        public QueryPlanState State
        {
            get
            {
                if (!Plans.Any())
                {
                    return QueryPlanState.Empty;
                }
                else if (Plans.Any(o => o.State == QueryPlanState.Materialized))
                {
                    return QueryPlanState.Materialized;
                }
                else
                {
                    return QueryPlanState.Immaterial;
                }
            }
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get
            {
                return UnionImmaterialPlans(Plans);
            }
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (State == QueryPlanState.Empty)
            {
                return Enumerable.Empty<IKeyWith<TKey, TResult>>();
            }
            else if (State == QueryPlanState.Materialized)
            {
                return ExecuteMaterialized(predicate);
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TKey, TResult>()).ToList();
            }
        }

        private IEnumerable<IKeyWith<TKey, TResult>> ExecuteMaterialized(Expression<Func<TKey, bool>> predicate)
        {
            var plansByMaterializedStatus = Plans.ToLookup(o => o.State);

            var immaterialPlans = plansByMaterializedStatus[QueryPlanState.Immaterial];
            var materialPlans = plansByMaterializedStatus[QueryPlanState.Materialized];

            var results = Enumerable.Empty<IKeyWith<TKey, TResult>>();

            if (immaterialPlans.Any())
            {
                results = results.Concat(UnionImmaterialPlans(immaterialPlans).Where(predicate.ForKeyWith<TKey, TResult>()).ToList());
            }

            foreach (var plan in materialPlans)
            {
                results = results.Concat(plan.Execute(predicate));
            }

            return results;
        }

        private IQueryable<IKeyWith<TKey, TResult>> UnionImmaterialPlans(IEnumerable<IQueryPlan<TKey, TResult>> plans)
        {
            var query = plans.First().ImmaterialQuery;

            foreach (var plan in plans.Skip(1))
            {
                query = query.Concat(plan.ImmaterialQuery);
            }

            return query;
        }
    }
}

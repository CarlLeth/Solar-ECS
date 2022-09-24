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
        /// Modifies the IQueryable underlying this query plan using the transformation given.
        /// Keys are not preserved, which may have performance implications.
        /// </summary>
        /// <typeparam name="TStart"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query plan.</param>
        /// <param name="transformation">A transformation to apply to the underlying IQueryable of this query plan.</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> TransformQuery<TStart, TResult>(this IQueryPlan<TStart> query,
            Func<IQueryable<IKeyWith<Guid, TStart>>, IQueryable<IKeyWith<Guid, TResult>>> transformation)
        {
            return Transform(query, transformation, false);
        }

        /// <summary>
        /// Modifies the IQueryable underlying this query plan using the transformation given.
        /// Keys are not preserved, which may have performance implications.
        /// </summary>
        /// <typeparam name="TStart"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query plan.</param>
        /// <param name="transformation">A transformation to apply to the underlying IQueryable of this query plan.</param>
        /// <returns></returns>
        public static IQueryPlan<TEndKey, TEndResult> TransformQuery<TStartKey, TStartResult, TEndKey, TEndResult>(this IQueryPlan<TStartKey, TStartResult> query,
            Func<IQueryable<IKeyWith<TStartKey, TStartResult>>, IQueryable<IKeyWith<TEndKey, TEndResult>>> transformation)
        {
            return Transform(query, transformation, false);
        }

        /// <summary>
        /// Modifies the IQueryable underlying this query plan using the transformation given.
        /// The transformation must preserve the meaning of the keys of each element.
        /// </summary>
        /// <typeparam name="TStart"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query plan.</param>
        /// <param name="transformation">A transformation to apply to the underlying IQueryable of this query plan. Must preserve the meaning of element keys.</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> TransformWithKeyPreserved<TStart, TResult>(this IQueryPlan<TStart> query,
            Func<IQueryable<IKeyWith<Guid, TStart>>, IQueryable<IKeyWith<Guid, TResult>>> transformation)
        {
            return Transform(query, transformation, true);
        }

        /// <summary>
        /// Modifies the IQueryable underlying this query plan using the transformation given.
        /// The transformation must preserve the meaning of the keys of each element.
        /// </summary>
        /// <typeparam name="TStart"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This query plan.</param>
        /// <param name="transformation">A transformation to apply to the underlying IQueryable of this query plan. Must preserve the meaning of element keys.</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TEndResult> TransformWithKeyPreserved<TKey, TStartResult, TEndResult>(this IQueryPlan<TKey, TStartResult> query,
            Func<IQueryable<IKeyWith<TKey, TStartResult>>, IQueryable<IKeyWith<TKey, TEndResult>>> transformation)
        {
            return Transform(query, transformation, true);
        }

        private static IQueryPlan<TResult> Transform<TStart, TResult>(this IQueryPlan<TStart> query,
            Func<IQueryable<IKeyWith<Guid, TStart>>, IQueryable<IKeyWith<Guid, TResult>>> transformation, bool canPassPrediateThrough)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TResult>();
            }

            return new TransformQueryPlan<Guid, TStart, Guid, TResult>(query, transformation, canPassPrediateThrough).AsEntityQuery();
        }

        private static IQueryPlan<TEndKey, TEndResult> Transform<TStartKey, TStartResult, TEndKey, TEndResult>(this IQueryPlan<TStartKey, TStartResult> query,
            Func<IQueryable<IKeyWith<TStartKey, TStartResult>>, IQueryable<IKeyWith<TEndKey, TEndResult>>> transformation, bool canPassPrediateThrough)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TEndKey, TEndResult>();
            }

            return new TransformQueryPlan<TStartKey, TStartResult, TEndKey, TEndResult>(query, transformation, canPassPrediateThrough);
        }
    }
}

namespace SolarEcs.Queries
{
    public class TransformQueryPlan<TStartKey, TStartResult, TEndKey, TEndResult> : IOrderedQueryPlan<TEndKey, TEndResult>
    {
        private IQueryPlan<TStartKey, TStartResult> BaseQuery;
        private Func<IQueryable<IKeyWith<TStartKey, TStartResult>>, IQueryable<IKeyWith<TEndKey, TEndResult>>> Transformation;
        private bool CanPassPredicateThrough;

        public TransformQueryPlan(IQueryPlan<TStartKey, TStartResult> baseQuery, Func<IQueryable<IKeyWith<TStartKey, TStartResult>>, IQueryable<IKeyWith<TEndKey, TEndResult>>> transformation,
            bool canPassPredicateThrough)
        {
            this.BaseQuery = baseQuery;
            this.Transformation = transformation;
            this.CanPassPredicateThrough = canPassPredicateThrough;
        }

        public QueryPlanState State
        {
            get { return BaseQuery.State; }
        }

        public IQueryable<IKeyWith<TEndKey, TEndResult>> ImmaterialQuery
        {
            get { return Transformation(BaseQuery.ImmaterialQuery); }
        }

        public IEnumerable<IKeyWith<TEndKey, TEndResult>> Execute(Expression<Func<TEndKey, bool>> predicate)
        {
            if (State == QueryPlanState.Materialized)
            {
                if (CanPassPredicateThrough)
                {
                    var startKeyPredicate = predicate as Expression<Func<TStartKey, bool>>;
                    return Transformation(BaseQuery.Execute(startKeyPredicate).AsQueryable());
                }
                else
                {
                    var predicateCompiled = predicate.Compile();
                    return Transformation(BaseQuery.ExecuteAll().AsQueryable()).Where(o => predicateCompiled(o.Key));
                }
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TEndKey, TEndResult>()).ToList();
            }
        }
    }
}

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
        /// Returns an empty IQueryPlan with the specified key and model types.
        /// </summary>
        /// <typeparam name="TKey">The key type for the empty query.</typeparam>
        /// <typeparam name="TModel">The model type for the empty query.</typeparam>
        /// <returns></returns>
        public static IQueryPlan<TKey, TModel> Empty<TKey, TModel>()
        {
            return new EmptyQueryPlan<TKey, TModel>();
        }

        /// <summary>
        /// Returns an empty IQueryPlan with the specified model type and an implicit Guid key.
        /// </summary>
        /// <typeparam name="TModel">The model type for the empty query.</typeparam>
        /// <returns></returns>
        public static IQueryPlan<TModel> Empty<TModel>()
        {
            return new EmptyQueryPlan<Guid, TModel>().AsEntityQuery();
        }
    }
}

namespace Solar.Ecs.Queries
{
    public class EmptyQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            return Enumerable.Empty<IKeyWith<TKey, TResult>>();
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get { throw new InvalidOperationException("Cannot retrieve an ImmaterialQuery from an empty query plan."); }
        }

        public QueryPlanState State
        {
            get { return QueryPlanState.Empty; }
        }
    }
}

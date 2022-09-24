using SolarEcs.Infrastructure;
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
        /// Applies a filter to the elements of this IQueryPlan based on the IDs (keys) of their associated entities.
        /// Prefer over Where if the filter is only on the key.
        /// </summary>
        /// <param name="predicate">The filter to apply</param>
        public static IQueryPlan<TResult> WhereKey<TResult>(this IQueryPlan<TResult> query, Expression<Func<Guid, bool>> predicate)
        {
            return ((IQueryPlan<Guid, TResult>)query).WhereKey(predicate).AsEntityQuery();
        }

        /// <summary>
        /// Filters this IQueryPlan down to a single entity.
        /// Prefer over Where if the filter is only on the key.
        /// </summary>
        /// <param name="entity">The entity to restrict this query to</param>
        public static IQueryPlan<TResult> For<TResult>(this IQueryPlan<TResult> query, Guid entity)
        {
            return query.WhereKey(o => o == entity);
        }

        /// <summary>
        /// Filters this IQueryPlan down to a set of entities.
        /// Prefer over Where if the filter is only on the key.
        /// </summary>
        /// <param name="entities">The entities to restrict this query to</param>
        public static IQueryPlan<TResult> For<TResult>(this IQueryPlan<TResult> query, params Guid[] entities)
        {
            return query.WhereKey(o => entities.Contains(o));
        }

        /// <summary>
        /// Filters this IQueryPlan down to a set of entities.
        /// Prefer over Where if the filter is only on the key.
        /// </summary>
        /// <param name="entities">The entities to restrict this query to</param>
        public static IQueryPlan<TResult> For<TResult>(this IQueryPlan<TResult> query, IEnumerable<Guid> entities)
        {
            return query.WhereKey(o => entities.Contains(o));
        }

        /// <summary>
        /// Applies a filter to the elements of this IQueryPlan based on their keys.
        /// Prefer over Where if the filter is only on the key.
        /// </summary>
        /// <param name="predicate">The filter to apply</param>
        public static IQueryPlan<TKey, TResult> WhereKey<TKey, TResult>(this IQueryPlan<TKey, TResult> query, Expression<Func<TKey, bool>> predicate)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            return new FilterKeyQueryPlan<TKey, TResult>(query, predicate);
        }

        /// <summary>
        /// Filters this IQueryPlan to those whose keys are members of the given query.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="entityFilterQuery">The query containing entities to filter this query to</param>
        /// <returns></returns>
        public static IQueryPlan<TModel> WhereEntitiesIn<TModel, TOther>(this IQueryPlan<TModel> query, IQueryPlan<TOther> entityFilterQuery)
        {
            return query.EntityJoin(entityFilterQuery, (model, throwaway) => model);
        }

        /// <summary>
        /// Filters this IQueryPlan to those whose keys are members of the given query.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="keyFilterQuery">The query containing keys to filter this query to</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TModel> WhereKeysIn<TKey, TModel, TOther>(this IQueryPlan<TKey, TModel> query, IQueryPlan<TKey, TOther> keyFilterQuery)
        {
            return query.KeyJoin(keyFilterQuery, (model, throwaway) => model);
        }
    }
}

namespace SolarEcs.Queries
{
    public class FilterKeyQueryPlan<TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        private IQueryPlan<TKey, TResult> BaseQuery;
        private Expression<Func<TKey, bool>> Predicate;

        public FilterKeyQueryPlan(IQueryPlan<TKey, TResult> baseQuery, Expression<Func<TKey, bool>> predicate)
        {
            this.BaseQuery = baseQuery;
            this.Predicate = predicate;
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> additionalPredicate)
        {
            var param = Predicate.Parameters[0];

            var replacer = new ParameterReplacingExpressionVisitor();
            replacer.AddReplacementRule(additionalPredicate.Parameters[0], param);

            var composedBody = Expression.AndAlso(Predicate.Body, replacer.Visit(additionalPredicate.Body));
            var fullPredicate = Expression.Lambda<Func<TKey, bool>>(composedBody, param);

            return BaseQuery.Execute(fullPredicate);
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get { return BaseQuery.ImmaterialQuery.Where(Predicate.ForKeyWith<TKey, TResult>()); }
        }

        public QueryPlanState State
        {
            get { return BaseQuery.State; }
        }
    }
}

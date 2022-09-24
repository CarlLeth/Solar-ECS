using Solar.Ecs.Infrastructure;
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
        /// Immediately executes this query plan, filtering its key to a single entity.
        /// Returns the element corresponding to that entity.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">The query plan to execute.</param>
        /// <param name="entity">The entity to filter the query to.</param>
        /// <returns></returns>
        public static IKeyWith<Guid, TResult> Execute<TResult>(this IQueryPlan<Guid, TResult> query, Guid entity)
        {
            return query.Execute(o => o == entity).FirstOrDefault();
        }

        /// <summary>
        /// Immediately executes this query plan, filtering to the given list of entities.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">The query plan to execute.</param>
        /// <param name="firstEntity">The first entity to filter the query to.</param>
        /// <param name="otherEntities">Other entities to filter the query to.</param>
        /// <returns></returns>
        public static IEnumerable<IKeyWith<Guid, TResult>> Execute<TResult>(this IQueryPlan<Guid, TResult> query, Guid firstEntity, params Guid[] otherEntities)
        {
            var entities = otherEntities.ToList();
            entities.Add(firstEntity);
            return query.Execute(o => entities.Contains(o));
        }

        /// <summary>
        /// Immediately executes this query plan with no filter on its keys.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">The query plan to execute.</param>
        /// <returns></returns>
        public static IEnumerable<IKeyWith<TKey, TResult>> ExecuteAll<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            return query.Execute(o => true);
        }

        /// <summary>
        /// Immediately executes this query plan and maps its elements into a dictionary whose keys are the elements' keys and whose values are the elements' models.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">The query plan to execute.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TResult> ExecuteToDictionary<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            return query.ExecuteAll().ToDictionary(o => o.Key, o => o.Model);
        }

        /// <summary>
        /// Immediately executes this query plan, ignoring the model type and returning an IEnumerable of each key in the result set.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<TKey> ExecuteKeysOnly<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            if (query.State == QueryPlanState.Immaterial)
            {
                return query.ImmaterialQuery.Select(o => o.Key).ToList();
            }
            else
            {
                return query.Select(o => false).ExecuteAll().Select(o => o.Key);
            }
        }

        /// <summary>
        /// Immediately executes this query plan, ignoring the key type and returning an IEnumerable of each model in the result set.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> ExecuteModelsOnly<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            if (query.State == QueryPlanState.Immaterial)
            {
                return query.ImmaterialQuery.Select(o => o.Model).ToList();
            }
            else
            {
                return query.GroupBy(o => 0).ExecuteAll().SelectMany(o => o.Model);
            }
        }
    }
}

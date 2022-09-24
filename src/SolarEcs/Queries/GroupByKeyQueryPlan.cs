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
        /// Groups the elements of this IQueryPlan by their key.  Models associated to the same key will be grouped into the same IEnumerable.
        /// Example: {K1, M1a}, {K1, M1b}, {K2, M2} -> {K1, [M1a, M1b]}, {K2, [M2]}
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns>An IQueryPlan containing one element per distinct key in the original query, whose model is an IEnumerable of all models associated with that key.</returns>
        public static IQueryPlan<IEnumerable<TResult>> GroupByKey<TResult>(this IQueryPlan<TResult> query)
        {
            return query.GroupByKey(o => o);
        }

        /// <summary>
        /// Groups the elements of this IQueryPlan by their key.  Models associated to the same key will be grouped into the same IEnumerable.
        /// Example: {K1, M1a}, {K1, M1b}, {K2, M2} -> {K1, [M1a, M1b]}, {K2, [M2]}
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns>An IQueryPlan containing one element per distinct key in the original query, whose model is an IEnumerable of all models associated with that key.</returns>
        public static IQueryPlan<TKey, IEnumerable<TResult>> GroupByKey<TKey, TResult>(this IQueryPlan<TKey, TResult> query)
        {
            return query.GroupByKey(o => o);
        }

        /// <summary>
        /// Groups the elements of this IQueryPlan by their key and transforms the resulting IEnumerable.
        /// Models associated to the same key will be grouped into the same IEnumerable, which is then transformed by the aggregation expression.
        /// Example: {K1, M1a}, {K1, M1b}, {K2, M2} -> {K1, aggregation([M1a, M1b])}, {K2, aggregation([M2])}
        /// Functionally equivalent to .Aggregate().Select(aggregation).
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns>An IQueryPlan containing one element per distinct key in the original query, whose model is an IEnumerable of all models
        /// associated with that key transformed by the aggregation expression.</returns>
        public static IQueryPlan<TAggregate> GroupByKey<TResult, TAggregate>(this IQueryPlan<TResult> query,
            Expression<Func<IEnumerable<TResult>, TAggregate>> aggregation)
        {
            return ((IQueryPlan<Guid, TResult>)query).GroupByKey(aggregation).AsEntityQuery();
        }

        /// <summary>
        /// Groups the elements of this IQueryPlan by their key and transforms the resulting IEnumerable.
        /// Models associated to the same key will be grouped into the same IEnumerable, which is then transformed by the aggregation expression.
        /// Example: {K1, M1a}, {K1, M1b}, {K2, M2} -> {K1, aggregation([M1a, M1b])}, {K2, aggregation([M2])}
        /// Functionally equivalent to Aggregate().Select(aggregation).
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns>An IQueryPlan containing one element per distinct key in the original query, whose model is an IEnumerable of all models
        /// associated with that key transformed by the aggregation expression.</returns>
        public static IQueryPlan<TKey, TAggregate> GroupByKey<TKey, TResult, TAggregate>(this IQueryPlan<TKey, TResult> query,
            Expression<Func<IEnumerable<TResult>, TAggregate>> aggregation)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TAggregate>();
            }
            else
            {
                return new GroupByKeyQueryPlan<TKey, TResult, TAggregate>(query, aggregation);
            }
        }
    }
}

namespace SolarEcs.Queries
{
    public class GroupByKeyQueryPlan<TKey, TResult, TAggregate> : IQueryPlan<TKey, TAggregate>
    {
        private IQueryPlan<TKey, TResult> BaseQuery;
        private Expression<Func<IEnumerable<TResult>, TAggregate>> Aggregation;

        public GroupByKeyQueryPlan(IQueryPlan<TKey, TResult> baseQuery, Expression<Func<IEnumerable<TResult>, TAggregate>> aggregation)
        {
            this.BaseQuery = baseQuery;
            this.Aggregation = aggregation;
        }

        public QueryPlanState State
        {
            get
            {
                return BaseQuery.State;
            }
        }

        public IEnumerable<IKeyWith<TKey, TAggregate>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (State == QueryPlanState.Materialized)
            {
                var compiled = Aggregation.Compile();
                return BaseQuery.Execute(predicate)
                    .GroupBy(o => o.Key)
                    .Select(grp => new KeyWith<TKey, TAggregate>(grp.Key, compiled(grp.Select(o => o.Model))));
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TKey, TAggregate>()).ToList();
            }
        }

        public IQueryable<IKeyWith<TKey, TAggregate>> ImmaterialQuery
        {
            get
            {
                var fullSelector = CreateFullSelector();
                return BaseQuery.ImmaterialQuery.GroupBy(o => o.Key).Select(fullSelector);
            }
        }

        private Expression<Func<IGrouping<TKey, IKeyWith<TKey, TResult>>, KeyWith<TKey, TAggregate>>> CreateFullSelector()
        {
            Expression<Func<IGrouping<TKey, IKeyWith<TKey, TResult>>, TAggregate, KeyWith<TKey, TAggregate>>> template =
                (grp, placeholder) => new KeyWith<TKey, TAggregate>(grp.Key, placeholder);

            var param = template.Parameters[0];
            var aggregatePlaceholder = template.Parameters[1];

            var aggregateSelector = CreateAggregateSelector(param);

            var replacer = new ParameterReplacingExpressionVisitor();
            replacer.AddReplacementRule(aggregatePlaceholder, aggregateSelector);

            var body = QueryExpressions.Clean(replacer.Visit(template.Body));

            return Expression.Lambda<Func<IGrouping<TKey, IKeyWith<TKey, TResult>>, KeyWith<TKey, TAggregate>>>(body, param);
        }

        private Expression CreateAggregateSelector(ParameterExpression param)
        {
            Expression<Func<IGrouping<TKey, IKeyWith<TKey, TResult>>, IEnumerable<TResult>>> resultSelector = grp => grp.Select(o => o.Model);
            var aggregateSelector = resultSelector.Into(Aggregation);

            var replacer = new ParameterReplacingExpressionVisitor();
            replacer.AddReplacementRule(aggregateSelector.Parameters[0], param);

            return replacer.Visit(aggregateSelector.Body);
        }
    }
}

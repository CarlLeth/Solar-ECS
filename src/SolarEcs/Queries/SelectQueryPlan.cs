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
        /// Transforms the model of each element in this query into a new form as specified by a selector function on the model.
        /// Example: {K1, M1}, {K2, M2} -> {K1, selector(M1)}, {K2, selector(M2)}.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="selector">The expression to apply to each element in the query</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Select<TSource, TResult>(this IQueryPlan<TSource> query, Expression<Func<TSource, TResult>> selector)
        {
            return ((IQueryPlan<Guid, TSource>)query).Select(selector).AsEntityQuery();
        }

        /// <summary>
        /// Transforms the model of each element in this query into a new form as specified by a selector function on the model.
        /// Example: {K1, M1}, {K2, M2} -> {K1, selector(M1)}, {K2, selector(M2)}.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="selector">The expression to apply to each element in the query</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Select<TKey, TSource, TResult>(this IQueryPlan<TKey, TSource> query, Expression<Func<TSource, TResult>> selector)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            //The key is not used in the selector, but is necessary for the expression.  Create a key parameter that will be ignored by the expression body.
            var keyParam = Expression.Parameter(typeof(TKey));

            var selectorWithKey = (Expression<Func<TKey, TSource, TResult>>)Expression.Lambda(selector.Body, keyParam, selector.Parameters[0]);
            return query.Select(selectorWithKey);
        }

        /// <summary>
        /// Transforms the model of each element in this query into a new form as specified by a selector function on the key and model.
        /// Example: {K1, M1}, {K2, M2} -> {K1, selector(K1, M1)}, {K2, selector(K2, M2)}.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="selectorWithId">The expression to apply to each element in the query</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> Select<TSource, TResult>(this IQueryPlan<TSource> query, Expression<Func<Guid, TSource, TResult>> selectorWithId)
        {
            return ((IQueryPlan<Guid, TSource>)query).Select(selectorWithId).AsEntityQuery();
        }

        /// <summary>
        /// Transforms the model of each element in this query into a new form as specified by a selector function on the key and model.
        /// Example: {K1, M1}, {K2, M2} -> {K1, selector(K1, M1)}, {K2, selector(K2, M2)}.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">This IQueryPlan</param>
        /// <param name="selectorWithKey">The expression to apply to each element in the query</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> Select<TKey, TSource, TResult>(this IQueryPlan<TKey, TSource> query, Expression<Func<TKey, TSource, TResult>> selectorWithKey)
        {
            if (query.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            return new SelectQueryPlan<TKey, TSource, TResult>(query, selectorWithKey);
        }
    }
}

namespace SolarEcs.Queries
{
    public class SelectQueryPlan<TKey, TSource, TResult> : IQueryPlan<TKey, TResult>
    {
        public IQueryPlan<TKey, TSource> BaseQuery { get; private set; }
        public Expression<Func<TKey, TSource, TResult>> Transform { get; private set; }

        private Expression<Func<IKeyWith<TKey, TSource>, IKeyWith<TKey, TResult>>> _fullTransform;

        public SelectQueryPlan(IQueryPlan<TKey, TSource> baseQuery, Expression<Func<TKey, TSource, TResult>> transform)
        {
            this.BaseQuery = baseQuery;
            this.Transform = transform;
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (BaseQuery.State == QueryPlanState.Materialized)
            {
                var transformCompiled = Transform.Compile();
                return BaseQuery.Execute(predicate).Select(o => new KeyWith<TKey, TResult>(o.Key, transformCompiled(o.Key, o.Model)));
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TKey, TResult>()).ToList();
            }
        }

        public QueryPlanState State
        {
            get { return BaseQuery.State; }
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get
            {
                if (_fullTransform == null)
                {
                    _fullTransform = CreateFullTransform();
                }

                return BaseQuery.ImmaterialQuery.Select(_fullTransform);
            }
        }

        private Expression<Func<IKeyWith<TKey, TSource>, IKeyWith<TKey, TResult>>> CreateFullTransform()
        {
            var param = Expression.Parameter(typeof(IKeyWith<TKey, TSource>));

            var keyParam = Transform.Parameters[0];
            var keyAccessor = Expression.Property(param, "Key");

            var resultParam = Transform.Parameters[1];
            var resultAccessor = Expression.Property(param, "Model");

            var paramVisitor = new ParameterReplacingExpressionVisitor();
            paramVisitor.AddReplacementRule(keyParam, keyAccessor);
            paramVisitor.AddReplacementRule(resultParam, resultAccessor);

            Expression parameterReplacedBody = paramVisitor.Visit(Transform.Body);

            var keyWithConstructor = typeof(KeyWith<TKey, TResult>).GetConstructor(new Type[] { typeof(TKey), typeof(TResult) });
            var fullBody = Expression.New(keyWithConstructor, keyAccessor, parameterReplacedBody);

            Expression safeBody = QueryExpressions.Clean(fullBody);

            return Expression.Lambda<Func<IKeyWith<TKey, TSource>, IKeyWith<TKey, TResult>>>(safeBody, param);
        }
    }
}

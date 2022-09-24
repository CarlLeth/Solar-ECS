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
        /// Performs a cross join between this IQueryPlan and the given IQueryPlan.
        /// Resulting elements will be pairwise matches between all elements of the two IQueryPlans,
        /// with a key given by keySelector and model given by resultSelector.
        /// The resulting IQueryPlan will return a number of elements equal to |left| × |right|.
        /// </summary>
        /// <typeparam name="TLeftKey"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightKey"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left">This IQueryPlan</param>
        /// <param name="right">The IQueryPlan to cross join with this</param>
        /// <param name="keySelector">An expression applied to each pair of elements to produce the resulting key</param>
        /// <param name="resultSelector">An expression applied to each pair of elements to produce the resulting model</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> CrossJoin<TLeftKey, TLeftResult, TRightKey, TRightResult, TKey, TResult>(
            this IQueryPlan<TLeftKey, TLeftResult> left, IQueryPlan<TRightKey, TRightResult> right,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TKey>> keySelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector)
        {
            if (left.State == QueryPlanState.Empty || right.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }
            else
            {
                return new CrossJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TKey, TResult>(left, right, keySelector, resultSelector);
            }
        }
    }
}

namespace Solar.Ecs.Queries
{
    public class CrossJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TKey, TResult> : IQueryPlan<TKey, TResult>
    {
        private IQueryPlan<TLeftKey, TLeftResult> Left;
        private IQueryPlan<TRightKey, TRightResult> Right;
        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TKey>> KeySelector;
        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> ResultSelector;

        public CrossJoinQueryPlan(IQueryPlan<TLeftKey, TLeftResult> left, IQueryPlan<TRightKey, TRightResult> right,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TKey>> keySelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector)
        {
            this.Left = left;
            this.Right = right;
            this.KeySelector = keySelector;
            this.ResultSelector = resultSelector;
        }

        public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
        {
            if (State == QueryPlanState.Materialized)
            {
                var leftResults = Left.ExecuteAll();
                var rightResults = Right.ExecuteAll();

                var keySelectorCompiled = KeySelector.Compile();
                var resultSelectorCompiled = ResultSelector.Compile();

                return leftResults.CrossJoin(rightResults, (left, right) => new KeyWith<TKey, TResult>(keySelectorCompiled(left, right), resultSelectorCompiled(left, right)));
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TKey, TResult>()).ToList();
            }
        }

        public IQueryable<IKeyWith<TKey, TResult>> ImmaterialQuery
        {
            get
            {
                var leftQuery = Left.ImmaterialQuery;
                var rightQuery = Right.ImmaterialQuery;

                var keyBody = KeySelector.Body;
                var resultBody = CreateResultSelectorWithUnifiedParameters();

                var fullBody = QueryExpressions.Clean(NewKeyWith(keyBody, resultBody));

                var leftParam = KeySelector.Parameters[0];
                var rightParam = KeySelector.Parameters[1];

                var fullSelector = Expression.Lambda<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, IKeyWith<TKey, TResult>>>(fullBody, leftParam, rightParam);

                return leftQuery.CrossJoin(rightQuery, fullSelector);
            }
        }

        private Expression CreateResultSelectorWithUnifiedParameters()
        {
            var visitor = new ParameterReplacingExpressionVisitor();
            visitor.AddReplacementRule(ResultSelector.Parameters[0], KeySelector.Parameters[0]);
            visitor.AddReplacementRule(ResultSelector.Parameters[1], KeySelector.Parameters[1]);

            return visitor.Visit(ResultSelector.Body);
        }

        private Expression NewKeyWith(Expression key, Expression model)
        {
            var keyWithConstructor = typeof(KeyWith<TKey, TResult>).GetConstructor(new Type[] { typeof(TKey), typeof(TResult) });
            return Expression.New(keyWithConstructor, key, model);
        }

        public QueryPlanState State
        {
            get
            {
                if (Left.State == QueryPlanState.Materialized || Right.State == QueryPlanState.Materialized)
                {
                    return QueryPlanState.Materialized;
                }
                else
                {
                    return QueryPlanState.Immaterial;
                }
            }
        }
    }
}

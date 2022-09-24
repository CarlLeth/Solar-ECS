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
        /// Correlates the elements of two queries by matching their keys.
        /// The resulting query will contain one element for each pair of elements with matching keys from the joined queries,
        /// whose key is the same as the matched key from the joined queries, and whose model is defined by the resultSelector expression.
        /// Prefer over Join(rightQuery, left => left.Key, right => right.Key, resultSelector).
        /// Example: {K1, LM1}, {K2, LM2} entity join {K1, RM1}, {K3, RM3} -> {K1, resultSelector(LM1, RM1)}.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the outer query</param>
        /// <param name="rightQuery">The inner query to join to this query</param>
        /// <param name="resultSelector">An expression to create a resulting model</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> EntityJoin<TLeft, TRight, TResult>(this IQueryPlan<Guid, TLeft> leftQuery, IQueryPlan<Guid, TRight> rightQuery,
            Expression<Func<TLeft, TRight, TResult>> resultSelector)
        {
            return leftQuery.KeyJoin(rightQuery, resultSelector).AsEntityQuery();
        }

        /// <summary>
        /// Correlates the elements of two queries by matching their keys.
        /// The resulting query will contain one element for each pair of elements with matching keys from the joined queries,
        /// whose key is the same as the matched key from the joined queries, and whose model is defined by the resultSelector expression.
        /// Prefer over Join(rightQuery, left => left.Key, right => right.Key, resultSelector).
        /// Example: {K1, LM1}, {K2, LM2} key join {K1, RM1}, {K3, RM3} -> {K1, resultSelector(LM1, RM1)}.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the outer query</param>
        /// <param name="rightQuery">The inner query to join to this query</param>
        /// <param name="resultSelector">An expression to create a resulting model</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> KeyJoin<TLeft, TRight, TKey, TResult>(this IQueryPlan<TKey, TLeft> leftQuery, IQueryPlan<TKey, TRight> rightQuery,
            Expression<Func<TLeft, TRight, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty || rightQuery.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<TKey, TLeft, TKey>(leftQuery, o => o.Key, true);
            var rightSpec = new PlanJoinSpecification<TKey, TRight, TKey>(rightQuery, o => o.Key, true);

            var fullSelector = ExpandResultSelector<TLeft, TRight, TKey, TResult>(resultSelector);

            return new JoinQueryPlan<TKey, TLeft, TKey, TRight, TKey, TKey, TResult>(leftSpec, rightSpec, fullSelector, (left, right) => left.Key);
        }

        private static Expression<Func<IKeyWith<TKey, TLeft>, IKeyWith<TKey, TRight>, TResult>> ExpandResultSelector<TLeft, TRight, TKey, TResult>(
            Expression<Func<TLeft, TRight, TResult>> resultSelector)
        {

            var leftParam = Expression.Parameter(typeof(IKeyWith<TKey, TLeft>));
            var rightParam = Expression.Parameter(typeof(IKeyWith<TKey, TRight>));

            var leftModel = Expression.Property(leftParam, "Model");
            var rightModel = Expression.Property(rightParam, "Model");

            var replacer = new ParameterReplacingExpressionVisitor();
            replacer.AddReplacementRule(resultSelector.Parameters[0], leftModel);
            replacer.AddReplacementRule(resultSelector.Parameters[1], rightModel);

            return Expression.Lambda<Func<IKeyWith<TKey, TLeft>, IKeyWith<TKey, TRight>, TResult>>(replacer.Visit(resultSelector.Body), leftParam, rightParam);
        }

        /// <summary>
        /// Correlates the elements of two queries by generating join-keys for both sequencies and then matching.
        /// The resulting query will contain one element for each pair of elements with matched join-keys from the joined queries.
        /// By default, each element's key will be equal to its key from this (the outer/left) query.
        /// Use .TakeRightKey() or .TakeJoinKey() to select different keys for the resulting elements.
        /// </summary>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightKey"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TJoinKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the outer query</param>
        /// <param name="rightQuery">The inner query to join to this query</param>
        /// <param name="leftKeySelector">The expression to generate join-keys for this (outer/left) query</param>
        /// <param name="rightKeySelector">The expression to generate join-keys for the joined (inner/right) query</param>
        /// <param name="resultSelector">An expression to create a resulting model</param>
        /// <returns></returns>
        public static IJoinedQueryPlan<TRightKey, TJoinKey, TResult> Join<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<Guid, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<Guid, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty || rightQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlanImplicit<TRightKey, TJoinKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<Guid, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new JoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(leftSpec, rightSpec, resultSelector, (left, right) => left.Key);
        }

        /// <summary>
        /// Correlates the elements of two queries by generating join-keys for both sequencies and then matching.
        /// The resulting query will contain one element for each pair of elements with matched join-keys from the joined queries.
        /// By default, each element's key will be equal to its key from this (the outer/left) query.
        /// Use .TakeRightKey() or .TakeJoinKey() to select different keys for the resulting elements.
        /// </summary>
        /// <typeparam name="TLeftKey"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightKey"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TJoinKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the outer query</param>
        /// <param name="rightQuery">The inner query to join to this query</param>
        /// <param name="leftKeySelector">The expression to generate join-keys for this (outer/left) query</param>
        /// <param name="rightKeySelector">The expression to generate join-keys for the joined (inner/right) query</param>
        /// <param name="resultSelector">An expression to create a resulting model</param>
        /// <returns></returns>
        public static IJoinedQueryPlan<TLeftKey, TRightKey, TJoinKey, TLeftKey, TResult> Join<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<TLeftKey, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty || rightQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TLeftKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new JoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TLeftKey, TResult>(leftSpec, rightSpec, resultSelector, (left, right) => left.Key);
        }
    }
}

namespace Solar.Ecs.Queries
{
    public class JoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult> : JoinQueryPlan<Guid, TLeftResult, TRightKey, TRightResult, TJoinKey, Guid, TResult>,
        IJoinedQueryPlan<TRightKey, TJoinKey, TResult>
    {
        public JoinQueryPlanImplicit(PlanJoinSpecification<Guid, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<Guid, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, IKeyWith<TRightKey, TRightResult>, Guid>> keySelector)
            : base(left, right, resultSelector, keySelector)
        {
        }

        IQueryPlan<TResult> IJoinedQueryPlan<TRightKey, TJoinKey, TResult>.TakeLeftKey()
        {
            return this;
        }
    }

    public class JoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TSelectedKey, TResult> : IJoinedQueryPlan<TLeftKey, TRightKey, TJoinKey, TSelectedKey, TResult>
    {
        private PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> Left;
        private PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> Right;

        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> ResultSelector;
        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TSelectedKey>> KeySelector;

        public JoinQueryPlan(PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TResult>> resultSelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TSelectedKey>> keySelector)
        {
            this.Left = left;
            this.Right = right;
            this.ResultSelector = resultSelector;
            this.KeySelector = keySelector;
        }

        public QueryPlanState State
        {
            get
            {
                if (Left.Plan.State == QueryPlanState.Materialized || Right.Plan.State == QueryPlanState.Materialized)
                {
                    return QueryPlanState.Materialized;
                }
                else
                {
                    return QueryPlanState.Immaterial;
                }
            }
        }

        public IEnumerable<IKeyWith<TSelectedKey, TResult>> Execute(Expression<Func<TSelectedKey, bool>> predicate)
        {
            if (State == QueryPlanState.Materialized)
            {
                return CreateMaterialResults(predicate);
            }
            else
            {
                return ImmaterialQuery.Where(predicate.ForKeyWith<TSelectedKey, TResult>()).ToList();
            }
        }

        public IQueryable<IKeyWith<TSelectedKey, TResult>> ImmaterialQuery
        {
            get
            {
                var leftQuery = Left.Plan.ImmaterialQuery;
                var rightQuery = Right.Plan.ImmaterialQuery;

                var fullSelector = QueryExpressions.Clean(CreateFullSelector()) as Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, KeyWith<TSelectedKey, TResult>>>;

                var leftKeySelector = QueryExpressions.Clean(Left.KeySelector);
                var rightKeySelector = QueryExpressions.Clean(Right.KeySelector);

                return leftQuery.Join(rightQuery, leftKeySelector, rightKeySelector, fullSelector);
            }
        }

        private Expression CreateFullSelector()
        {
            var leftParam = ResultSelector.Parameters[0];
            var rightParam = ResultSelector.Parameters[1];

            var keyWithConstructor = typeof(KeyWith<TSelectedKey, TResult>).GetConstructor(new Type[] { typeof(TSelectedKey), typeof(TResult) });

            var fullBody = Expression.New(keyWithConstructor, CreateKeySelector(leftParam, rightParam), ResultSelector.Body);

            return Expression.Lambda(fullBody, leftParam, rightParam);
        }

        private Expression CreateKeySelector(ParameterExpression leftParam, ParameterExpression rightParam)
        {
            var paramReplacer = new ParameterReplacingExpressionVisitor();
            paramReplacer.AddReplacementRule(KeySelector.Parameters[0], leftParam);
            paramReplacer.AddReplacementRule(KeySelector.Parameters[1], rightParam);

            return paramReplacer.Visit(KeySelector.Body);
        }

        private IEnumerable<IKeyWith<TSelectedKey, TResult>> CreateMaterialResults(Expression<Func<TSelectedKey, bool>> predicate)
        {
            var resultSelectorCompiled = ResultSelector.Compile();
            var leftKeySelectorCompiled = Left.KeySelector.Compile();
            var rightKeySelectorCompiled = Right.KeySelector.Compile();
            var keySelectorCompiled = KeySelector.Compile();

            //First try to filter both queries by the predicate.
            var leftFiltered = JoinHelpers.FilterByPredicateIfPossible(Left, predicate);
            var rightFiltered = JoinHelpers.FilterByPredicateIfPossible(Right, predicate);

            //Next try to filter both queries by the other.
            leftFiltered = JoinHelpers.FilterByOtherSpecIfPossible(leftFiltered, rightFiltered);
            rightFiltered = JoinHelpers.FilterByOtherSpecIfPossible(rightFiltered, leftFiltered);

            var leftSet = leftFiltered.Execute();
            var rightSet = rightFiltered.Execute();

            var results = leftSet.Join(rightSet, leftKeySelectorCompiled, rightKeySelectorCompiled, (left, right) =>
                new KeyWith<TSelectedKey, TResult>(keySelectorCompiled(left, right), resultSelectorCompiled.Invoke(left, right)));

            //Immaterial filtering may not have been successful, so apply the compiled filter to the results.
            var predicateCompiled = predicate.Compile();
            return results.Where(o => predicateCompiled(o.Key));
        }

        public IQueryPlan<TLeftKey, TResult> TakeLeftKey()
        {
            return Shift((left, right) => left.Key, true, false);
        }

        public IQueryPlan<TRightKey, TResult> TakeRightKey()
        {
            return Shift((left, right) => right.Key, false, true);
        }

        public IQueryPlan<TJoinKey, TResult> TakeJoinKey()
        {
            var leftParam = Left.KeySelector.Parameters[0];
            var rightParam = Right.KeySelector.Parameters[0];

            //Construct (leftParam, rightParam) => Left.KeySelector(leftParam);
            var joinKeySelector = Expression.Lambda<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TJoinKey>>(Left.KeySelector.Body, leftParam, rightParam);

            return Shift(joinKeySelector, false, false);
        }

        private IQueryPlan<TNewKey, TResult> Shift<TNewKey>(Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IKeyWith<TRightKey, TRightResult>, TNewKey>> keySelector,
            bool passthroughLeft, bool passthroughRight)
        {
            var leftSpec = new PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey>(Left.Plan, Left.KeySelector, passthroughLeft);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(Right.Plan, Right.KeySelector, passthroughRight);

            return new JoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TNewKey, TResult>(leftSpec, rightSpec, ResultSelector, keySelector);
        }
    }
}

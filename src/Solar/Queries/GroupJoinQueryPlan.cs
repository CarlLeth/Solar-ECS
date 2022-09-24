using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Solar.Ecs.Infrastructure;
using Solar.Ecs.Queries;

namespace Solar
{
    public static partial class QueryPlan
    {
        /// <summary>
        /// Correlates the elements of this query with sequences of elements of the given query by generating join-keys for both sequencies and then matching.
        /// The resulting query will contain one element for each element in this outer query.
        /// By default, each element's key will be equal to its key from this (the outer/left) query.
        /// Use .TakeJoinKey() to select different keys for the resulting elements.
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
        public static ILeftJoinedQueryPlan<TJoinKey, TResult> GroupJoin<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<Guid, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<Guid, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty || rightQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlanImplicit<TRightKey, TJoinKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<Guid, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new GroupJoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(leftSpec, rightSpec, resultSelector, left => left.Key);
        }

        /// <summary>
        /// Correlates the elements of this query with sequences of elements of the given query by generating join-keys for both sequencies and then matching.
        /// The resulting query will contain one element for each element in this outer query.
        /// By default, each element's key will be equal to its key from this (the outer/left) query.
        /// Use .TakeJoinKey() to select different keys for the resulting elements.
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
        public static ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TLeftKey, TResult> GroupJoin<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<TLeftKey, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TLeftKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new GroupJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TLeftKey, TResult>(leftSpec, rightSpec, resultSelector, left => left.Key);
        }
    }
}

namespace Solar.Ecs.Queries
{
    public class GroupJoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult> : GroupJoinQueryPlan<Guid, TLeftResult, TRightKey, TRightResult, TJoinKey, Guid, TResult>,
        ILeftJoinedQueryPlan<TJoinKey, TResult>
    {
        public GroupJoinQueryPlanImplicit(PlanJoinSpecification<Guid, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<Guid, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, Guid>> keySelector)
            : base(left, right, resultSelector, keySelector)
        {
        }

        IQueryPlan<TResult> ILeftJoinedQueryPlan<TJoinKey, TResult>.TakeLeftKey()
        {
            return this;
        }
    }

    public class GroupJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TSelectedKey, TResult> : ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TSelectedKey, TResult>
    {
        private PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> Left;
        private PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> Right;

        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, TResult>> ResultSelector;
        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TSelectedKey>> KeySelector;

        public GroupJoinQueryPlan(PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TSelectedKey>> keySelector)
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

                var fullSelector = QueryExpressions.Clean(CreateFullSelector()) as Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, KeyWith<TSelectedKey, TResult>>>;

                var leftKeySelector = QueryExpressions.Clean(Left.KeySelector);
                var rightKeySelector = QueryExpressions.Clean(Right.KeySelector);

                return leftQuery.GroupJoin(rightQuery, leftKeySelector, rightKeySelector, fullSelector);
            }
        }

        private Expression CreateFullSelector()
        {
            var leftParam = ResultSelector.Parameters[0];
            var rightParam = ResultSelector.Parameters[1];

            var keyWithConstructor = typeof(KeyWith<TSelectedKey, TResult>).GetConstructor(new Type[] { typeof(TSelectedKey), typeof(TResult) });

            var fullBody = Expression.New(keyWithConstructor, CreateKeySelector(leftParam), ResultSelector.Body);

            return Expression.Lambda(fullBody, leftParam, rightParam);
        }

        private Expression CreateKeySelector(ParameterExpression leftParam)
        {
            var paramReplacer = new ParameterReplacingExpressionVisitor();
            paramReplacer.AddReplacementRule(KeySelector.Parameters[0], leftParam);

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

            //Next try to filter the right query by the left.  The left query should not be filtered by the right.
            rightFiltered = JoinHelpers.FilterByOtherSpecIfPossible(rightFiltered, leftFiltered);

            var leftSet = leftFiltered.Execute();
            var rightSet = rightFiltered.Execute();

            var results = leftSet.GroupJoin(rightSet, leftKeySelectorCompiled, rightKeySelectorCompiled, (left, right) =>
                new KeyWith<TSelectedKey, TResult>(keySelectorCompiled(left), resultSelectorCompiled.Invoke(left, right)));

            //Immaterial filtering may not have been successful, so apply the compiled filter to the results.
            var predicateCompiled = predicate.Compile();
            return results.Where(o => predicateCompiled(o.Key));
        }

        public IQueryPlan<TLeftKey, TResult> TakeLeftKey()
        {
            return Shift(left => left.Key, true);
        }

        public IQueryPlan<TJoinKey, TResult> TakeJoinKey()
        {
            return Shift(Left.KeySelector, Left.CanPassPredicateThrough);
        }

        private IQueryPlan<TNewKey, TResult> Shift<TNewKey>(Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TNewKey>> keySelector,
            bool passthroughLeft)
        {
            var leftSpec = new PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey>(Left.Plan, Left.KeySelector, passthroughLeft);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(Right.Plan, Right.KeySelector, false);

            return new GroupJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TNewKey, TResult>(leftSpec, rightSpec, ResultSelector, keySelector);
        }
    }
}

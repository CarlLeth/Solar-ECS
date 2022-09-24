using SolarEcs.Data;
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
        /// Correlates the elements of two queries by matching their keys, keeping elements in the left query with no matching elements in the right query.
        /// The resulting query will contain one element for each pair of elements with matching keys from the joined queries, plus one per element
        /// in the left query with no match in the right.
        /// The element's key will be the same as the matched key from the joined queries, and its model will be defined by the innerResultSelector expression,
        /// or by the leftOnlyResultSelector expression for those left elements with no match.
        /// Prefer over LeftJoin(rightQuery, left => left.Key, right => right.Key, innerResultSelector, leftOnlyResultSelector).
        /// Example: {K1, LM1}, {K2, LM2} entity left join {K1, RM1}, {K3, RM3} -> {K1, innerResultSelector(LM1, RM1)}, {K2, leftOnlyResultSelector(LM2)}.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the left query</param>
        /// <param name="rightQuery">The right query to join to this query</param>
        /// <param name="innerResultSelector">An expression to create a resulting model from a matched pair of elements.</param>
        /// <param name="leftOnlyResultSelector">An expression to create a resulting model from a left element only, when no match was found.</param>
        /// <returns></returns>
        public static IQueryPlan<TResult> EntityLeftJoin<TLeft, TRight, TResult>(this IQueryPlan<Guid, TLeft> leftQuery, IQueryPlan<Guid, TRight> rightQuery,
            Expression<Func<TLeft, Optional<TRight>, TResult>> resultSelector)
        {
            return KeyLeftJoin(leftQuery, rightQuery, resultSelector).AsEntityQuery();
        }

        /// <summary>
        /// Correlates the elements of two queries by matching their keys, keeping elements in the left query with no matching elements in the right query.
        /// The resulting query will contain one element for each pair of elements with matching keys from the joined queries, plus one per element
        /// in the left query with no match in the right.
        /// The element's key will be the same as the matched key from the joined queries, and its model will be defined by the innerResultSelector expression,
        /// or by the leftOnlyResultSelector expression for those left elements with no match.
        /// Prefer over LeftJoin(rightQuery, left => left.Key, right => right.Key, innerResultSelector, leftOnlyResultSelector).
        /// Example: {K1, LM1}, {K2, LM2} entity left join {K1, RM1}, {K3, RM3} -> {K1, innerResultSelector(LM1, RM1)}, {K2, leftOnlyResultSelector(LM2)}.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the left query</param>
        /// <param name="rightQuery">The right query to join to this query</param>
        /// <param name="innerResultSelector">An expression to create a resulting model from a matched pair of elements.</param>
        /// <param name="leftOnlyResultSelector">An expression to create a resulting model from a left element only, when no match was found.</param>
        /// <returns></returns>
        public static IQueryPlan<TKey, TResult> KeyLeftJoin<TLeft, TRight, TKey, TResult>(this IQueryPlan<TKey, TLeft> leftQuery, IQueryPlan<TKey, TRight> rightQuery,
            Expression<Func<TLeft, Optional<TRight>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty)
            {
                return Empty<TKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<TKey, TLeft, TKey>(leftQuery, o => o.Key, true);
            var rightSpec = new PlanJoinSpecification<TKey, TRight, TKey>(rightQuery, o => o.Key, true);

            var leftParam = Expression.Parameter(typeof(IKeyWith<TKey, TLeft>));
            var leftModel = Expression.Property(leftParam, "Model");

            Expression<Func<Optional<IKeyWith<TKey, TRight>>, Optional<TRight>>> rightSelector = optKeyModel => new Optional<TRight>() {
                ValueOrEmpty = optKeyModel.ValueOrEmpty.Select(km => km.Model)
            };

            var rightParam = rightSelector.Parameters[0];
            var rightBody = rightSelector.Body;

            var replacer = new ParameterReplacingExpressionVisitor();
            replacer.AddReplacementRule(resultSelector.Parameters[0], leftModel);
            replacer.AddReplacementRule(resultSelector.Parameters[1], rightBody);

            var fullSelector = Expression.Lambda<Func<IKeyWith<TKey, TLeft>, Optional<IKeyWith<TKey, TRight>>, TResult>>(replacer.Visit(resultSelector.Body), leftParam, rightParam);

            return new LeftJoinQueryPlan<TKey, TLeft, TKey, TRight, TKey, TKey, TResult>(leftSpec, rightSpec, fullSelector, left => left.Key);
        }

        /// <summary>
        /// Correlates the elements of two queries by generating join-keys for both sequencies, then matching, keeping elements in the left query with no matching elements in the right query.
        /// The resulting query will contain one element for each pair of elements with matching join-keys from the joined queries, plus one per element
        /// in the left query with no match in the right.
        /// By default, the element's key will be equal to its key from this (the left) query.
        /// Use .TakeJoinKey() to select the join-keys as the keys for the resulting elements.
        /// </summary>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightKey"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TJoinKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the left query</param>
        /// <param name="rightQuery">The right query to join to this query</param>
        /// <param name="leftKeySelector">The expression to generate join-keys for this (left) query.</param>
        /// <param name="rightKeySelector">The expression to generate join-keys for the joined (right) query.</param>
        /// <param name="innerResultSelector">An expression to create a resulting model from a matched pair of elements.</param>
        /// <param name="leftOnlyResultSelector">An expression to create a resulting model from a left element only, when no match was found.</param>
        /// <returns></returns>
        public static ILeftJoinedQueryPlan<TJoinKey, TResult> LeftJoin<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<Guid, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<Guid, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, Optional<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlanImplicit<TRightKey, TJoinKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<Guid, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new LeftJoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(leftSpec, rightSpec, resultSelector, left => left.Key);
        }

        /// <summary>
        /// Correlates the elements of two queries by generating join-keys for both sequencies, then matching, keeping elements in the left query with no matching elements in the right query.
        /// The resulting query will contain one element for each pair of elements with matching join-keys from the joined queries, plus one per element
        /// in the left query with no match in the right.
        /// By default, the element's key will be equal to its key from this (the left) query.
        /// Use .TakeJoinKey() to select the join-keys as the keys for the resulting elements.
        /// </summary>
        /// <typeparam name="TLeftKey"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightKey"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TJoinKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="leftQuery">This query; the left query</param>
        /// <param name="rightQuery">The right query to join to this query</param>
        /// <param name="leftKeySelector">The expression to generate join-keys for this (left) query.</param>
        /// <param name="rightKeySelector">The expression to generate join-keys for the joined (right) query.</param>
        /// <param name="innerResultSelector">An expression to create a resulting model from a matched pair of elements.</param>
        /// <param name="leftOnlyResultSelector">An expression to create a resulting model from a left element only, when no match was found.</param>
        /// <returns></returns>
        public static ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TLeftKey, TResult> LeftJoin<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TResult>(
            this IQueryPlan<TLeftKey, TLeftResult> leftQuery, IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TJoinKey>> leftKeySelector, Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, Optional<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector)
        {
            if (leftQuery.State == QueryPlanState.Empty)
            {
                return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TLeftKey, TResult>();
            }

            var leftSpec = new PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey>(leftQuery, leftKeySelector, true);
            var rightSpec = new PlanJoinSpecification<TRightKey, TRightResult, TJoinKey>(rightQuery, rightKeySelector, false);

            return new LeftJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TLeftKey, TResult>(leftSpec, rightSpec, resultSelector, left => left.Key);
        }
    }
}

namespace SolarEcs.Queries
{
    public class LeftJoinQueryPlanImplicit<TLeftResult, TRightKey, TRightResult, TJoinKey, TResult> : LeftJoinQueryPlan<Guid, TLeftResult, TRightKey, TRightResult, TJoinKey, Guid, TResult>,
        ILeftJoinedQueryPlan<TJoinKey, TResult>
    {
        public LeftJoinQueryPlanImplicit(PlanJoinSpecification<Guid, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<Guid, TLeftResult>, Optional<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector,
            Expression<Func<IKeyWith<Guid, TLeftResult>, Guid>> keySelector)
            : base (left, right, resultSelector, keySelector)
        {
        }

        public new IQueryPlan<TResult> TakeLeftKey()
        {
            return this;
        }
    }

    public class LeftJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TSelectedKey, TResult> : ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TSelectedKey, TResult>
    {
        private PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> Left;
        private PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> Right;

        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, Optional<IKeyWith<TRightKey, TRightResult>>, TResult>> ResultSelector;

        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, TSelectedKey>> KeySelector;

        public LeftJoinQueryPlan(PlanJoinSpecification<TLeftKey, TLeftResult, TJoinKey> left, PlanJoinSpecification<TRightKey, TRightResult, TJoinKey> right,
            Expression<Func<IKeyWith<TLeftKey, TLeftResult>, Optional<IKeyWith<TRightKey, TRightResult>>, TResult>> resultSelector,
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
                if (Right.Plan.State == QueryPlanState.Empty)
                {
                    return Left.Plan.State;
                }
                else if (Left.Plan.State == QueryPlanState.Materialized || Right.Plan.State == QueryPlanState.Materialized)
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
            if (Right.Plan.State == QueryPlanState.Empty)
            {
                return CreateBypassResults(predicate);
            }
            else if (State == QueryPlanState.Materialized)
            {
                return CreateMaterializedResults(predicate);
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
                return ImmaterialQueryJoined();
            }
        }

        private IQueryable<IKeyWith<TSelectedKey, TResult>> ImmaterialQueryJoined()
        {
            var leftQuery = Left.Plan.ImmaterialQuery;
            var rightQuery = Right.Plan.ImmaterialQuery;

            var selector = CreateJoinedSelector();

            var leftKeySelector = QueryExpressions.Clean(Left.KeySelector);
            var rightKeySelector = QueryExpressions.Clean(Right.KeySelector);

            return leftQuery.GroupJoin(rightQuery, leftKeySelector, rightKeySelector, selector);
        }

        private Expression<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, KeyWith<TSelectedKey, TResult>>> CreateJoinedSelector()
        {
            Expression<Func<IEnumerable<IKeyWith<TRightKey, TRightResult>>, Optional<IKeyWith<TRightKey, TRightResult>>>> setToOptional =
                set => new Optional<IKeyWith<TRightKey, TRightResult>>() { ValueOrEmpty = set };

            var paramLeft = ResultSelector.Parameters[0];
            var paramRight = setToOptional.Parameters[0];
            var selectorParamRight = setToOptional.Body;

            var resultParameterVisitor = new ParameterReplacingExpressionVisitor();
            resultParameterVisitor.AddReplacementRule(ResultSelector.Parameters[1], selectorParamRight);
            var resultSelector = resultParameterVisitor.Visit(ResultSelector.Body);

            var keyParameterVisitor = new ParameterReplacingExpressionVisitor();
            keyParameterVisitor.AddReplacementRule(KeySelector.Parameters[0], paramLeft);
            var keySelector = keyParameterVisitor.Visit(KeySelector.Body);

            var keyWithConstructor = typeof(KeyWith<TSelectedKey, TResult>).GetConstructor(new Type[] { typeof(TSelectedKey), typeof(TResult) });

            var fullBody = QueryExpressions.Clean(Expression.New(keyWithConstructor, keySelector, resultSelector));

            return Expression.Lambda<Func<IKeyWith<TLeftKey, TLeftResult>, IEnumerable<IKeyWith<TRightKey, TRightResult>>, KeyWith<TSelectedKey, TResult>>>(fullBody, paramLeft, paramRight);
        }

        private IEnumerable<IKeyWith<TSelectedKey, TResult>> CreateBypassResults(Expression<Func<TSelectedKey, bool>> predicate)
        {
            var resultSelectorCompiled = ResultSelector.Compile();
            var keySelectorCompiled = KeySelector.Compile();

            var leftFiltered = JoinHelpers.FilterByPredicateIfPossible(Left, predicate);

            var predicateCompiled = predicate.Compile();

            var nullOptional = Optional.Empty<IKeyWith<TRightKey, TRightResult>>();

            return leftFiltered.Execute()
                .Select(o => new KeyWith<TSelectedKey, TResult>(keySelectorCompiled(o), resultSelectorCompiled(o, nullOptional)))
                .Where(o => predicateCompiled(o.Key));
        }

        private IEnumerable<IKeyWith<TSelectedKey, TResult>> CreateMaterializedResults(Expression<Func<TSelectedKey, bool>> predicate)
        {
            var leftKeySelectorCompiled = Left.KeySelector.Compile();
            var rightKeySelectorCompiled = Right.KeySelector.Compile();
            var resultSelectorCompiled = ResultSelector.Compile();
            var keySelectorCompiled = KeySelector.Compile();

            //First try to filter both queries by the predicate
            var leftFiltered = JoinHelpers.FilterByPredicateIfPossible(Left, predicate);
            var rightFiltered = JoinHelpers.FilterByPredicateIfPossible(Right, predicate);

            //Next try to filter the right query by the left.  The left query should not be filtered by the right.
            rightFiltered = JoinHelpers.FilterByOtherSpecIfPossible(rightFiltered, leftFiltered);

            var leftSet = leftFiltered.Execute();
            var rightSet = rightFiltered.Execute();

            var results = leftSet.LeftJoin(rightSet, leftKeySelectorCompiled, rightKeySelectorCompiled, (left, right) => new KeyWith<TSelectedKey, TResult>(
                keySelectorCompiled(left),
                resultSelectorCompiled(left, right)
            ));

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

            return new LeftJoinQueryPlan<TLeftKey, TLeftResult, TRightKey, TRightResult, TJoinKey, TNewKey, TResult>(leftSpec, rightSpec, ResultSelector, keySelector);
        }
    }
}

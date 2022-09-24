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
        #region KeyJoins

        //Base implementation used by all unresolved key joins
        private static UnresolvedJoin<TKey, TTuple> CreateKeyJoin<TKey, TLeft, TRight, TTuple>(IQueryPlan<TKey, TLeft> leftQuery, IQueryPlan<TKey, TRight> rightQuery,
            Expression<Func<IKeyWith<TKey, TLeft>, IKeyWith<TKey, TRight>, TTuple>> tupledResultSelector)
        {
            var leftSpec = new PlanJoinSpecification<TKey, TLeft, TKey>(leftQuery, o => o.Key, true);
            var rightSpec = new PlanJoinSpecification<TKey, TRight, TKey>(rightQuery, o => o.Key, true);

            var tupled = new JoinQueryPlan<TKey, TLeft, TKey, TRight, TKey, TKey, TTuple>(leftSpec, rightSpec, tupledResultSelector, (left, right) => left.Key);

            return new UnresolvedJoin<TKey, TTuple>(tupled);
        }

        public static IUnresolvedJoin<Guid, DataTuple<IKeyWith<Guid, TLeft>, TRight>> EntityJoin<TLeft, TRight>(
            this IQueryPlan<Guid, TLeft> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<IKeyWith<TKey, TLeft>, TRight>> KeyJoin<TKey, TLeft, TRight>(
            this IQueryPlan<TKey, TLeft> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyJoin(leftQuery, rightQuery,
                (left, right) => new DataTuple<IKeyWith<TKey, TLeft>, TRight>(left, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TRightResult>> EntityJoin<TLeft1, TLeft2, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TRightResult>(left.Model.Item1, left.Model.Item2, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TLeft4, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, right.Model));
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightResult>> EntityJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightResult>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> unresolved, IQueryPlan<Guid, TRightResult> rightQuery)
        {
            return KeyJoin(unresolved, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightResult>> KeyJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightResult>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> unresolved, IQueryPlan<TKey, TRightResult> rightQuery)
        {
            return CreateKeyJoin(unresolved.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightResult>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, left.Model.Item8, right.Model));
        }

        #endregion

        #region FullJoins

        //Expression builder used by unresolved full joins on >2 queries
        private static Expression<Func<IKeyWith<TLeftKey, TLeftTuple>, TJoinKey>> CreateTupledKeySelectorLambda<TLeftKey, TLeftTuple, TJoinKey>(LambdaExpression leftKeySelector)
        {
            var param = Expression.Parameter(typeof(IKeyWith<TLeftKey, TLeftTuple>));
            var model = Expression.Property(param, "Model");

            var replacer = new ParameterReplacingExpressionVisitor();
            var tupleItemCount = typeof(TLeftTuple).GetGenericArguments().Length;

            for (int i = 0; i < tupleItemCount; i++)
            {
                //Construct o => o.Model.Item1 (for i = 0)
                var itemAccessor = Expression.Property(model, "Item" + (i + 1));
                replacer.AddReplacementRule(leftKeySelector.Parameters[i], itemAccessor);
            }

            var body = replacer.Visit(leftKeySelector.Body);
            return Expression.Lambda<Func<IKeyWith<TLeftKey, TLeftTuple>, TJoinKey>>(body, param);
        }

        public static IUnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, IKeyWith<TKey2, TResult2>>> Join<TKey1, TResult1, TKey2, TResult2, TJoinKey>(
            this IQueryPlan<TKey1, TResult1> query1,
            IQueryPlan<TKey2, TResult2> query2,
            Expression<Func<IKeyWith<TKey1, TResult1>, TJoinKey>> keySelector1,
            Expression<Func<IKeyWith<TKey2, TResult2>, TJoinKey>> keySelector2)
        {
            var tupled = query1.Join(query2, keySelector1, keySelector2, (o1, o2) => new DataTuple<IKeyWith<TKey1, TResult1>, IKeyWith<TKey2, TResult2>>(o1, o2));
            return new UnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, IKeyWith<TKey2, TResult2>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IKeyWith<TRightKey, TRightResult>>> Join<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.Join(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IKeyWith<TRightKey, TRightResult>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, left.Model.Item8, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IKeyWith<TRightKey, TRightResult>>>(tupled);
        }
        #endregion

        #region GroupJoins

        public static IUnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, IEnumerable<IKeyWith<TKey2, TResult2>>>> GroupJoin<TKey1, TResult1, TKey2, TResult2, TJoinKey>(
            this IQueryPlan<TKey1, TResult1> query1,
            IQueryPlan<TKey2, TResult2> query2,
            Expression<Func<IKeyWith<TKey1, TResult1>, TJoinKey>> keySelector1,
            Expression<Func<IKeyWith<TKey2, TResult2>, TJoinKey>> keySelector2)
        {
            var tupled = query1.GroupJoin(query2, keySelector1, keySelector2, (o1, o2) => new DataTuple<IKeyWith<TKey1, TResult1>, IEnumerable<IKeyWith<TKey2, TResult2>>>(o1, o2));
            return new UnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, IEnumerable<IKeyWith<TKey2, TResult2>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IEnumerable<IKeyWith<TRightKey, TRightResult>>>> GroupJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.GroupJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IEnumerable<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, left.Model.Item8, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, IEnumerable<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        #endregion

        #region LeftJoins

        //Base implementation used by all unresolved key joins
        private static UnresolvedJoin<TKey, TTuple> CreateKeyLeftJoin<TKey, TLeft, TRight, TTuple>(IQueryPlan<TKey, TLeft> leftQuery, IQueryPlan<TKey, TRight> rightQuery,
            Expression<Func<IKeyWith<TKey, TLeft>, Optional<IKeyWith<TKey, TRight>>, TTuple>> tupledResultSelector)
        {
            var leftSpec = new PlanJoinSpecification<TKey, TLeft, TKey>(leftQuery, o => o.Key, true);
            var rightSpec = new PlanJoinSpecification<TKey, TRight, TKey>(rightQuery, o => o.Key, false);

            var tupled = new LeftJoinQueryPlan<TKey, TLeft, TKey, TRight, TKey, TKey, TTuple>(leftSpec, rightSpec, tupledResultSelector, left => left.Key);

            return new UnresolvedJoin<TKey, TTuple>(tupled);
        }

        public static IUnresolvedJoin<Guid, DataTuple<IKeyWith<Guid, TLeft1>, Optional<TRight>>> EntityLeftJoin<TLeft1, TRight>(
            this IQueryPlan<Guid, TLeft1> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<IKeyWith<TKey, TLeft1>, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TRight>(
            this IQueryPlan<TKey, TLeft1> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery, rightQuery,
                (left, right) => new DataTuple<IKeyWith<TKey, TLeft1>, Optional<TRight>>(left, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, Optional<TRight>>(left.Model.Item1, left.Model.Item2, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TLeft4, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<TRight>>> EntityLeftJoin<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRight>(
            this IUnresolvedJoin<Guid, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> leftQuery, IQueryPlan<Guid, TRight> rightQuery)
        {
            return KeyLeftJoin(leftQuery, rightQuery);
        }

        public static IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<TRight>>> KeyLeftJoin<TKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRight>(
            this IUnresolvedJoin<TKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> leftQuery, IQueryPlan<TKey, TRight> rightQuery)
        {
            return CreateKeyLeftJoin(leftQuery.TupledPlan, rightQuery,
                (left, right) => new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<TRight>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, left.Model.Item8, new Optional<TRight>() { ValueOrEmpty = right.ValueOrEmpty.Select(o => o.Model) })
            );
        }

        public static IUnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, Optional<IKeyWith<TKey2, TResult2>>>> LeftJoin<TKey1, TResult1, TKey2, TResult2, TJoinKey>(
            this IQueryPlan<TKey1, TResult1> query1,
            IQueryPlan<TKey2, TResult2> query2,
            Expression<Func<IKeyWith<TKey1, TResult1>, TJoinKey>> keySelector1,
            Expression<Func<IKeyWith<TKey2, TResult2>, TJoinKey>> keySelector2)
        {
            var tupled = query1.LeftJoin(query2, keySelector1, keySelector2, (o1, o2) => new DataTuple<IKeyWith<TKey1, TResult1>, Optional<IKeyWith<TKey2, TResult2>>>(o1, o2));
            return new UnresolvedJoin<TKey1, DataTuple<IKeyWith<TKey1, TResult1>, Optional<IKeyWith<TKey2, TResult2>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        public static IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<IKeyWith<TRightKey, TRightResult>>>> LeftJoin<TLeftKey, TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TRightKey, TRightResult, TJoinKey>(
            this IUnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>> unresolved,
            IQueryPlan<TRightKey, TRightResult> rightQuery,
            Expression<Func<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, TJoinKey>> leftKeySelector,
            Expression<Func<IKeyWith<TRightKey, TRightResult>, TJoinKey>> rightKeySelector)
        {
            var tupledLeftKeySelector = CreateTupledKeySelectorLambda<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8>, TJoinKey>(leftKeySelector);
            var tupled = unresolved.TupledPlan.LeftJoin(rightQuery, tupledLeftKeySelector, rightKeySelector, (left, right) =>
                new DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<IKeyWith<TRightKey, TRightResult>>>(left.Model.Item1, left.Model.Item2, left.Model.Item3, left.Model.Item4, left.Model.Item5, left.Model.Item6, left.Model.Item7, left.Model.Item8, right));

            return new UnresolvedJoin<TLeftKey, DataTuple<TLeft1, TLeft2, TLeft3, TLeft4, TLeft5, TLeft6, TLeft7, TLeft8, Optional<IKeyWith<TRightKey, TRightResult>>>>(tupled);
        }

        #endregion

        private static IUnresolvedJoin<TKey, TTuple> FilterUnresolvedJoin<TKey, TTuple>(IUnresolvedJoin<TKey, TTuple> unresolved, LambdaExpression predicate)
        {
            var modelPredicate = CreateTupledResultSelector<TTuple, bool>(predicate);
            Expression<Func<IKeyWith<TKey, TTuple>, TTuple>> getModel = o => o.Model;
            var tupledPredicate = getModel.Into(modelPredicate);
            return new UnresolvedJoin<TKey, TTuple>(unresolved.TupledPlan.Where(tupledPredicate));
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2>> Where<TKey, T1, T2>(this IUnresolvedJoin<TKey, DataTuple<T1, T2>> unresolved,
            Expression<Func<T1, T2, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3>> Where<TKey, T1, T2, T3>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3>> unresolved,
            Expression<Func<T1, T2, T3, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4>> Where<TKey, T1, T2, T3, T4>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4>> unresolved,
            Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5>> Where<TKey, T1, T2, T3, T4, T5>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6>> Where<TKey, T1, T2, T3, T4, T5, T6>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7>> Where<TKey, T1, T2, T3, T4, T5, T6, T7>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>> Where<TKey, T1, T2, T3, T4, T5, T6, T7, T8>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        public static IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Where<TKey, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate)
        {
            return FilterUnresolvedJoin(unresolved, predicate);
        }

        #region Resolving

        //Expression builder used by all resolvers >2 queries
        private static Expression<Func<TTuple, TResult>> CreateTupledResultSelector<TTuple, TResult>(LambdaExpression resultSelector)
        {
            var param = Expression.Parameter(typeof(TTuple));

            var replacer = new ParameterReplacingExpressionVisitor();
            var tupleItemCount = typeof(TTuple).GetGenericArguments().Length;

            for (int i = 0; i < tupleItemCount; i++)
            {
                var itemAccessor = Expression.Property(param, "Item" + (i + 1));
                replacer.AddReplacementRule(resultSelector.Parameters[i], itemAccessor);
            }

            var body = replacer.Visit(resultSelector.Body);
            return Expression.Lambda<Func<TTuple, TResult>>(body, param);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2>> unresolved,
            Expression<Func<T1, T2, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2>> unresolved,
            Expression<Func<T1, T2, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3>> unresolved,
            Expression<Func<T1, T2, T3, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3>> unresolved,
            Expression<Func<T1, T2, T3, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4>> unresolved,
            Expression<Func<T1, T2, T3, T4, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4>> unresolved,
            Expression<Func<T1, T2, T3, T4, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, T5, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4, T5>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, T5, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, T5, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4, T5>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, T5, T6, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4, T5, T6>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, T5, T6, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, T5, T6, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4, T5, T6>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, T5, T6, T7, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4, T5, T6, T7>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, T5, T6, T7, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, T5, T6, T7, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4, T5, T6, T7>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        public static IQueryPlan<TResult> ResolveJoin<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IUnresolvedJoin<Guid, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> resultSelector)
        {
            return ResolveJoin<Guid, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(unresolved, resultSelector).AsEntityQuery();
        }

        public static IQueryPlan<TKey, TResult> ResolveJoin<TKey, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IUnresolvedJoin<TKey, DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>> unresolved,
            Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> resultSelector)
        {
            var tupledResultSelector = CreateTupledResultSelector<DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>, TResult>(resultSelector);
            return unresolved.TupledPlan.Select(tupledResultSelector);
        }

        #endregion
    }
}

namespace Solar.Ecs.Queries
{
    public class UnresolvedJoin<TKey, TTuple> : IUnresolvedJoin<TKey, TTuple>
    {
        public IQueryPlan<TKey, TTuple> TupledPlan { get; private set; }

        public UnresolvedJoin(IQueryPlan<TKey, TTuple> tupledPlan)
        {
            this.TupledPlan = tupledPlan;
        }
    }
}

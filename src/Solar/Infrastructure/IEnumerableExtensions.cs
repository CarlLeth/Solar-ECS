using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Infrastructure;
using SolarEcs;

namespace System.Linq
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> CrossJoin<TInner, TOuter, TResult>(this IEnumerable<TInner> inner, IEnumerable<TOuter> outer,
            Func<TInner, TOuter, TResult> resultSelector)
        {
            if (inner == null)
            {
                throw new ArgumentNullException("inner");
            }
            if (outer == null)
            {
                throw new ArgumentNullException("outer");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            return inner.SelectMany(o => outer, resultSelector);
        }

        public static IQueryable<TResult> CrossJoin<TInner, TOuter, TResult>(this IQueryable<TInner> inner, IQueryable<TOuter> outer,
            Expression<Func<TInner, TOuter, TResult>> resultSelector)
        {
            if (inner == null)
            {
                throw new ArgumentNullException("inner");
            }
            if (outer == null)
            {
                throw new ArgumentNullException("outer");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            return inner.SelectMany(o => outer, resultSelector);
        }

        public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, Optional<TRight>, TResult> resultSelector)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (leftKeySelector == null)
            {
                throw new ArgumentNullException("leftKeySelector");
            }
            if (rightKeySelector == null)
            {
                throw new ArgumentNullException("rightKeySelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            return left
                .GroupJoin(right, leftKeySelector, rightKeySelector, (l, rSet) => resultSelector(l, new Optional<TRight>(rSet)));
        }

        public static IQueryable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IQueryable<TLeft> left, IQueryable<TRight> right,
            Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<TLeft, Optional<TRight>, TResult>> resultSelector)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (leftKeySelector == null)
            {
                throw new ArgumentNullException("leftKeySelector");
            }
            if (rightKeySelector == null)
            {
                throw new ArgumentNullException("rightKeySelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            var joined = left
                .GroupJoin(right, leftKeySelector, rightKeySelector, (l, rSet) => new JoinHolder<TLeft, TRight>()
                {
                    Left = l,
                    Right = new Optional<TRight>() { ValueOrEmpty = rSet }
                });

            var joinedSelector = CreateJoinedSelector(resultSelector);

            return joined.Select(joinedSelector);
        }

        private static Expression<Func<JoinHolder<TLeft, TRight>, TResult>> CreateJoinedSelector<TLeft, TRight, TResult>(Expression<Func<TLeft, Optional<TRight>, TResult>> startExpression)
        {
            var param = Expression.Parameter(typeof(JoinHolder<TLeft, TRight>));
            var paramLeft = Expression.Property(param, "Left");
            var paramRight = Expression.Property(param, "Right");

            var parameterVisitor = new ParameterReplacingExpressionVisitor();
            parameterVisitor.AddReplacementRule(startExpression.Parameters[0], paramLeft);
            parameterVisitor.AddReplacementRule(startExpression.Parameters[1], paramRight);

            var body = parameterVisitor.Visit(startExpression.Body);

            return Expression.Lambda(body, param) as Expression<Func<JoinHolder<TLeft, TRight>, TResult>>;
        }

        private class JoinHolder<TLeft, TRight>
        {
            public TLeft Left { get; set; }
            public Optional<TRight> Right { get; set; }
        }

        internal static void ForEach<TElement>(this IEnumerable<TElement> enumerable, Action<TElement> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }

        internal static void ForEach<TElement>(this IEnumerable<TElement> enumerable, Action<TElement, int> indexedAction)
        {
            int i = 0;
            foreach (var element in enumerable)
            {
                indexedAction(element, i++);
            }
        }

        public static IEnumerable<TModel> Models<TKey, TModel>(this IEnumerable<IKeyWith<TKey, TModel>> entities)
        {
            return entities.Select(o => o.Model);
        }

        public static IEnumerable<TComponent> Components<TComponent>(this IEnumerable<EntityWith<TComponent>> entities)
        {
            return entities.Select(o => o.Component);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Queries
{
    internal static class JoinHelpers
    {
        public static PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey> FilterByPredicateIfPossible<TFirstKey, TFirstResult, TJoinKey, TSelectedKey>(
            PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey> first, Expression<Func<TSelectedKey, bool>> predicate)
        {
            if (first.CanPassPredicateThrough && typeof(TFirstKey) == typeof(TSelectedKey))
            {
                var passthroughPredicate = Expression.Lambda<Func<TFirstKey, bool>>(predicate.Body, predicate.Parameters[0]);
                return new PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey>(first.Plan.WhereKey(passthroughPredicate), first.KeySelector, first.CanPassPredicateThrough);
            }
            else
            {
                return first;
            }
        }

        public static PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey> FilterByOtherSpecIfPossible<TFirstKey, TFirstResult, TSecondKey, TSecondResult, TJoinKey>(
            PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey> first, PlanJoinSpecification<TSecondKey, TSecondResult, TJoinKey> second)
        {
            bool isPrimitiveType = typeof(TJoinKey).IsValueType || typeof(TJoinKey) == typeof(string);

            if (isPrimitiveType && first.Plan.State == QueryPlanState.Immaterial && second.Plan.State == QueryPlanState.Materialized)
            {
                var secondKeySelectorCompiled = second.KeySelector.Compile();
                var joinKeysFromSecond = second.Execute().Select(secondKeySelectorCompiled);
                Expression<Func<TJoinKey, bool>> joinKeysPredicate = o => joinKeysFromSecond.Contains(o);

                var firstPred = first.KeySelector.Into(joinKeysPredicate);

                return new PlanJoinSpecification<TFirstKey, TFirstResult, TJoinKey>(
                    first.Plan.TransformQuery(qry => qry.Where(firstPred)), first.KeySelector, first.CanPassPredicateThrough);
            }
            else
            {
                return first;
            }
        }
    }
}

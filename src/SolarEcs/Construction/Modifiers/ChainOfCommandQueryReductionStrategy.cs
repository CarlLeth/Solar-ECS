using SolarEcs.Construction;
using SolarEcs.Construction.Modifiers;
using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.Modifiers
{
    public class ChainOfCommandQueryReductionStrategy<TKey, TResult> : IReductionStrategy<IQueryPlan<TKey, TResult>>
    {
        private IDictionary<Type, int> SystemPriorityByType;

        public ChainOfCommandQueryReductionStrategy(IEnumerable<Type> systemTypesInPriorityOrder)
        {
            if (systemTypesInPriorityOrder == null || !systemTypesInPriorityOrder.Any())
            {
                throw new ArgumentException("systemTypesInPriorityOrder must contain at least one element.");
            }

            SystemPriorityByType = systemTypesInPriorityOrder
                .Select((type, i) => new { SystemType = type, Priority = i })
                .ToDictionary(o => o.SystemType, o => o.Priority);
        }

        public IQueryPlan<TKey, TResult> Reduce(IEnumerable<SystemGeneratedResult<IQueryPlan<TKey, TResult>>> systemGeneratedPlans)
        {
            return new ChainOfCommandQueryStep(systemGeneratedPlans.OrderBy(o => GetPriority(o.SystemType)).Select(o => o.Result));
        }

        private int GetPriority(Type systemType)
        {
            if (SystemPriorityByType.ContainsKey(systemType))
            {
                return SystemPriorityByType[systemType];
            }
            else
            {
                return int.MaxValue;
            }
        }

        private class ChainOfCommandQueryStep : IQueryPlan<TKey, TResult>
        {
            private IEnumerable<IQueryPlan<TKey, TResult>> QueriesInPriorityOrder;

            public ChainOfCommandQueryStep(IEnumerable<IQueryPlan<TKey, TResult>> queriesInPriorityOrder)
            {
                this.QueriesInPriorityOrder = queriesInPriorityOrder.Where(o => o.State != QueryPlanState.Empty);
            }
            
            public QueryPlanState State
            {
                get
                {
                    if (!QueriesInPriorityOrder.Any())
                    {
                        return QueryPlanState.Empty;
                    }
                    else if (QueriesInPriorityOrder.Any(o => o.State == QueryPlanState.Materialized))
                    {
                        return QueryPlanState.Materialized;
                    }
                    else
                    {
                        return QueryPlanState.Immaterial;
                    }
                }
            }

            public IEnumerable<IKeyWith<TKey, TResult>> Execute(Expression<Func<TKey, bool>> predicate)
            {
                if (State == QueryPlanState.Empty)
                {
                    return Enumerable.Empty<IKeyWith<TKey, TResult>>();
                }
                else if (State == QueryPlanState.Materialized)
                {
                    return ExecuteMaterialized(predicate);
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
                    IQueryable<IKeyWith<TKey, TResult>> results = QueriesInPriorityOrder.First().ImmaterialQuery;
                    
                    foreach (var query in QueriesInPriorityOrder.Skip(1).Select(qry => qry.ImmaterialQuery))
                    {
                        var existingIds = results.Select(high => high.Key);
                        results = results.Concat(
                            query.Where(low => !existingIds.Contains(low.Key)));
                    }

                    return results;
                }
            }

            private IEnumerable<IKeyWith<TKey, TResult>> ExecuteMaterialized(Expression<Func<TKey, bool>> predicate)
            {
                var resultsByKey = new Dictionary<TKey,IKeyWith<TKey, TResult>>();

                foreach (var result in QueriesInPriorityOrder.SelectMany(qry => qry.Execute(predicate)))
                {
                    if (!resultsByKey.ContainsKey(result.Key))
                    {
                        resultsByKey[result.Key] = result;
                    }
                }

                return resultsByKey.Values;
            }
        }
    }
}

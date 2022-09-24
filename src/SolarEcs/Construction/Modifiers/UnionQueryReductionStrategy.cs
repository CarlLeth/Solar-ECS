using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.Modifiers
{
    public class UnionQueryReductionStrategy<TKey, TResult> : IReductionStrategy<IQueryPlan<TKey, TResult>>
    {
        public IQueryPlan<TKey, TResult> Reduce(IEnumerable<SystemGeneratedResult<IQueryPlan<TKey, TResult>>> systemGeneratedPlans)
        {
            var queries = systemGeneratedPlans.Select(o => o.Result);
            return new UnionQueryPlan<TKey, TResult>(queries);
        }
    }
}

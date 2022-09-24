using Solar.Ecs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    public interface IUnresolvedJoin<TKey, TTuple>
    {
        IQueryPlan<TKey, TTuple> TupledPlan { get; }
    }
}

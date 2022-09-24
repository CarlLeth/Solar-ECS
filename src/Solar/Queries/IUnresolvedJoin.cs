using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public interface IUnresolvedJoin<TKey, TTuple>
    {
        IQueryPlan<TKey, TTuple> TupledPlan { get; }
    }
}

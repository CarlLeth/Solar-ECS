using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs.WritePlans
{
    public class QueryStarterWritePlan<T> : IWritePlan<T>
    {
        public IQueryPlan<T> ExistingModels { get; }

        public QueryStarterWritePlan(IQueryPlan<T> existingModels)
        {
            ExistingModels = existingModels;
        }

        public IEnumerable<ICommitable> Apply(ChangeScript<T> script) => Enumerable.Empty<ICommitable>();
    }
}

using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs.WritePlans
{
    public class EmptyWritePlan<T> : IWritePlan<T>
    {
        public IQueryPlan<T> ExistingModels => QueryPlan.Empty<T>();
        public IEnumerable<ICommitable> Apply(ChangeScript<T> script) => Enumerable.Empty<ICommitable>();
    }
}

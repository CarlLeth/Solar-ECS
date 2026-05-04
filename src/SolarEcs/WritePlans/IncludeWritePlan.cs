using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs.WritePlans
{
    public class IncludeWritePlan<TModel, TPart> : IWritePlan<TModel>
    {
        public IWritePlan<TModel> BasePlan { get; }
        public IWritePlan<TPart> IncludedPlan { get; }
        public Func<ChangeScript<TModel>, IQueryPlan<TPart>, ChangeScript<TPart>> GetPartScript { get; }

        public IncludeWritePlan(IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan, Func<ChangeScript<TModel>, IQueryPlan<TPart>, ChangeScript<TPart>> getPartScript)
        {
            BasePlan = basePlan;
            IncludedPlan = includedPlan;
            GetPartScript = getPartScript;
        }

        public IQueryPlan<TModel> ExistingModels => BasePlan.ExistingModels;

        public IEnumerable<ICommitable> Apply(ChangeScript<TModel> script)
        {
            return Enumerable.Concat(
                BasePlan.Apply(script),
                IncludedPlan.Apply(GetPartScript(script, IncludedPlan.ExistingModels))
            ).ToList();
        }
    }
}

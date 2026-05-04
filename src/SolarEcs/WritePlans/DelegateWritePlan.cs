using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.WritePlans
{
    public class DelegateWritePlan<T> : IWritePlan<T>
    {
        public IQueryPlan<T> ExistingModels { get; }

        private Func<ChangeScript<T>, IEnumerable<ICommitable>> ApplyFunc;

        public DelegateWritePlan(IQueryPlan<T> existingModels, Func<ChangeScript<T>, IEnumerable<ICommitable>> applyFunc)
        {
            ExistingModels = existingModels;
            ApplyFunc = applyFunc;
        }

        public IEnumerable<ICommitable> Apply(ChangeScript<T> script)
        {
            return ApplyFunc(script);
        }
    }
}

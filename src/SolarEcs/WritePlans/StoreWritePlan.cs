using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.WritePlans
{
    public class StoreWritePlan<T> : IWritePlan<T>
    {
        public IStore<T> Store { get; }

        public IQueryPlan<T> ExistingModels => Store.ToQueryPlan();

        public StoreWritePlan(IStore<T> store)
        {
            Store = store;
        }

        public IEnumerable<ICommitable> Apply(ChangeScript<T> script)
        {
            var trans = Store.CreateTransaction();

            foreach (var unassignment in script.Unassign)
            {
                trans.Unassign(unassignment);
            }

            foreach (var assignment in script.Assign)
            {
                trans.Assign(assignment.Key, assignment.Value);
            }

            return trans.ApplyChanges();
        }
    }
}

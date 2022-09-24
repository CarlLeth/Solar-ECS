using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Transactions
{
    public class EmptyTransaction<TModel> : ITransaction<TModel>
    {
        public bool IsValid { get; private set; }

        public EmptyTransaction()
        {
            IsValid = true;
        }

        public bool CanAssign(Guid id, TModel model)
        {
            return true;
        }

        public void Assign(Guid id, TModel component)
        {
            //Do nothing
        }

        public void Unassign(Guid id)
        {
            //Do nothing
        }

        public IEnumerable<ICommitable> ApplyChanges()
        {
            return Enumerable.Empty<ICommitable>();
        }

        public void Invalidate(string failureMessage = null)
        {
            IsValid = false;
        }

        public IQueryPlan<TModel> ExistingModels
        {
            get { return QueryPlan.Empty<TModel>(); }
        }
    }
}

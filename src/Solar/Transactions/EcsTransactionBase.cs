using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Transactions
{
    public abstract class EcsTransactionBase<TModel> : ITransaction<TModel>
    {
        public IQueryPlan<TModel> ExistingModels { get; private set; }
        public bool IsValid { get; private set; }

        private string FailureMessage;

        protected EcsTransactionBase(IQueryPlan<TModel> existingModels)
        {
            this.IsValid = true;
            this.ExistingModels = existingModels;
        }

        public abstract bool CanAssign(Guid id, TModel component);
        public abstract void Assign(Guid id, TModel component);
        public abstract void Unassign(Guid id);

        protected abstract IEnumerable<ICommitable> Apply();

        public void Invalidate(string failureMessage = null)
        {
            IsValid = false;
            FailureMessage = failureMessage;
        }

        public IEnumerable<ICommitable> ApplyChanges()
        {
            EnsureIsValid();
            return Apply();
        }

        private void EnsureIsValid()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(FailureMessage ?? "Transaction has been invalidated.");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Transactions
{
    public class EagerTransaction<TComponent> : EcsTransactionBase<TComponent>
    {
        private Func<Guid, TComponent, bool> CanAssignPredicate;
        private Action<Guid, TComponent> AssignAction;
        private Action<Guid> UnassignAction;
        private Func<IEnumerable<ICommitable>> ApplyChangesAction;

        public EagerTransaction(Func<Guid, TComponent, bool> canAssignPredicate, Action<Guid, TComponent> assignAction, Action<Guid> unassignAction,
            Func<IEnumerable<ICommitable>> applyChangesAction, IQueryPlan<TComponent> existingModels)
            : base(existingModels)
        {
            this.CanAssignPredicate = canAssignPredicate;
            this.AssignAction = assignAction;
            this.UnassignAction = unassignAction;
            this.ApplyChangesAction = applyChangesAction;
        }

        public override bool CanAssign(Guid id, TComponent component)
        {
            return CanAssignPredicate(id, component);
        }

        public override void Assign(Guid id, TComponent component)
        {
            AssignAction(id, component);
        }

        public override void Unassign(Guid id)
        {
            UnassignAction(id);
        }

        protected override IEnumerable<ICommitable> Apply()
        {
            if (ApplyChangesAction != null)
            {
                return ApplyChangesAction();
            }
            else
            {
                return Enumerable.Empty<ICommitable>();
            }
        }
    }
}

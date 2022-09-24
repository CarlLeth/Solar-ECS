using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Transactions
{
    public class ActionBufferingTransaction<TComponent> : EcsTransactionBase<TComponent>
    {
        private Func<Guid, TComponent, bool> CanAssignPredicate;
        private Action<Guid, TComponent> AssignAction;
        private Action<Guid> UnassignAction;

        private List<Action> PendingActions;

        private IEnumerable<ICommitable> Commitables;

        public ActionBufferingTransaction(Func<Guid, TComponent, bool> canAssignPredicate, Action<Guid, TComponent> assignAction, Action<Guid> unassignAction,
            IEnumerable<ICommitable> commitables, IQueryPlan<TComponent> existingModels)
            : base(existingModels)
        {
            this.CanAssignPredicate = canAssignPredicate;
            this.AssignAction = assignAction;
            this.UnassignAction = unassignAction;
            this.Commitables = commitables;

            this.PendingActions = new List<Action>();
        }

        public override bool CanAssign(Guid id, TComponent component)
        {
            return CanAssignPredicate(id, component);
        }

        public override void Assign(Guid id, TComponent component)
        {
            PendingActions.Add(() => AssignAction(id, component));
        }

        public override void Unassign(Guid id)
        {
            PendingActions.Add(() => UnassignAction(id));
        }

        protected override IEnumerable<ICommitable> Apply()
        {
            foreach (var action in PendingActions)
            {
                action();
            }

            PendingActions.Clear();

            return Commitables;
        }
    }
}

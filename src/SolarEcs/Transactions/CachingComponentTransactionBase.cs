using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Transactions
{
    public abstract class CachingComponentTransactionBase<TComponent> : EcsTransactionBase<TComponent>
    {
        private Dictionary<Guid, TComponent> PendingAssigns;
        private HashSet<Guid> PendingUnassigns;

        public CachingComponentTransactionBase(IQueryPlan<TComponent> existingModels)
            : base(existingModels)
        {
            PendingAssigns = new Dictionary<Guid, TComponent>();
            PendingUnassigns = new HashSet<Guid>();
        }

        public override bool CanAssign(Guid id, TComponent component)
        {
            return id != default(Guid);
        }

        public override void Assign(Guid id, TComponent component)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to assign a component to the empty id '{0}'", default(Guid)));
            }

            PendingUnassigns.Remove(id);
            PendingAssigns[id] = component;
        }

        public override void Unassign(Guid id)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to unassign a component from the empty id '{0}'", default(Guid)));
            }

            PendingAssigns.Remove(id);
            PendingUnassigns.Add(id);
        }

        protected override IEnumerable<ICommitable> Apply()
        {
            if (!PendingAssigns.Any() && !PendingUnassigns.Any())
            {
                return Enumerable.Empty<ICommitable>();
            }

            var committables = Apply(PendingAssigns.Select(pair => KeyWith.Create(pair.Key, pair.Value)), PendingUnassigns)
                .ToList();

            PendingAssigns.Clear();
            PendingUnassigns.Clear();

            return committables;
        }

        protected abstract IEnumerable<ICommitable> Apply(IEnumerable<IKeyWith<Guid, TComponent>> assignments, IEnumerable<Guid> unassignments);
    }
}
